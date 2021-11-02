using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetOrderHeaderParameters : ISetOrderHeaderParameters
    {
        public DateTime? DateOrderReceived { get; set; }

        [StringLength(Constants.StringLengths.PurchaseOrderNumber)]
        public string CustomerPurchaseOrderNumber { get; set; }
        [StringLength(Constants.StringLengths.PaymentTerms)]
        public string PaymentTerms { get; set; }
        [StringLength(Constants.StringLengths.ContactName)]
        public string OrderRequestedBy { get; set; }
        [StringLength(Constants.StringLengths.ContactName)]
        public string OrderTakenBy { get; set; }
    }
}