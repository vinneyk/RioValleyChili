using System;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateIntraWarehouseOrderParameters : IUpdateIntraWarehouseOrderParameters
    {
        public string UserToken { get; set; }
        public string OperatorName { get;  set; }
        public DateTime MovementDate { get;  set; }
        public string IntraWarehouseOrderKey { get;  set; }
    }
}