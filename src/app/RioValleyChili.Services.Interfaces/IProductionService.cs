using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductionService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Interfaces.ServiceCompositions;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IProductionService : IPickInventoryServiceComponent
    {
        IResult<string> CreatePackSchedule(ICreatePackScheduleParameters parameters);
        IResult<string> UpdatePackSchedule(IUpdatePackScheduleParameters parameters);
        IResult<string> RemovePackSchedule(IRemovePackScheduleParameters parameters);
        IResult<IQueryable<IPackScheduleSummaryReturn>> GetPackSchedules();
        IResult<IPackScheduleDetailReturn> GetPackSchedule(string packScheduleKey);
        IResult<ICreateProductionBatchReturn> CreateProductionBatch(ICreateProductionBatchParameters parameters);
        IResult<string> UpdateProductionBatch(IUpdateProductionBatchParameters parameters);
        IResult<string> RemoveProductionBatch(string productionBatchKey);
        IResult SetPickedInventoryForProductionBatch(string productionBatchKey, ISetPickedInventoryParameters parameters);
        IResult<IProductionBatchDetailReturn> GetProductionBatch(string productionBatchKey);
        IResult<IPickableInventoryReturn> GetInventoryItemsToPickBatch(FilterInventoryForBatchParameters parameters = null);
        IResult<IEnumerable<string>> GetProductionBatchInstructions();
        IResult<IProductionPacketReturn> GetProductionPacketForBatch(string productionBatchKey);
        IResult<IProductionPacketReturn> GetProductionPacketForPackSchedule(string packScheduleKey);
        IResult<IPackSchedulePickSheetReturn> GetPackSchedulePickSheet(string packScheduleKey);

        IResult<string> CreateProductionSchedule(ICreateProductionScheduleParameters parameters);
        IResult UpdateProductionSchedule(IUpdateProductionScheduleParameters parameters);
        IResult DeleteProductionSchedule(string productionScheduleKey);
        IResult<IQueryable<IProductionScheduleSummaryReturn>> GetProductionSchedules(FilterProductionScheduleParameters parameters = null);
        IResult<IProductionScheduleDetailReturn> GetProductionSchedule(string productionScheduleKey);
        IResult<IEnumerable<IProductionScheduleReportReturn>> GetProductionScheduleReport(DateTime productionDate, string productionLocationKey);

        IResult<IQueryable<IWorkTypeReturn>> GetWorkTypes();
        IResult<IQueryable<ILocationReturn>> GetProductionLines();
    }
}