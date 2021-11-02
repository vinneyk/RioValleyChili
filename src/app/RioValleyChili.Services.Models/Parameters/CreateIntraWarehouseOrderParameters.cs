using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.IntraWarehouseOrderService;
using RioValleyChili.Services.Interfaces.Parameters.PickInventoryServiceComponent;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateIntraWarehouseOrderParameters : ICreateIntraWarehouseOrderParameters
    {
        public decimal TrackingSheetNumber { get; set; }
        public string OperatorName { get; set; }
        public DateTime MovementDate { get; set; }

        IEnumerable<IIntraWarehouseOrderPickedItemParameters> ICreateIntraWarehouseOrderParameters.PickedItems { get { return PickedItems; } }
        public IEnumerable<IntraWarehouseOrderPickedItemParameters> PickedItems { get; set; }
        
        string IUserIdentifiable.UserToken { get; set; }
    }
}