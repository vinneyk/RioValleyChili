using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateSalesOrderParameters : IUpdateSalesOrderParameters
    {
        public string SalesOrderKey { get; set; }
        public bool CreditMemo { get; set; }
        public string UserToken { get; set; }
        public string BrokerKey { get; set; }
        public string FacilitySourceKey { get; set; }
        public bool PreShipmentSampleRequired { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNotes { get; set; }
        public float FreightCharge { get; set; }
        public IEnumerable<SalesOrderItemParameters> OrderItems { get; set; }
        public ISetOrderHeaderParameters HeaderParameters { get; set; }
        public ISetShipmentInformation SetShipmentInformation { get; set; }

        IEnumerable<ISalesOrderItem> ISetSalesOrderParameters.PickOrderItems { get { return OrderItems; } }
    }
}