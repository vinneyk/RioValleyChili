using System;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseOrderService
{
    public class UpdateIntraWarehouseOrderParameters : IUpdateIntraWarehouseOrderParameters
    {
        public string OperatorName { get; set; }
        public DateTime MovementDate { get; set; }
        public string IntraWarehouseOrderKey { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}