using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Conductors.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal abstract class ProductionScheduleConductorBase
    {
        protected IProductionUnitOfWork ProductionUnitOfWork;

        protected ProductionScheduleConductorBase(IProductionUnitOfWork productionUnitOfWork)
        {
            if(productionUnitOfWork == null) { throw new ArgumentNullException("productionUnitOfWork"); }
            ProductionUnitOfWork = productionUnitOfWork;
        }

        protected IResult<ProductionSchedule> Set(ProductionSchedule productionSchedule, IEmployeeKey user, DateTime timestamp, IEnumerable<SetProductionScheduleItemParameters> items)
        {
            productionSchedule.EmployeeId = user.EmployeeKey_Id;
            productionSchedule.TimeStamp = timestamp;

            var itemsToRemove = productionSchedule.ScheduledItems.ToDictionary(i => i.Index);

            foreach(var item in items)
            {
                ProductionScheduleItem scheduledItem;
                if(itemsToRemove.TryGetValue(item.Index, out scheduledItem))
                {
                    itemsToRemove.Remove(item.Index);
                }
                else
                {
                    scheduledItem = ProductionUnitOfWork.ProductionScheduleItemRepository.Add(new ProductionScheduleItem
                        {
                            ProductionDate = productionSchedule.ProductionDate,
                            ProductionLineLocationId = productionSchedule.ProductionLineLocationId,
                            Index = item.Index
                        });
                }

                scheduledItem.FlushBefore = item.FlushBefore;
                scheduledItem.FlushBeforeInstructions = item.FlushBeforeInstructions;
                scheduledItem.FlushAfter = item.FlushAfter;
                scheduledItem.FlushAfterInstructions = item.FlushAfterInstructions;
                scheduledItem.PackScheduleDateCreated = item.PackScheduleKey.PackScheduleKey_DateCreated;
                scheduledItem.PackScheduleSequence = item.PackScheduleKey.PackScheduleKey_DateSequence;
            }

            foreach(var item in itemsToRemove)
            {
                ProductionUnitOfWork.ProductionScheduleItemRepository.Remove(item.Value);
            }

            return new SuccessResult<ProductionSchedule>(productionSchedule);
        }
    }

    internal class CreateProductionScheduleConductor : ProductionScheduleConductorBase
    {
        internal CreateProductionScheduleConductor(IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        internal IResult<ProductionSchedule> Create(DateTime timestamp, CreateProductionScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(ProductionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ProductionSchedule>();
            }

            var location = ProductionUnitOfWork.LocationRepository.FindByKey(parameters.ProductionLocationKey);
            if(location == null || location.LocationType != LocationType.ProductionLine)
            {
                return new InvalidResult<ProductionSchedule>(null, string.Format(UserMessages.ProductionLocationNotFound, parameters.ProductionLocationKey));
            }

            if(ProductionUnitOfWork.ProductionScheduleRepository.Filter(p => p.ProductionDate == parameters.Parameters.ProductionDate && p.ProductionLineLocationId == location.Id).Any())
            {
                return new InvalidResult<ProductionSchedule>(null, string.Format(UserMessages.ProductionScheduleAlreadyExists, parameters.Parameters.ProductionDate.ToShortDateString(), location.Description));
            }

            var packSchedules = ProductionUnitOfWork.PackScheduleRepository
                .Filter(p => p.ProductionLineLocationId == location.Id && p.ProductionBatches.Any(b => !b.ProductionHasBeenCompleted))
                .Select(p => new
                    {
                        packSchedule = p,
                        lastScheduled = p.ScheduledItems
                            .OrderByDescending(i => i.ProductionSchedule.TimeStamp)
                            .FirstOrDefault()
                    })
                .ToList();

            var index = 0;
            var scheduledItems = new List<SetProductionScheduleItemParameters>();
            foreach(var packSchedule in packSchedules
                .OrderByDescending(p => p.lastScheduled != null)
                .ThenBy(p => p.lastScheduled != null ? p.lastScheduled.Index : int.MaxValue)
                .ThenByDescending(p => p.lastScheduled != null ? p.lastScheduled.ProductionDate : DateTime.MinValue)
                .ThenBy(p => p.packSchedule.ScheduledProductionDate)
                .ThenBy(p => p.packSchedule.DateCreated)
                .ThenBy(p => p.packSchedule.SequentialNumber))
            {
                var scheduledItem = new SetProductionScheduleItemParameters
                    {
                        PackScheduleKey = packSchedule.packSchedule.ToPackScheduleKey(),
                        Index = index++
                    };

                if(packSchedule.lastScheduled != null)
                {
                    scheduledItem.FlushBefore = packSchedule.lastScheduled.FlushBefore;
                    scheduledItem.FlushBeforeInstructions = packSchedule.lastScheduled.FlushBeforeInstructions;
                    scheduledItem.FlushAfter = packSchedule.lastScheduled.FlushAfter;
                    scheduledItem.FlushAfterInstructions = packSchedule.lastScheduled.FlushAfterInstructions;
                }

                scheduledItems.Add(scheduledItem);
            }

            var productionSchedule = ProductionUnitOfWork.ProductionScheduleRepository.Add(new ProductionSchedule
                {
                    ProductionDate = parameters.Parameters.ProductionDate,
                    ProductionLineLocationId = location.Id,
                    ProductionLineLocation = location,
                    ScheduledItems = new List<ProductionScheduleItem>()
                });

            return Set(productionSchedule, employeeResult.ResultingObject, timestamp, scheduledItems);
        }
    }

    internal class UpdateProductionScheduleConductor : ProductionScheduleConductorBase
    {
        internal UpdateProductionScheduleConductor(IProductionUnitOfWork productionUnitOfWork) : base(productionUnitOfWork) { }

        internal IResult<ProductionSchedule> Update(DateTime timestamp, UpdateProductionScheduleParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(ProductionUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ProductionSchedule>();
            }

            var productionSchedule = ProductionUnitOfWork.ProductionScheduleRepository.FindByKey(parameters.ProductionScheduleKey,
                p => p.ProductionLineLocation,
                p => p.ScheduledItems);
            if(productionSchedule == null)
            {
                return new InvalidResult<ProductionSchedule>(null, string.Format(UserMessages.ProductionScheduleNotFound, parameters.ProductionScheduleKey));
            }

            var items = parameters.ScheduledItems.Select(i => new SetProductionScheduleItemParameters
                {
                    Index = i.Parameters.Index,
                    FlushBefore = i.Parameters.FlushBefore,
                    FlushBeforeInstructions = i.Parameters.FlushBeforeInstructions,
                    FlushAfter = i.Parameters.FlushAfter,
                    FlushAfterInstructions = i.Parameters.FlushAfterInstructions,
                    PackScheduleKey = i.PackScheduleKey
                }).ToList();

            return Set(productionSchedule, employeeResult.ResultingObject, timestamp, items);
        }
    }
}