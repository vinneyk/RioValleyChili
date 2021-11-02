using System;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    public enum NewContextMethod
    {
        SyncChileMaterialsReceived,
        SyncMillAndWetdown,
        CreateInventoryAdjustment,
        SyncProductionBatchResults,
        SyncLot,
        CreatePackSchedule,
        UpdatePackSchedule,
        DeletePackSchedule,
        CreateProductionBatch,
        UpdateProductionBatch,
        DeleteProductionBatch,
        Notebook,
        PickInventoryForProductionBatch,    
        SyncCustomerSpecs,
        ReceiveInventory,
        SyncContract,
        SyncContractsStatus,
        DeleteContract,
        SyncIntraWarehouseOrder,
        SetShipmentInformation,
        Post,
        SyncInventoryShipmentOrder,
        CompleteExpiredContracts,
        SyncFacility,
        SyncLocations,
        ReceiveTreatmentOrder,
        SyncSalesOrder,
        DeleteTreatmentOrder,
        DeleteSalesOrder,
        SyncPostInvoice,
        SyncCompany,
        Product,
        [Obsolete("Use Product instead. -RI 2016-09-05")]
        ChileProductIngredients,
        SampleOrder,
        DeleteCustomerNote,
        DeleteCompanyContact,
        ProductionSchedule,
        DeleteLot,
        SalesQuote
    }
}