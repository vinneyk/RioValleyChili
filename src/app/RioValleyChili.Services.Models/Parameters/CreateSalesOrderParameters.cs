using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateSalesOrderParameters : ICreateSalesOrderParameters
    {
        public string UserToken { get; set; }
        public string BrokerKey { get; set; }
        public string CustomerKey { get; set; }
        public string FacilitySourceKey { get; set; }
        public bool PreShipmentSampleRequired { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNotes { get; set; }
        public float FreightCharge { get; set; }
        public bool IsMiscellaneous { get; set; }

        public SetOrderHeaderParameters HeaderParameters { get; set; }
        public SetInventoryShipmentInformationParameters SetShipmentInformation { get; set; }
        public IEnumerable<SalesOrderItemParameters> PickOrderItems { get; set; }

        ISetOrderHeaderParameters ISetSalesOrderParameters.HeaderParameters { get { return HeaderParameters; } }
        ISetShipmentInformation ISetSalesOrderParameters.SetShipmentInformation { get { return SetShipmentInformation; } }
        IEnumerable<ISalesOrderItem> ISetSalesOrderParameters.PickOrderItems { get { return PickOrderItems; } }
    }
}