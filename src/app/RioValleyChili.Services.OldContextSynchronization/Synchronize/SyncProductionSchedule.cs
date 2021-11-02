using System;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.ProductionSchedule)]
    public class SyncProductionSchedule : SyncCommandBase<IProductionUnitOfWork, ProductionScheduleKey>
    {
        public SyncProductionSchedule(IProductionUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<ProductionScheduleKey> getInput)
        {
            var productionScheduleKey = getInput();

            var productionSchedule = UnitOfWork.ProductionScheduleRepository.FindByKey(productionScheduleKey,
                p => p.ScheduledItems.Select(i => i.PackSchedule.ProductionBatches));

            var location = UnitOfWork.LocationRepository.FindByKey(productionScheduleKey.ToLocationKey());
            string street;
            int row;
            LocationDescriptionHelper.GetStreetRow(location.Description, out street, out row);

            var oldProductionSchedule = OldContext.tblProductionSchedules
                .Include(p => p.tblProductionScheduleGroups.Select(g => g.tblProductionScheduleItems))
                .FirstOrDefault(p => p.ProductionDate == productionScheduleKey.ProductionScheduleKey_ProductionDate && (int)p.LineNumber == row);
            if(productionSchedule == null)
            {
                if(oldProductionSchedule != null)
                {
                    DeleteSchedule(oldProductionSchedule);
                }
            }
            else
            {
                SetSchedule(oldProductionSchedule, productionSchedule, row);
            }

            OldContext.SaveChanges();
            
            Console.WriteLine(ConsoleOutput.SyncProductionSchedule, productionScheduleKey.ProductionScheduleKey_ProductionDate.ToString("yyyyMMdd"), row);
        }

        private void SetSchedule(tblProductionSchedule oldProductionSchedule, ProductionSchedule productionSchedule, int lineNumber)
        {
            if(oldProductionSchedule == null)
            {
                OldContext.tblProductionSchedules.AddObject(oldProductionSchedule = new tblProductionSchedule
                    {
                        ProductionDate = productionSchedule.ProductionDate,
                        LineNumber = lineNumber,
                        tblProductionScheduleGroups = new EntityCollection<tblProductionScheduleGroup>()
                    });
            }

            oldProductionSchedule.DateCreated = productionSchedule.TimeStamp.ConvertUTCToLocal();
            oldProductionSchedule.CreatedBy = productionSchedule.EmployeeId;

            var oldGroups = oldProductionSchedule.tblProductionScheduleGroups.ToDictionary(g => g.PSNum);
            foreach(var item in productionSchedule.ScheduledItems)
            {
                tblProductionScheduleGroup oldGroup;
                if(oldGroups.TryGetValue(item.PackSchedule.PSNum.Value, out oldGroup))
                {
                    oldGroups.Remove(item.PackSchedule.PSNum.Value);
                }
                else
                {
                    OldContext.tblProductionScheduleGroups.AddObject(oldGroup = new tblProductionScheduleGroup
                        {
                            ProductionDate = oldProductionSchedule.ProductionDate,
                            LineNumber = oldProductionSchedule.LineNumber,
                            PSNum = item.PackSchedule.PSNum.Value,
                            tblProductionScheduleItems = new EntityCollection<tblProductionScheduleItem>()
                        });
                }

                oldGroup.Index = item.Index;
                oldGroup.Instructions = item.PackSchedule.SummaryOfWork;
                oldGroup.FlushBefore = item.FlushBefore;
                oldGroup.FlushBeforeInstructions = item.FlushBeforeInstructions;
                oldGroup.FlushAfter = item.FlushAfter;
                oldGroup.FlushAfterInstructions = item.FlushAfterInstructions;

                var oldItems = oldGroup.tblProductionScheduleItems.ToDictionary(i => i.LotNumber);
                foreach(var batch in item.PackSchedule.ProductionBatches
                                         .Where(b => !b.ProductionHasBeenCompleted)
                                         .OrderBy(b => b.LotTypeId)
                                         .ThenBy(b => b.LotDateCreated)
                                         .ThenBy(b => b.LotDateSequence))
                {
                    var lotNumber = LotNumberParser.BuildLotNumber(batch);

                    tblProductionScheduleItem oldItem;
                    if(oldItems.TryGetValue(lotNumber, out oldItem))
                    {
                        oldItems.Remove(lotNumber);
                    }
                    else
                    {
                        OldContext.tblProductionScheduleItems.AddObject(oldItem = new tblProductionScheduleItem
                            {
                                ProductionDate = oldGroup.ProductionDate,
                                LineNumber = oldGroup.LineNumber,
                                PSNum = oldGroup.PSNum,
                                LotNumber = lotNumber
                            });
                    }
                    oldItem.BatchNumber = batch.LotDateSequence;
                }

                foreach(var oldItem in oldItems.Values)
                {
                    OldContext.tblProductionScheduleItems.DeleteObject(oldItem);
                }
            }

            foreach(var oldGroup in oldGroups.Values)
            {
                foreach(var oldItem in oldGroup.tblProductionScheduleItems)
                {
                    OldContext.tblProductionScheduleItems.DeleteObject(oldItem);
                }
                OldContext.tblProductionScheduleGroups.DeleteObject(oldGroup);
            }
        }

        private void DeleteSchedule(tblProductionSchedule oldProductionSchedule)
        {
            foreach(var oldGroup in oldProductionSchedule.tblProductionScheduleGroups.ToList())
            {
                foreach(var oldItem in oldGroup.tblProductionScheduleItems.ToList())
                {
                    OldContext.tblProductionScheduleItems.DeleteObject(oldItem);
                }
                OldContext.tblProductionScheduleGroups.DeleteObject(oldGroup);
            }

            OldContext.tblProductionSchedules.DeleteObject(oldProductionSchedule);
        }
    }
}