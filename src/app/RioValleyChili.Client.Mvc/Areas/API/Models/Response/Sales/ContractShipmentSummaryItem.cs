using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.WebApi;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Sales
{
    public class ContractShipmentSummaryItem : ILinkedResource<ContractShipmentSummaryItem>
    {
        public string ContractKey { get; set; }
        public string ContractNumber { get; set; }
        public string ContractStatus { get; set; }

        public string ProductName { get; set; }
        public string CustomerProductCode { get; set; }
        public string PackagingName { get; set; }
        public string Treatment { get; set; }

        public string ContractBeginDate { get; set; }
        public string ContractEndDate { get; set; }

        public double BasePrice { get; set; }
        public double ContractItemValue { get; set; }
        public int ContractItemPounds { get; set; }

        public int TotalPoundsShippedForContractItem { get; set; }
        public int TotalPoundsPendingForContractItem { get; set; }
        public int TotalPoundsRemainingForContractItem { get; set; }

        #region ILinkedResource<T> implementation

        public ResourceLinkCollection Links { get; set; }
        public string HRef { get { return Links.SelfHRef; } }
        public ContractShipmentSummaryItem Data { get { return this; } }

        #endregion
    }

    public class ContractShipmentSummaries : LinkedResoureCollection<ContractShipmentSummaryItem>
    {
        public ContractShipmentSummaries()  { }
        public ContractShipmentSummaries(IEnumerable<ContractShipmentSummaryItem> data) : base(data) { }
    }
}