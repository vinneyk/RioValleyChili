using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SampleOrder)]
    public class SyncSampleOrder : SyncCommandBase<ISampleOrderUnitOfWork, SyncSampleOrderParameters>
    {
        public SyncSampleOrder(ISampleOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncSampleOrderParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();

            lock(Lock)
            {
                string message;
                var commitNewContext = false;
                if(parameters.DeleteSampleID != null)
                {
                    DeleteSampleOrder(parameters.DeleteSampleID.Value);
                    message = string.Format(ConsoleOutput.DeletedTblSample, parameters.DeleteSampleID);
                }
                else if(parameters.SampleOrderKey != null)
                {
                    var tblSample = Sync(parameters.SampleOrderKey, out commitNewContext);
                    message = string.Format(ConsoleOutput.SynchedTblSample, tblSample.SampleID);
                }
                else
                {
                    return;
                }

                OldContext.SaveChanges();
                if(commitNewContext)
                {
                    UnitOfWork.Commit();
                }

                Console.WriteLine(message);
            }
        }

        private static readonly object Lock = new object();

        private void DeleteSampleOrder(int sampleID)
        {
            var tblSample = GetTblSampleRecords(sampleID);
            if(tblSample == null)
            {
                return;
            }

            foreach(var detail in tblSample.tblSampleDetails.ToList())
            {
                DeleteTblSampleDetail(detail);
            }

            foreach(var note in tblSample.tblSampleNotes.ToList())
            {
                OldContext.tblSampleNotes.DeleteObject(note);
            }

            OldContext.tblSamples.DeleteObject(tblSample);
        }

        private tblSample Sync(SampleOrderKey sampleOrderKey, out bool commitNewContext)
        {
            commitNewContext = false;
            var sampleOrder = UnitOfWork.SampleOrderRepository.FindByKey(sampleOrderKey,
                s => s.Broker,
                s => s.RequestCustomer.Company,
                s => s.JournalEntries,
                s => s.Items.Select(i => i.Product),
                s => s.Items.Select(i => i.Lot),
                s => s.Items.Select(i => i.Spec),
                s => s.Items.Select(i => i.Match));
            if(sampleOrder == null)
            {
                throw new Exception(string.Format("Could not load SampleOrder[{0}]", sampleOrderKey));
            }

            var tblSample = GetTblSampleRecords(sampleOrder.SampleID);
            if(tblSample == null)
            {
                commitNewContext = true;
                var minSampleId = sampleOrder.Year * 1000;
                var sampleId = OldContext.tblSamples.Select(s => s.SampleID).Where(i => i > minSampleId).DefaultIfEmpty(minSampleId).Max() + 1;
                tblSample = new tblSample
                    {
                        SampleID = sampleId,
                        tblSampleNotes = new EntityCollection<tblSampleNote>(),
                        tblSampleDetails = new EntityCollection<tblSampleDetail>(),
                        s_GUID = Guid.NewGuid()
                    };
                OldContext.tblSamples.AddObject(tblSample);
                sampleOrder.SampleID = tblSample.SampleID;
            }

            tblSample.EmployeeID = sampleOrder.EmployeeId;
            tblSample.EntryDate = sampleOrder.TimeStamp.ConvertUTCToLocal();
            tblSample.Comments = sampleOrder.Comments;
            tblSample.Notes2Print = sampleOrder.PrintNotes;
            tblSample.Volume = (decimal?) sampleOrder.Volume;
            tblSample.SampleDate = sampleOrder.DateDue;
            tblSample.DateRecd = sampleOrder.DateReceived;
            tblSample.DateCompleted = sampleOrder.DateCompleted;
            tblSample.Completed = sampleOrder.DateCompleted.HasValue;
            tblSample.Priority = sampleOrder.ShipmentMethod;
            tblSample.FOB = sampleOrder.FOB;
            tblSample.Status = sampleOrder.Status.ToDisplayString();
            tblSample.Active = sampleOrder.Active;

            tblSample.Company_IA = sampleOrder.RequestCustomer == null ? null : sampleOrder.RequestCustomer.Company.Name;
            tblSample.Broker = sampleOrder.Broker == null ? null : sampleOrder.Broker.Name;

            tblSample.Contact_IA = sampleOrder.Request.Name;
            tblSample.Address1_IA = sampleOrder.Request.Address.AddressLine1;
            tblSample.Address2_IA = sampleOrder.Request.Address.AddressLine2;
            tblSample.Address3_IA = sampleOrder.Request.Address.AddressLine3;
            tblSample.City_IA = sampleOrder.Request.Address.City;
            tblSample.State_IA = sampleOrder.Request.Address.State;
            tblSample.Zip_IA = sampleOrder.Request.Address.PostalCode;
            tblSample.Country_IA = sampleOrder.Request.Address.Country;

            tblSample.SCompany = sampleOrder.ShipToCompany;
            tblSample.SContact = sampleOrder.ShipTo.Name;
            tblSample.SPhone = sampleOrder.ShipTo.Phone;
            tblSample.SAddress1 = sampleOrder.ShipTo.Address.AddressLine1;
            tblSample.SAddress2 = sampleOrder.ShipTo.Address.AddressLine2;
            tblSample.SAddress3 = sampleOrder.ShipTo.Address.AddressLine3;
            tblSample.SCity = sampleOrder.ShipTo.Address.City;
            tblSample.SState = sampleOrder.ShipTo.Address.State;
            tblSample.SZip = sampleOrder.ShipTo.Address.PostalCode;
            tblSample.SCountry = sampleOrder.ShipTo.Address.Country;

            SetTblSampleNotes(tblSample, sampleOrder, ref commitNewContext);
            SetTblSampleDetails(tblSample, sampleOrder, ref commitNewContext);

            return tblSample;
        }

        private void SetTblSampleNotes(tblSample tblSample, SampleOrder sampleOrder, ref bool commitNewContext)
        {
            var existingNotes = tblSample.tblSampleNotes.ToDictionary(n => n.SamNoteID);
            var samNoteID = OldContext.tblSampleNotes.Select(n => n.SamNoteID).DefaultIfEmpty(DateTime.Now.RoundMillisecondsForSQL()).Max();
            foreach(var entry in sampleOrder.JournalEntries)
            {
                tblSampleNote note = null;
                if(entry.SamNoteID != null && existingNotes.TryGetValue(entry.SamNoteID.Value, out note))
                {
                    existingNotes.Remove(note.SamNoteID);
                }

                if(note == null)
                {
                    commitNewContext = true;
                    samNoteID = samNoteID.AddSeconds(1);
                    note = new tblSampleNote
                        {
                            SamNoteID = samNoteID,
                            SampleID = tblSample.SampleID,
                            s_GUID = Guid.NewGuid()
                        };
                    OldContext.tblSampleNotes.AddObject(note);
                    entry.SamNoteID = note.SamNoteID;
                }

                note.EmployeeID = entry.EmployeeId;
                note.SampleJnlDate = entry.Date;
                note.SampleNote = entry.Text;
            }

            foreach(var tblSampleNote in existingNotes)
            {
                OldContext.tblSampleNotes.DeleteObject(tblSampleNote.Value);
            }
        }

        private void SetTblSampleDetails(tblSample tblSample, SampleOrder sampleOrder, ref bool commitNewContext)
        {
            var existingDetails = tblSample.tblSampleDetails.ToDictionary(n => n.SampleDetailID);
            var sampleDetailId = OldContext.tblSampleDetails.Select(n => n.SampleDetailID).DefaultIfEmpty(DateTime.Now.RoundMillisecondsForSQL()).Max();
            var custSpecId = OldContext.tblSampleCustSpecs.Select(n => n.CustSpecID).DefaultIfEmpty(DateTime.Now.RoundMillisecondsForSQL()).Max();
            var rvcMatchId = OldContext.tblSampleRVCMatches.Select(n => n.RVCMatchID).DefaultIfEmpty(DateTime.Now.RoundMillisecondsForSQL()).Max();
            foreach(var item in sampleOrder.Items)
            {
                tblSampleDetail detail = null;
                if(item.SampleDetailID != null && existingDetails.TryGetValue(item.SampleDetailID.Value, out detail))
                {
                    existingDetails.Remove(detail.SampleDetailID);
                }

                if(detail == null)
                {
                    commitNewContext = true;
                    sampleDetailId = sampleDetailId.AddSeconds(1);
                    detail = new tblSampleDetail
                        {
                            SampleDetailID = sampleDetailId,
                            SampleID = tblSample.SampleID,
                            s_GUID = Guid.NewGuid()
                        };
                    OldContext.tblSampleDetails.AddObject(detail);
                    item.SampleDetailID = detail.SampleDetailID;
                }

                detail.ProdID = item.Product == null ? (int?) null : int.Parse(item.Product.ProductCode);
                detail.Lot = item.Lot == null ? (int?) null : LotNumberParser.BuildLotNumber(item.Lot);
                detail.Qty = item.Quantity;
                detail.Desc = item.Description;
                detail.SampleMatch = item.CustomerProductName;

                if(item.Spec == null)
                {
                    DeleteTblSampleCustSpecs(detail);
                }
                else
                {
                    var spec = detail.tblSampleCustSpecs.FirstOrDefault(s => s.CustSpecID == item.Spec.CustSpecID);
                    if(spec == null)
                    {
                        commitNewContext = true;
                        custSpecId = custSpecId.AddSeconds(1);
                        spec = new tblSampleCustSpec
                            {
                                CustSpecID = custSpecId,
                                SampleDetailID = detail.SampleDetailID
                            };
                        OldContext.tblSampleCustSpecs.AddObject(spec);
                        item.Spec.CustSpecID = spec.CustSpecID;
                    }

                    SetSpec(spec, item.Spec);
                }

                if(item.Match == null)
                {
                    DeleteTblSampleRVCMatches(detail);
                }
                else
                {
                    var match = detail.tblSampleRVCMatches.FirstOrDefault(s => s.RVCMatchID == item.Match.RVCMatchID);
                    if(match == null)
                    {
                        commitNewContext = true;
                        rvcMatchId = rvcMatchId.AddSeconds(1);
                        match = new tblSampleRVCMatch
                            {
                                RVCMatchID = rvcMatchId,
                                SampleDetailID = detail.SampleDetailID
                            };
                        OldContext.tblSampleRVCMatches.AddObject(match);
                        item.Match.RVCMatchID = match.RVCMatchID;
                    }

                    SetMatch(match, item.Match);
                }
            }

            foreach(var detail in existingDetails)
            {
                DeleteTblSampleDetail(detail.Value);
            }
        }

        private static void SetSpec(tblSampleCustSpec spec, SampleOrderItemSpec itemSpec)
        {
            spec.Notes = itemSpec.Notes;

            spec.AstaMin = (int?) itemSpec.AstaMin;
            spec.AstaMax = (int?) itemSpec.AstaMax;
            spec.MoistureMin = (int?) itemSpec.MoistureMin;
            spec.MoistureMax = (int?) itemSpec.MoistureMax;
            spec.WaterActivityMin = (int?) itemSpec.WaterActivityMin;
            spec.WaterActivityMax = (int?) itemSpec.WaterActivityMax;
            spec.Mesh = (short?) itemSpec.Mesh;
            spec.AoverB = (int?) itemSpec.AoverB;
            spec.ScovMin = (int?) itemSpec.ScovMin;
            spec.ScovMax = (int?) itemSpec.ScovMax;
            spec.ScanMin = (int?) itemSpec.ScanMin;
            spec.ScanMax = (int?) itemSpec.ScanMax;
            spec.TPCMin = (int?) itemSpec.TPCMin;
            spec.TPCMax = (int?) itemSpec.TPCMax;
            spec.YeastMin = (int?) itemSpec.YeastMin;
            spec.YeastMax = (int?) itemSpec.YeastMax;
            spec.MoldMin = (int?) itemSpec.MoldMin;
            spec.MoldMax = (int?) itemSpec.MoldMax;
            spec.ColiformsMin = (int?) itemSpec.ColiformsMin;
            spec.ColiformsMax = (int?) itemSpec.ColiformsMax;
            spec.EColiMin = (int?) itemSpec.EColiMin;
            spec.EColiMax = (int?) itemSpec.EColiMax;
            spec.SalMin = (int?) itemSpec.SalMin;
            spec.SalMax = (int?) itemSpec.SalMax;
        }

        private static void SetMatch(tblSampleRVCMatch match, SampleOrderItemMatch itemMatch)
        {
            match.Notes = itemMatch.Notes;

            match.Gran = itemMatch.Gran;
            match.AvgAsta = itemMatch.AvgAsta;
            match.AoverB = itemMatch.AoverB;
            match.AvgScov = itemMatch.AvgScov;
            match.H2O = itemMatch.H2O;
            match.Scan = itemMatch.Scan;
            match.Yeast = itemMatch.Yeast;
            match.Mold = itemMatch.Mold;
            match.Coli = itemMatch.Coli;
            match.TPC = itemMatch.TPC;
            match.EColi = itemMatch.EColi;
            match.Sal = itemMatch.Sal;
            match.InsPrts = itemMatch.InsPrts;
            match.RodHrs = itemMatch.RodHrs;
        }

        private tblSample GetTblSampleRecords(int? sampleId)
        {
            if(sampleId == null)
            {
                return null;
            }

            return OldContext.tblSamples
                .Include
                (
                    s => s.tblSampleNotes,
                    s => s.tblSampleDetails.Select(d => d.tblSampleCustSpecs),
                    s => s.tblSampleDetails.Select(d => d.tblSampleCusMatches),
                    s => s.tblSampleDetails.Select(d => d.tblSampleRVCMatches),
                    s => s.tblSampleDetails.Select(d => d.tblSampleRVCIngredients)
                )
                .FirstOrDefault(s => s.SampleID == sampleId);
        }

        private void DeleteTblSampleDetail(tblSampleDetail detail)
        {
            DeleteTblSampleCustSpecs(detail);
            DeleteTblSampleRVCMatches(detail);

            foreach(var match in detail.tblSampleCusMatches.ToList())
            {
                OldContext.tblSampleCusMatches.DeleteObject(match);
            }

            foreach(var ingredient in detail.tblSampleRVCIngredients.ToList())
            {
                OldContext.tblSampleRVCIngredients.DeleteObject(ingredient);
            }

            OldContext.tblSampleDetails.DeleteObject(detail);
        }

        private void DeleteTblSampleCustSpecs(tblSampleDetail detail)
        {
            foreach(var spec in detail.tblSampleCustSpecs.ToList())
            {
                OldContext.tblSampleCustSpecs.DeleteObject(spec);
            }
        }

        private void DeleteTblSampleRVCMatches(tblSampleDetail detail)
        {
            foreach(var match in detail.tblSampleRVCMatches.ToList())
            {
                OldContext.tblSampleRVCMatches.DeleteObject(match);
            }
        }
    }
}