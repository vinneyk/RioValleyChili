using System;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Mvc.Utilities
{
    public class InventoryServiceFactory
    {
        private readonly IWarehouseOrderService _warehouseOrderService;
        private readonly IIntraWarehouseOrderService _intraWarehouseOrderService;
        private readonly ITreatmentOrderService _treatmentOrderService;
        private readonly IProductionService _productionService;

        public InventoryServiceFactory(
            IWarehouseOrderService warehouseOrderService,
            IIntraWarehouseOrderService intraWarehouseOrderService,
            ITreatmentOrderService treatmentOrderService,
            IProductionService productionService)
        {
            if (warehouseOrderService == null) { throw new ArgumentNullException("warehouseOrderService"); }
            _warehouseOrderService = warehouseOrderService;

            if (intraWarehouseOrderService == null) { throw new ArgumentNullException("intraWarehouseOrderService"); }
            _intraWarehouseOrderService = intraWarehouseOrderService;

            if (treatmentOrderService == null) { throw new ArgumentNullException("treatmentOrderService"); }
            _treatmentOrderService = treatmentOrderService;

            if (productionService == null) { throw new ArgumentNullException("productionService"); }
            _productionService = productionService;
        }

        public IWarehouseOrderService TransWarehouseOrderService { get { return _warehouseOrderService; } }
        public IIntraWarehouseOrderService IntraWarehouseOrderService { get { return _intraWarehouseOrderService; } }
        public ITreatmentOrderService TreatmentOrderService { get { return _treatmentOrderService; } }
        public IProductionService ProductionService { get { return _productionService; } }
    }
}