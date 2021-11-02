using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class FacilityEntityObjectMother : EntityMotherLogBase<Facility, FacilityEntityObjectMother.CallbackParameters>
    {
        private readonly DbContext _newContext;

        public FacilityEntityObjectMother(ObjectContext oldContext, DbContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContext = newContext;
        }

        private enum EntityTypes
        {
            Facility,
            Location
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<Facility> BirthRecords()
        {
            _loadCount.Reset();

            var facilityId = _newContext.NextIdentity<Facility>(w => w.Id);
            var locationId = _newContext.NextIdentity<Location>(l => l.Id);

            foreach(var warehouse in SelectRecordsToLoad())
            {
                _loadCount.AddRead(EntityTypes.Facility);

                if(WarehouseExcluded(warehouse))
                {
                    Log(new CallbackParameters(CallbackReason.FacilityExcluded)
                        {
                            WarehouseAbbreviation = warehouse.WhouseAbbr
                        });
                    continue;
                }

                var warehouseLocations = new List<Location>();
                if(warehouse.WHID == 0)
                {
                    warehouseLocations.AddRange(new []
                        {
                            CreateProductionLine(1, facilityId),
                            CreateProductionLine(2, facilityId),
                            CreateProductionLine(3, facilityId),
                            CreateProductionLine(4, facilityId),
                            CreateProductionLine(5, facilityId),
                            CreateProductionLine(6, facilityId)
                        });
                }
                foreach(var location in warehouse.tblLocations)
                {
                    _loadCount.AddRead(EntityTypes.Location);

                    var locationType = GetLocationType(location.Street);
                    warehouseLocations.Add(new Location
                        {
                            LocID = location.LocID,
                            FacilityId = facilityId,

                            LocationType = locationType,
                            Active = location.InActive == false,
                            Locked = !string.IsNullOrEmpty(location.FreezeRow),
                            Description = LocationDescriptionHelper.GetDescription(location.Street, location.Row ?? 0),
                        });
                    _loadCount.AddLoaded(EntityTypes.Location);
                    locationId += 1;
                }

                _loadCount.AddLoaded(EntityTypes.Facility);

                yield return new Facility
                    {
                        WHID = warehouse.WHID,
                        Id = facilityId,
                        Active = warehouse.InActive == false,
                        Name = warehouse.WhouseAbbr,
                        PhoneNumber = warehouse.Phone,
                        EMailAddress = warehouse.Email,
                        ShippingLabelName = warehouse.Company,
                        FacilityType = GetFacilityType(warehouse),

                        Address = new Address
                            {
                                AddressLine1 = warehouse.Address1,
                                AddressLine2 = warehouse.Address2,
                                AddressLine3 = warehouse.Address3,
                                City = warehouse.City,
                                State = warehouse.State,
                                PostalCode = warehouse.Zip,
                                Country = warehouse.Country
                            },

                        Locations = warehouseLocations
                    };
                facilityId += 1;
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private static LocationType GetLocationType(string street)
        {
            if(!string.IsNullOrWhiteSpace(street))
            {
                street = street.ToUpper();
                switch(street)
                {
                    case "PROD": return LocationType.ProductionStaging;
                    case "SHIP": return LocationType.Shipping;
                    case "LINE": return LocationType.ProductionLine;
                }
            }

            return LocationType.Warehouse;
        }

        private List<tblWarehouse> SelectRecordsToLoad()
        {
            return OldContext.CreateObjectSet<tblWarehouse>()
                .Include("tblLocations")
                .ToList();
        }

        private static bool WarehouseExcluded(tblWarehouse warehouse)
        {
            return WarehouseExclusions.Any(w => w == warehouse.WhouseAbbr);
        }

        private static FacilityType GetFacilityType(tblWarehouse warehouse)
        {
            FacilityType type;
            return FacilityTypes.TryGetValue(warehouse.WhouseAbbr, out type) ? type : FacilityType.Internal;
        }

        private static Location CreateProductionLine(int line, int facilityId)
        {
            return new Location
                {
                    LocationType = LocationType.ProductionLine,
                    Description = LocationDescriptionHelper.GetDescription("Line", line),
                    Active = true,
                    Locked = false,
                    FacilityId = facilityId
                };
        }

        private static readonly List<string> WarehouseExclusions = new List<string>
            {
                //"Isomedix",
                //"Sterigenics/NJ"
            };

        private static readonly Dictionary<string, FacilityType> FacilityTypes = new Dictionary<string, FacilityType>
            {
                 { "Isomedix", FacilityType.Treatment },
                 { "Sterigenics/NJ", FacilityType.Treatment },
            };


        public enum CallbackReason
        {
            Exception,
            Summary,
            FacilityExcluded,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public string WarehouseAbbreviation { get; set; }

            protected override CallbackReason ExceptionReason { get { return FacilityEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return FacilityEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return FacilityEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case FacilityEntityObjectMother.CallbackReason.Exception: return ReasonCategory.Error;
                    case FacilityEntityObjectMother.CallbackReason.FacilityExcluded: return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }

        }
    }
}
