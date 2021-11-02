using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class SampleOrderEntityObjectMother : EntityMotherLogBase<SampleOrder, SampleOrderEntityObjectMother.CallbackParameters, SampleOrderEntityObjectMother.EntityType>
    {
        public SampleOrderEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        public enum EntityType
        {
            SampleOrder,
            SampleOrderJournalEntry,
            SampleOrderItem,
            SampleOrderItemSpec,
            SampleOrderItemMatch
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            tblSample_EmployeeID_Null,
            tblSampleNote_EmployeeID_Null,
            tblSampleDetail_MissingProduct,
            tblSampleDetail_InvalidLotNumber,
            tblSampleDetail_MissingLot,
            tblSample_MissingCompanyIA,
            tblSample_MissingBroker,
            tblSample_InvalidStatus
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            public tblSampleDTO tblSample { get; set; }
            public tblSampleNoteDTO tblSampleNote { get; set; }
            public tblSampleDetailDTO tblSampleDetail { get; set; }

            protected override CallbackReason ExceptionReason { get { return SampleOrderEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return SampleOrderEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return SampleOrderEntityObjectMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_MissingProduct:
                    case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_InvalidLotNumber:
                    case SampleOrderEntityObjectMother.CallbackReason.tblSampleDetail_MissingLot:
                    case SampleOrderEntityObjectMother.CallbackReason.tblSample_MissingCompanyIA:
                    case SampleOrderEntityObjectMother.CallbackReason.tblSample_MissingBroker:
                    case SampleOrderEntityObjectMother.CallbackReason.tblSample_InvalidStatus:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }

        protected override IEnumerable<SampleOrder> BirthRecords()
        {
            foreach(var tblSample in SelectRecordsToLoad())
            {
                var sampleOrder = LoadSampleOrder(tblSample);
                if(sampleOrder != null)
                {
                    LoadCount.AddLoaded(EntityType.SampleOrder);
                    LoadCount.AddLoaded(EntityType.SampleOrderJournalEntry, (uint) sampleOrder.JournalEntries.Count);
                    LoadCount.AddLoaded(EntityType.SampleOrderItem, (uint) sampleOrder.Items.Count);
                    LoadCount.AddLoaded(EntityType.SampleOrderItemSpec, (uint) sampleOrder.Items.Count(i => i.Spec != null));
                    LoadCount.AddLoaded(EntityType.SampleOrderItemMatch, (uint) sampleOrder.Items.Count(i => i.Match != null));

                    yield return sampleOrder;
                }
            }
        }

        private SampleOrder LoadSampleOrder(tblSampleDTO tblSample)
        {
            LoadCount.AddRead(EntityType.SampleOrder);

            var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
            if(tblSample.EmployeeID != null)
            {
                employeeId = tblSample.EmployeeID.Value;
            }
            else
            {
                Log(new CallbackParameters(CallbackReason.tblSample_EmployeeID_Null)
                    {
                        tblSample = tblSample
                    });
            }

            var requestCustomer = _newContextHelper.GetCompany(tblSample.Company_IA, CompanyType.Customer);
            if(requestCustomer == null && !string.IsNullOrWhiteSpace(tblSample.Company_IA))
            {
                Log(new CallbackParameters(CallbackReason.tblSample_MissingCompanyIA)
                    {
                        tblSample = tblSample
                    });
                return null;
            }

            var broker = _newContextHelper.GetCompany(tblSample.Broker, CompanyType.Broker);
            if(broker == null && !string.IsNullOrWhiteSpace(tblSample.Broker))
            {
                Log(new CallbackParameters(CallbackReason.tblSample_MissingBroker)
                    {
                        tblSample = tblSample
                    });
                return null;
            }

            var status = ParseStatus(tblSample.Status);
            if(status == null)
            {
                Log(new CallbackParameters(CallbackReason.tblSample_InvalidStatus)
                    {
                        tblSample = tblSample
                    });
                return null;
            }

            var dateDue = tblSample.SampleDate ?? tblSample.EntryDate.GetDate().Value;
            var dateReceived = tblSample.DateRecd ?? tblSample.EntryDate.GetDate().Value;
            var dateCompleted = tblSample.DateCompleted;
            if(tblSample.Completed && dateCompleted == null)
            {
                dateCompleted = tblSample.SampleDate ?? tblSample.EntryDate.Value;
            }

            var sampleOrderKey = ParseSampleOrderKey(tblSample.SampleID);
            var sampleOrder = new SampleOrder
                {
                    Year = sampleOrderKey.SampleOrderKey_Year,
                    Sequence = sampleOrderKey.SampleOrderKey_Sequence,
                    EmployeeId = employeeId,
                    TimeStamp = tblSample.EntryDate.Value.ConvertLocalToUTC(),

                    Comments = tblSample.Comments,
                    PrintNotes = tblSample.Notes2Print,
                    Volume = (double)(tblSample.Volume ?? 0),
                    DateDue = dateDue,
                    DateReceived = dateReceived,
                    DateCompleted = dateCompleted,
                    ShipmentMethod = tblSample.Priority,
                    FOB = tblSample.FOB,
                    Status = status.Value,
                    Active = tblSample.Active,

                    RequestCustomerId = requestCustomer == null ? (int?)null : requestCustomer.Id,
                    BrokerId = broker == null ? (int?)null : broker.Id,

                    Request = new ShippingLabel
                        {
                            Name = tblSample.Contact_IA,
                            Address = new Address
                                {
                                    AddressLine1 = tblSample.Address1_IA,
                                    AddressLine2 = tblSample.Address2_IA,
                                    AddressLine3 = tblSample.Address3_IA,
                                    City = tblSample.City_IA,
                                    State = tblSample.State_IA,
                                    PostalCode = tblSample.Zip_IA,
                                    Country = tblSample.Country_IA,
                                }
                        },

                    ShipToCompany = tblSample.SCompany,
                    ShipTo = new ShippingLabel
                        {
                            Name = tblSample.SContact,
                            Phone = tblSample.SPhone,
                            Address = new Address
                                {
                                    AddressLine1 = tblSample.SAddress1,
                                    AddressLine2 = tblSample.SAddress2,
                                    AddressLine3 = tblSample.SAddress3,
                                    City = tblSample.SCity,
                                    State = tblSample.SState,
                                    PostalCode = tblSample.SZip,
                                    Country = tblSample.SCountry
                                }
                        },

                    SampleID = tblSample.SampleID
                };

            LoadNotes(sampleOrder, tblSample);
            LoadDetails(sampleOrder, tblSample);

            return sampleOrder;
        }

        private static SampleOrderStatus? ParseStatus(string status)
        {
            if(!string.IsNullOrWhiteSpace(status))
            {
                switch(new string(status.Where(c => !Char.IsWhiteSpace(c)).Select(Char.ToUpper).ToArray()))
                {
                    case "PENDING": return SampleOrderStatus.Pending;
                    case "SENT": return SampleOrderStatus.Sent;
                    case "REJECTED": return SampleOrderStatus.Rejected;
                    case "APPROVED": return SampleOrderStatus.Approved;
                    case "SEEJOURNALENTRY": return SampleOrderStatus.SeeJournalEntry;
                }
            }

            return null;
        }

        private void LoadNotes(SampleOrder sampleOrder, tblSampleDTO tblSample)
        {
            var journalEntries = new List<SampleOrderJournalEntry>();
            var noteSequence = 1;
            foreach(var tblSampleNote in tblSample.tblSampleNotes)
            {
                LoadCount.AddRead(EntityType.SampleOrderJournalEntry);

                var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
                if(tblSampleNote.EmployeeID != null)
                {
                    employeeId = tblSampleNote.EmployeeID.Value;
                }
                else
                {
                    Log(new CallbackParameters(CallbackReason.tblSampleNote_EmployeeID_Null)
                        {
                            tblSampleNote = tblSampleNote
                        });
                }

                journalEntries.Add(new SampleOrderJournalEntry
                    {
                        SampleOrderYear = sampleOrder.Year,
                        SampleOrderSequence = sampleOrder.Sequence,
                        EntrySequence = noteSequence++,

                        EmployeeId = employeeId,
                        Date = tblSampleNote.SampleJnlDate,
                        Text = tblSampleNote.SampleNote,

                        SamNoteID = tblSampleNote.SamNoteID
                    });
            }

            sampleOrder.JournalEntries = journalEntries;
        }

        private void LoadDetails(SampleOrder sampleOrder, tblSampleDTO tblSample)
        {
            var items = new List<SampleOrderItem>();
            var itemSequence = 1;
            foreach(var tblSampleDetail in tblSample.tblSampleDetails)
            {
                LoadCount.AddRead(EntityType.SampleOrderItem);

                var product = _newContextHelper.GetProduct(tblSampleDetail.ProdID);
                if(product == null && tblSampleDetail.ProdID != null)
                {
                    Log(new CallbackParameters(CallbackReason.tblSampleDetail_MissingProduct)
                        {
                            tblSampleDetail = tblSampleDetail
                        });
                    continue;
                }

                LotKey lotKey = null;
                if(tblSampleDetail.Lot != null)
                {
                    lotKey = LotNumberParser.ParseLotNumber(tblSampleDetail.Lot.Value);
                    if(lotKey == null)
                    {
                        Log(new CallbackParameters(CallbackReason.tblSampleDetail_InvalidLotNumber)
                            {
                                tblSampleDetail = tblSampleDetail
                            });
                        continue;
                    }

                    if(!_newContextHelper.LotLoaded(lotKey))
                    {
                        Log(new CallbackParameters(CallbackReason.tblSampleDetail_MissingLot)
                            {
                                tblSampleDetail = tblSampleDetail
                            });
                        continue;
                    }
                }

                var sampleOrderItem = new SampleOrderItem
                    {
                        SampleOrderYear = sampleOrder.Year,
                        SampleOrderSequence = sampleOrder.Sequence,
                        ItemSequence = itemSequence++,

                        ProductId = product == null ? (int?)null : product.ProductKey.ProductKey_ProductId,

                        LotDateCreated = lotKey == null ? (DateTime?)null : lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey == null ? (int?)null : lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey == null ? (int?)null : lotKey.LotKey_LotTypeId,
                        
                        Quantity = (int) (tblSampleDetail.Qty ?? 0),
                        Description = tblSampleDetail.Desc,
                        CustomerProductName = tblSampleDetail.SampleMatch,

                        SampleDetailID = tblSampleDetail.SampleDetailID,
                    };

                LoadItemSpec(tblSampleDetail.tblSampleCustSpec, sampleOrderItem);
                LoadItemMatch(tblSampleDetail.tblSampleRVCMatch, sampleOrderItem);

                items.Add(sampleOrderItem);
            }

            sampleOrder.Items = items;
        }

        private void LoadItemSpec(tblSampleCustSpecDTO tblSampleCustSpec, SampleOrderItem orderItem)
        {
            if(tblSampleCustSpec == null)
            {
                return;
            }

            LoadCount.AddRead(EntityType.SampleOrderItemSpec);

            orderItem.Spec = new SampleOrderItemSpec
                {
                    SampleOrderYear = orderItem.SampleOrderYear,
                    SampleOrderSequence = orderItem.SampleOrderSequence,
                    ItemSequence = orderItem.ItemSequence,

                    AstaMin = tblSampleCustSpec.AstaMin,
                    AstaMax = tblSampleCustSpec.AstaMax,
                    MoistureMin = tblSampleCustSpec.MoistureMin,
                    MoistureMax = tblSampleCustSpec.MoistureMax,
                    WaterActivityMin = tblSampleCustSpec.WaterActivityMin,
                    WaterActivityMax = tblSampleCustSpec.WaterActivityMax,
                    Mesh = tblSampleCustSpec.Mesh,
                    AoverB = tblSampleCustSpec.AoverB,
                    ScovMin = tblSampleCustSpec.ScovMin,
                    ScovMax = tblSampleCustSpec.ScovMax,
                    ScanMin = tblSampleCustSpec.ScanMin,
                    ScanMax = tblSampleCustSpec.ScanMax,
                    TPCMin = tblSampleCustSpec.TPCMin,
                    TPCMax = tblSampleCustSpec.TPCMax,
                    YeastMin = tblSampleCustSpec.YeastMin,
                    YeastMax = tblSampleCustSpec.YeastMax,
                    MoldMin = tblSampleCustSpec.MoldMin,
                    MoldMax = tblSampleCustSpec.MoldMax,
                    ColiformsMin = tblSampleCustSpec.ColiformsMin,
                    ColiformsMax = tblSampleCustSpec.ColiformsMax,
                    EColiMin = tblSampleCustSpec.EColiMin,
                    EColiMax = tblSampleCustSpec.EColiMax,
                    SalMin = tblSampleCustSpec.SalMin,
                    SalMax = tblSampleCustSpec.SalMax,

                    Notes = tblSampleCustSpec.Notes,

                    CustSpecID = tblSampleCustSpec.CustSpecID
                };
        }

        private void LoadItemMatch(tblSampleRVCMatchDTO tblSampleRVCMatch, SampleOrderItem orderItem)
        {
            if(tblSampleRVCMatch == null)
            {
                return;
            }

            LoadCount.AddRead(EntityType.SampleOrderItemMatch);

            orderItem.Match = new SampleOrderItemMatch
                {
                    SampleOrderYear = orderItem.SampleOrderYear,
                    SampleOrderSequence = orderItem.SampleOrderSequence,
                    ItemSequence = orderItem.ItemSequence,

                    Gran = tblSampleRVCMatch.Gran,
                    AvgAsta = tblSampleRVCMatch.AvgAsta,
                    AoverB = tblSampleRVCMatch.AoverB,
                    AvgScov = tblSampleRVCMatch.AvgScov,
                    H2O = tblSampleRVCMatch.H2O,
                    Scan = tblSampleRVCMatch.Scan,
                    Yeast = tblSampleRVCMatch.Yeast,
                    Mold = tblSampleRVCMatch.Mold,
                    Coli = tblSampleRVCMatch.Coli,
                    TPC = tblSampleRVCMatch.TPC,
                    EColi = tblSampleRVCMatch.EColi,
                    Sal = tblSampleRVCMatch.Sal,
                    InsPrts = tblSampleRVCMatch.InsPrts,
                    RodHrs = tblSampleRVCMatch.RodHrs,

                    Notes = tblSampleRVCMatch.Notes,

                    RVCMatchID = tblSampleRVCMatch.RVCMatchID
                };
        }

        private static SampleOrderKey ParseSampleOrderKey(int sampleID)
        {
            var year = sampleID / 1000;
            var sequence = sampleID - (year * 1000);

            return new SampleOrderKey(new SampleOrderKeyDTO
                {
                    SampleOrderKey_Year = year,
                    SampleOrderKey_Sequence = sequence
                });
        }

        private readonly NewContextHelper _newContextHelper;

        private IEnumerable<tblSampleDTO> SelectRecordsToLoad()
        {
            return OldContext.CreateObjectSet<tblSample>()
                .AsNoTracking()
                .Select(s => new tblSampleDTO
                    {
                        SampleID = s.SampleID,
                        EmployeeID = s.EmployeeID,
                        EntryDate = s.EntryDate,

                        Comments = s.Comments,
                        Notes2Print = s.Notes2Print,

                        Contact_IA = s.Contact_IA,
                        Company_IA = s.Company_IA,
                        Address1_IA = s.Address1_IA,
                        Address2_IA = s.Address2_IA,
                        Address3_IA = s.Address3_IA,
                        City_IA = s.City_IA,
                        State_IA = s.State_IA,
                        Zip_IA = s.Zip_IA,
                        Country_IA = s.Country_IA,

                        SContact = s.SContact,
                        SCompany = s.SCompany,
                        SAddress1 = s.SAddress1,
                        SAddress2 = s.SAddress2,
                        SAddress3 = s.SAddress3,
                        SCity = s.SCity,
                        SState = s.SState,
                        SZip = s.SZip,
                        SCountry = s.SCountry,
                        SPhone = s.SPhone,

                        Broker = s.Broker,
                        Volume = s.Volume,
                        SampleDate = s.SampleDate,
                        DateRecd = s.DateRecd,
                        DateCompleted = s.DateCompleted,
                        Priority = s.Priority,
                        FOB = s.FOB,
                        Status = s.Status,
                        Completed = s.Completed,
                        Active = s.Active,

                        tblSampleNotes = s.tblSampleNotes.Select(n => new tblSampleNoteDTO
                            {
                                SamNoteID = n.SamNoteID,
                                SampleJnlDate = n.SampleJnlDate,
                                SampleNote = n.SampleNote,
                                EmployeeID = n.EmployeeID
                            }),

                        tblSampleDetails = s.tblSampleDetails.Select(d => new tblSampleDetailDTO
                            {
                                SampleDetailID = d.SampleDetailID,
                                ProdID = d.ProdID,
                                Lot = d.Lot,
                                Qty = d.Qty,
                                Desc = d.Desc,
                                SampleMatch = d.SampleMatch,

                                tblSampleCustSpec = d.tblSampleCustSpecs.OrderByDescending(c => c.CustSpecID)
                                    .Select(c => new tblSampleCustSpecDTO
                                        {
                                            CustSpecID = c.CustSpecID,

                                            AstaMin = c.AstaMin,
                                            AstaMax = c.AstaMax,
                                            MoistureMin = c.MoistureMin,
                                            MoistureMax = c.MoistureMax,
                                            WaterActivityMin = c.WaterActivityMin,
                                            WaterActivityMax = c.WaterActivityMax,
                                            Mesh = c.Mesh,
                                            AoverB = c.AoverB,
                                            ScovMin = c.ScovMin,
                                            ScovMax = c.ScovMax,
                                            ScanMin = c.ScanMin,
                                            ScanMax = c.ScanMax,
                                            TPCMin = c.TPCMin,
                                            TPCMax = c.TPCMax,
                                            YeastMin = c.YeastMin,
                                            YeastMax = c.YeastMax,
                                            MoldMin = c.MoldMin,
                                            MoldMax = c.MoldMax,
                                            ColiformsMin = c.ColiformsMin,
                                            ColiformsMax = c.ColiformsMax,
                                            EColiMin = c.EColiMin,
                                            EColiMax = c.EColiMax,
                                            SalMin = c.SalMin,
                                            SalMax = c.SalMax,

                                            Notes = c.Notes
                                        })
                                    .FirstOrDefault(),
                                tblSampleRVCMatch = d.tblSampleRVCMatches.OrderByDescending(m => m.RVCMatchID)
                                    .Select(m => new tblSampleRVCMatchDTO
                                        {
                                            RVCMatchID = m.RVCMatchID,

                                            Gran = m.Gran,
                                            AvgAsta = m.AvgAsta,
                                            AoverB = m.AoverB,
                                            AvgScov = m.AvgScov,
                                            H2O = m.H2O,
                                            Scan = m.Scan,
                                            Yeast = m.Yeast,
                                            Mold = m.Mold,
                                            Coli = m.Coli,
                                            TPC = m.TPC,
                                            EColi = m.EColi,
                                            Sal = m.Sal,
                                            InsPrts = m.InsPrts,
                                            RodHrs = m.RodHrs,

                                            Notes = m.Notes
                                        })
                                    .FirstOrDefault()
                            }),
                    });
        }

        public class tblSampleDTO
        {
            public int SampleID { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime? EntryDate { get; set; }

            public string Comments { get; set; }
            public string Notes2Print { get; set; }

            public string Contact_IA { get; set; }
            public string Company_IA { get; set; }
            public string Address1_IA { get; set; }
            public string Address2_IA { get; set; }
            public string Address3_IA { get; set; }
            public string City_IA { get; set; }
            public string State_IA { get; set; }
            public string Zip_IA { get; set; }
            public string Country_IA { get; set; }

            public string SContact { get; set; }
            public string SCompany { get; set; }
            public string SAddress1 { get; set; }
            public string SAddress2 { get; set; }
            public string SAddress3 { get; set; }
            public string SCity { get; set; }
            public string SState { get; set; }
            public string SZip { get; set; }
            public string SCountry { get; set; }
            public string SPhone { get; set; }

            public string Broker { get; set; }
            public decimal? Volume { get; set; }
            public DateTime? SampleDate { get; set; }
            public DateTime? DateRecd { get; set; }
            public DateTime? DateCompleted { get; set; }
            public string Priority { get; set; }
            public string FOB { get; set; }
            public string Status { get; set; }

            public bool Completed { get; set; }
            public bool Active { get; set; }

            public IEnumerable<tblSampleNoteDTO> tblSampleNotes { get; set; }
            public IEnumerable<tblSampleDetailDTO> tblSampleDetails { get; set; }
        }

        public class tblSampleNoteDTO
        {
            public DateTime SamNoteID { get; set; }
            public DateTime? SampleJnlDate { get; set; }
            public string SampleNote { get; set; }
            public int? EmployeeID { get; set; }
        }

        public class tblSampleDetailDTO
        {
            public DateTime SampleDetailID { get; set; }
            public int? ProdID { get; set; }
            public int? Lot { get; set; }
            public decimal? Qty { get; set; }
            public string Desc { get; set; }
            public string SampleMatch { get; set; }

            public tblSampleCustSpecDTO tblSampleCustSpec { get; set; }
            public tblSampleRVCMatchDTO tblSampleRVCMatch { get; set; }
        }

        public class tblSampleCustSpecDTO
        {
            public DateTime CustSpecID { get; set; }

            public int? AstaMin { get; set; }
            public int? AstaMax { get; set; }
            public float? MoistureMin { get; set; }
            public float? MoistureMax { get; set; }
            public int? WaterActivityMin { get; set; }
            public int? WaterActivityMax { get; set; }
            public short? Mesh { get; set; }
            public float? AoverB { get; set; }
            public int? ScovMin { get; set; }
            public int? ScovMax { get; set; }
            public int? ScanMin { get; set; }
            public int? ScanMax { get; set; }
            public int? TPCMin { get; set; }
            public int? TPCMax { get; set; }
            public int? YeastMin { get; set; }
            public int? YeastMax { get; set; }
            public int? MoldMin { get; set; }
            public int? MoldMax { get; set; }
            public int? ColiformsMin { get; set; }
            public int? ColiformsMax { get; set; }
            public int? EColiMin { get; set; }
            public int? EColiMax { get; set; }
            public int? SalMin { get; set; }
            public int? SalMax { get; set; }

            public string Notes { get; set; }
        }

        public class tblSampleRVCMatchDTO
        {
            public DateTime RVCMatchID { get; set; }
            
            public string Gran { get; set; }
            public string AvgAsta { get; set; }
            public string AoverB { get; set; }
            public string AvgScov { get; set; }
            public string H2O { get; set; }
            public string Scan { get; set; }
            public string Yeast { get; set; }
            public string Mold { get; set; }
            public string Coli { get; set; }
            public string TPC { get; set; }
            public string EColi { get; set; }
            public string Sal { get; set; }
            public string InsPrts { get; set; }
            public string RodHrs { get; set; }

            public string Notes { get; set; }
        }

        private class SampleOrderKeyDTO : ISampleOrderKey
        {
            public int SampleOrderKey_Year { get; set; }
            public int SampleOrderKey_Sequence { get; set; }
        }
    }
}