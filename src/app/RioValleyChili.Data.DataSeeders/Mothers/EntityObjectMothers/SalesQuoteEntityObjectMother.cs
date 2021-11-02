using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class SalesQuoteEntityObjectMother : EntityMotherLogBase<SalesQuote, SalesQuoteEntityObjectMother.CallbackParameters, SalesQuoteEntityObjectMother.EntityType>
    {
        public SalesQuoteEntityObjectMother(RioValleyChiliDataContext newContext, ObjectContext oldContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
        }
        
        private readonly NewContextHelper _newContextHelper;

        protected override IEnumerable<SalesQuote> BirthRecords()
        {
            var dateSequences = new Dictionary<DateTime, int>();
            foreach(var oldQuote in SelectRecordsToLoad())
            {
                LoadCount.AddRead(EntityType.SalesQuote);

                var sourceFacility = _newContextHelper.GetFacility(oldQuote.WHID);
                if(oldQuote.WHID != null && sourceFacility == null)
                {
                    Log(new CallbackParameters(CallbackReason.FacilityNotFound)
                        {
                            tblQuote = oldQuote
                        });
                    continue;
                }

                var customer = _newContextHelper.GetCompany(oldQuote.Company_IA, CompanyType.Customer);
                if(oldQuote.Company_IA != null && customer == null)
                {
                    Log(new CallbackParameters(CallbackReason.CompanyNotFound)
                        {
                            tblQuote = oldQuote
                        });
                    continue;
                }

                var broker = _newContextHelper.GetCompany(oldQuote.Broker, CompanyType.Broker);
                if(oldQuote.Broker != null && broker == null)
                {
                    Log(new CallbackParameters(CallbackReason.BrokerNotFound)
                        {
                            tblQuote = oldQuote
                        });
                    continue;
                }

                var dateCreated = oldQuote.EntryDate.Value.Date;

                var shipmentInformation = new ShipmentInformation
                    {
                        DateCreated = dateCreated,
                        Sequence = _newContextHelper.ShipmentInformationKeys.GetNextSequence(dateCreated),

                        Status = ShipmentStatus.Scheduled,
                        PalletWeight = (double?)oldQuote.PalletOR,
                        PalletQuantity = (int)(oldQuote.PalletQty ?? 0),
                        FreightBillType = oldQuote.FreightBillType,
                        ShipmentMethod = oldQuote.ShipVia,
                        DriverName = oldQuote.Driver,
                        CarrierName = oldQuote.Carrier,
                        TrailerLicenseNumber = oldQuote.TrlNbr,
                        ContainerSeal = oldQuote.ContSeal,

                        RequiredDeliveryDate = oldQuote.DelDueDate,
                        ShipmentDate = oldQuote.Date ?? oldQuote.SchShipDate,
                        InternalNotes = oldQuote.InternalNotes,
                        ExternalNotes = oldQuote.ExternalNotes,
                        SpecialInstructions = oldQuote.SpclInstr,

                        ShipTo = oldQuote.ShipTo,
                        FreightBill = oldQuote.BillTo
                    };
                
                int sequence;
                if(!dateSequences.TryGetValue(dateCreated, out sequence))
                {
                    sequence = 0;
                }

                var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
                if(oldQuote.EmployeeID == null)
                {
                    Log(new CallbackParameters(CallbackReason.DefaultEmployee)
                        {
                            tblQuote = oldQuote
                        });
                }
                else
                {
                    employeeId = oldQuote.EmployeeID.Value;
                }

                var salesQuote = new SalesQuote
                    {
                        TimeStamp = oldQuote.EntryDate.Value.ConvertLocalToUTC(),
                        EmployeeId = employeeId,

                        DateCreated = dateCreated,
                        Sequence = sequence,

                        QuoteNum = oldQuote.QuoteNum,
                        QuoteDate = oldQuote.Date.Value,
                        DateReceived = oldQuote.DateRecd,
                        CalledBy = oldQuote.From,
                        TakenBy = oldQuote.TakenBy,
                        PaymentTerms = oldQuote.PayTerms,

                        SourceFacilityId = sourceFacility == null ? (int?)null : sourceFacility.Id,
                        CustomerId = customer == null ? (int?)null : customer.Id,
                        BrokerId = broker == null ? (int?)null : broker.Id,

                        ShipmentInfoDateCreated = shipmentInformation.DateCreated,
                        ShipmentInfoSequence = shipmentInformation.Sequence,
                        ShipmentInformation = shipmentInformation,

                        SoldTo = oldQuote.SoldTo,

                        Items = new List<SalesQuoteItem>()
                    };

                var itemSequence = 0;
                foreach(var detail in oldQuote.Details)
                {
                    LoadCount.AddRead(EntityType.SalesQuoteItem);

                    var product = _newContextHelper.GetProduct(detail.ProdID);
                    if(product == null)
                    {
                        Log(new CallbackParameters(CallbackReason.ProductNotFound)
                            {
                                tblQuoteDetail = detail
                            });
                        continue;
                    }

                    var packaging = _newContextHelper.GetPackagingProduct(detail.PkgID);
                    if(packaging == null)
                    {
                        Log(new CallbackParameters(CallbackReason.PackagingNotFound)
                        {
                            tblQuoteDetail = detail
                        });
                        continue;
                    }

                    var treatment = _newContextHelper.GetInventoryTreatment(detail.TrtmtID ?? 0);
                    if(treatment == null)
                    {
                        Log(new CallbackParameters(CallbackReason.TreatmentNotFound)
                            {
                                tblQuoteDetail = detail
                            });
                        continue;
                    }

                    salesQuote.Items.Add(new SalesQuoteItem
                        {
                            DateCreated = salesQuote.DateCreated,
                            Sequence = salesQuote.Sequence,
                            ItemSequence = itemSequence++,
                            
                            QDetailID = detail.QDetail,

                            ProductId = product.ProductKey.ProductKey_ProductId,
                            PackagingProductId = packaging.Id,
                            TreatmentId = treatment.Id,
                            
                            Quantity = (int) (detail.Quantity ?? 0),
                            PriceBase = (double) (detail.Price ?? 0),
                            PriceFreight = (double) (detail.FreightP ?? 0),
                            PriceTreatment = (double) (detail.TrtnmntP ?? 0),
                            PriceWarehouse = (double) (detail.WHCostP ?? 0),
                            PriceRebate = (double) (detail.Rebate ?? 0),
                            CustomerProductCode = detail.CustProductCode,
                        });
                }

                LoadCount.AddLoaded(EntityType.SalesQuote);
                LoadCount.AddLoaded(EntityType.SalesQuoteItem, (uint) salesQuote.Items.Count);

                yield return salesQuote;

                if(!dateSequences.ContainsKey(dateCreated))
                {
                    dateSequences.Add(dateCreated, sequence);
                }
                dateSequences[dateCreated] += 1;
            }
        }

        private IEnumerable<tblQuoteDTO> SelectRecordsToLoad()
        {
            return OldContext.CreateObjectSet<tblQuote>()
                .AsNoTracking()
                .Select(q => new tblQuoteDTO
                    {
                        QuoteNum = q.QuoteNum,
                        Date = q.Date,
                        DateRecd = q.DateRecd,
                        From = q.From,
                        TakenBy = q.TakenBy,

                        EmployeeID = q.EmployeeID,
                        EntryDate = q.EntryDate,
                        WHID = q.WHID,
                        Company_IA = q.Company_IA,
                        Broker = q.Broker,

                        Status = q.Status,
                        PalletOR = q.PalletOR,
                        PalletQty = q.PalletQty,
                        FreightBillType = q.FreightBillType,
                        ShipVia = q.ShipVia,
                        Driver = q.Driver,
                        Carrier = q.Carrier,
                        TrlNbr = q.TrlNbr,
                        ContSeal = q.ContSeal,
                        DelDueDate = q.DelDueDate,
                        SchShipDate = q.SchdShipDate,
                        PayTerms = q.PayTerms,
                        SpclInstr = q.SpclInstr,
                        InternalNotes = q.InternalNotes,
                        ExternalNotes = q.ExternalNotes,

                        SoldTo = new SoldToLabel
                            {
                                Name = q.CCompany,
                                Address = new Address
                                {
                                    AddressLine1 = q.CAddress1,
                                    AddressLine2 = q.CAddress2,
                                    AddressLine3 = q.CAddress3,
                                    City = q.CCity,
                                    State = q.CState,
                                    PostalCode = q.CZip,
                                    Country = q.CCountry
                                }
                            },
                        ShipTo = new ShipToLabel
                            {
                                Name = q.SCompany,
                                Phone = q.SPhone,
                                Address = new Address
                                {
                                    AddressLine1 = q.SAddress1,
                                    AddressLine2 = q.SAddress2,
                                    AddressLine3 = q.SAddress3,
                                    City = q.SCity,
                                    State = q.SState,
                                    PostalCode = q.SZip,
                                    Country = q.SCountry
                                }
                            },
                        BillTo = new BillToLabel
                            {
                                Name = q.Company,
                                Phone = q.Phone,
                                EMail = q.Email,
                                Fax = q.Fax,
                                Address = new Address
                                {
                                    AddressLine1 = q.Address1,
                                    AddressLine2 = q.Address2,
                                    AddressLine3 = q.Address3,
                                    City = q.City,
                                    State = q.State,
                                    PostalCode = q.Zip,
                                    Country = q.Country
                                }
                            },

                        Details = q.tblQuoteDetails.Select(d => new tblQuoteItemDTO
                            {
                                QDetail = d.QDetail,
                                ProdID = d.ProdID,
                                PkgID = d.PkgID,
                                TrtmtID = d.TrtmtID,
                                Quantity = d.Quantity,
                                Price = d.Price,
                                FreightP = d.FreightP,
                                TrtnmntP = d.TrtnmntP,
                                WHCostP = d.WHCostP,
                                Rebate = d.Rebate,
                                CustProductCode = d.CustProductCode
                            })
                    });
        }

        public class tblQuoteDTO
        {
            public int QuoteNum { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? DateRecd { get; set; }
            public string From { get; set; }
            public string TakenBy { get; set; }

            public int? EmployeeID { get; set; }
            public DateTime? EntryDate { get; set; }

            public int? WHID { get; set; }
            public string Company_IA { get; set; }
            public string Broker { get; set; }

            public int? Status { get; set; }
            public decimal? PalletOR { get; set; }
            public decimal? PalletQty { get; set; }
            public string FreightBillType { get; set; }
            public string ShipVia { get; set; }
            public string Driver { get; set; }
            public string Carrier { get; set; }
            public string TrlNbr { get; set; }
            public string ContSeal { get; set; }
            public DateTime? DelDueDate { get; set; }
            public DateTime? SchShipDate { get; set; }
            public string PayTerms { get; set; }
            public string SpclInstr { get; set; }
            public string InternalNotes { get; set; }
            public string ExternalNotes { get; set; }

            public SoldToLabel SoldTo { get; set; }
            public ShipToLabel ShipTo { get; set; }
            public BillToLabel BillTo { get; set; }

            public IEnumerable<tblQuoteItemDTO> Details { get; set; }
        }

        public class tblQuoteItemDTO
        {
            public DateTime QDetail { get; set; }

            public int? ProdID { get; set; }
            public int? PkgID { get; set; }
            public int? TrtmtID { get; set; }
            public decimal? Quantity { get; set; }
            public decimal? Price { get; set; }
            public decimal? FreightP { get; set; }
            public decimal? TrtnmntP { get; set; }
            public decimal? WHCostP { get; set; }
            public decimal? Rebate { get; set; }
            public string CustProductCode { get; set; }
        }

        public class SoldToLabel : ShippingLabel { }
        public class ShipToLabel : ShippingLabel { }
        public class BillToLabel : ShippingLabel { }

        public enum EntityType
        {
            SalesQuote,
            SalesQuoteItem
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            DefaultEmployee,
            FacilityNotFound,
            CompanyNotFound,
            BrokerNotFound,
            ProductNotFound,
            PackagingNotFound,
            TreatmentNotFound
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override CallbackReason ExceptionReason { get { return SalesQuoteEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return SalesQuoteEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return SalesQuoteEntityObjectMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case SalesQuoteEntityObjectMother.CallbackReason.FacilityNotFound:
                    case SalesQuoteEntityObjectMother.CallbackReason.CompanyNotFound:
                    case SalesQuoteEntityObjectMother.CallbackReason.BrokerNotFound:
                    case SalesQuoteEntityObjectMother.CallbackReason.ProductNotFound:
                    case SalesQuoteEntityObjectMother.CallbackReason.PackagingNotFound:
                    case SalesQuoteEntityObjectMother.CallbackReason.TreatmentNotFound:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }

            public tblQuoteDTO tblQuote { get; set; }
            public tblQuoteItemDTO tblQuoteDetail { get; set; }
        }
    }
}