using System;
using System.Collections.Generic;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Requests
{
    public class CreateSalesOrderRequest
    {
        private float? _freightCharge;
        public string CustomerKey { get; set; }
        public string BrokerKey { get; set; }
        public string FacilitySourceKey { get; set; }
        public bool PreShipmentSampleRequired { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNotes { get; set; }

        public float? FreightCharge
        {
            get { return _freightCharge ?? 0; }
            set { _freightCharge = value; }
        }

        public bool IsMiscellaneous { get; set; }

        public SetOrderHeaderRequestParameter HeaderParameters { get; set; }
        public SetShipmentInformationRequestParameter SetShipmentInformation { get; set; }
        public IEnumerable<SalesOrderItem> PickOrderItems { get; set; }
    }
}