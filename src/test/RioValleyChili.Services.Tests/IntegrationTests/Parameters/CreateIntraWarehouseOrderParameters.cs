using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateIntraWarehouseOrderParameters : ICreateIntraWarehouseOrderParameters
    {
        public string IntraWarehouseOrderKey { get; set; }
        public string UserToken { get; set; }
        public decimal TrackingSheetNumber { get; set; }
        public string OperatorName { get; set; }
        public DateTime MovementDate { get; set; }
        public IEnumerable<IIntraWarehouseOrderPickedItemParameters> PickedItems { get; set; }
    }
}