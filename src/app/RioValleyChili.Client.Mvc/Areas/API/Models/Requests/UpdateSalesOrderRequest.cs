using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class UpdateSalesOrderRequest
    {
        public string SalesOrderKey { get; set; }
        public string BrokerKey { get; set; }
        public string FacilitySourceKey { get; set; }
        public bool PreShipmentSampleRequired { get; set; }
        public bool CreditMemo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNotes { get; set; }
        public float FreightCharge { get; set; }

        public SetOrderHeaderRequestParameter HeaderParameters { get; set; }
        public SetShipmentInformationRequestParameter SetShipmentInformation { get; set; }

        public IEnumerable<SalesOrderItem> PickOrderItems { get; set; }
    }
}