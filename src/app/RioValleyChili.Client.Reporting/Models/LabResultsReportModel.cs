using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Client.Reporting.Models
{
    public class LabResultsReportModel
    {
        public string LotKey { get; set; }
        public LotHoldType? HoldType { get; set; }
        public string HoldDescription { get; set; }
        public LotQualityStatus QualityStatus { get; set; }
        public LotProductionStatus ProductionStatus { get; set; }
        public string PackScheduleKey { get; set; }
        public int? PSNum { get; set; }
        public string WorkType { get; set; }
        public string ProductionLineDescription { get; set; }
        public IProductionBatchTargetParameters TargetParameters { get; set; }
        public DateTime ProductionEndDate { get; set; }
        public string ProductionShift { get; set; }

        public bool LoBac { get; set; }
        public string ChileProductKey { get; set; }
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
        public bool ValidToPick { get; set; }

        public IEnumerable<string> UnresolvedDefects { get; set; }
        public IEnumerable<LotCustomerAllowanceModel> CustomerAllowances { get; set; }
        public string Notes { get; set; }
        public IEnumerable<DehydratedInputModel> DehydratedInputs { get; set; }
        
        public string ProductKey { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public ChileProductSpec ProductSpec { get; set; }
        public LabResults LotAttributeValues { get; set; }

        public string FormattedProductionLineDescription
        {
            get { return LocationDescriptionHelper.FormatLocationDescription(ProductionLineDescription); }
        }

        public bool HasHold
        {
            get { return HoldType.HasValue; }
        }

        public string NotesString
        {
            get { return WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(Notes) ? "" : Notes); }
        }

        public string Allowances
        {
            get { return string.Join(", ", (CustomerAllowances.Select(a => a.CustomerName)).Distinct()); }
        }

        public string QualityStatusString
        {
            get { return QualityStatus == LotQualityStatus.Released ? ValidToPick ? "Shippable" : "Blending" : QualityStatus.ToString(); }
        }
    }

    public class DehydratedInputModel : IDehydratedInputReturn
    {
        public string Variety { get; set; }
        public string ToteKey { get; set; }
        public string GrowerCode { get; set; }
        public string LotKey { get; set; }
        public string DehydratorName { get; set; }
    }

    public class ProductAttributeSpec
    {
        public string AttributeName { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
    }
    
    public class ChileProductSpec
    {
        public ProductAttributeSpec AB { get; set; }
        public ProductAttributeSpec AIA { get; set; }
        public ProductAttributeSpec Ash { get; set; }
        public ProductAttributeSpec Asta { get; set; }
        public ProductAttributeSpec AToxin { get; set; }
        public ProductAttributeSpec BI { get; set; }
        public ProductAttributeSpec ColiF { get; set; }
        public ProductAttributeSpec EColi { get; set; }
        public ProductAttributeSpec Ethox { get; set; }
        public ProductAttributeSpec Gluten { get; set; }
        public ProductAttributeSpec Gran { get; set; }
        public ProductAttributeSpec H2O { get; set; }
        public ProductAttributeSpec InsP { get; set; }
        public ProductAttributeSpec Lead { get; set; }
        public ProductAttributeSpec Mold { get; set; }
        public ProductAttributeSpec RodHrs { get; set; }
        public ProductAttributeSpec Sal { get; set; }
        public ProductAttributeSpec Scan { get; set; }
        public ProductAttributeSpec Scov { get; set; }
        public ProductAttributeSpec TPC { get; set; }
        public ProductAttributeSpec Yeast { get; set; }
    }

    public class LabResults
    {
        public LotAttributeValues AB { get; set; }
        public LotAttributeValues AIA { get; set; }
        public LotAttributeValues Ash { get; set; }
        public LotAttributeValues Asta { get; set; }
        public LotAttributeValues AToxin { get; set; }
        public LotAttributeValues BI { get; set; }
        public LotAttributeValues ColiF { get; set; }
        public LotAttributeValues EColi { get; set; }
        public LotAttributeValues Ethox { get; set; }
        public LotAttributeValues Gluten { get; set; }
        public LotAttributeValues Gran { get; set; }
        public LotAttributeValues H2O { get; set; }
        public LotAttributeValues InsP { get; set; }
        public LotAttributeValues Lead { get; set; }
        public LotAttributeValues Mold { get; set; }
        public LotAttributeValues RodHrs { get; set; }
        public LotAttributeValues Sal { get; set; }
        public LotAttributeValues Scan { get; set; }
        public LotAttributeValues Scov { get; set; }
        public LotAttributeValues TPC { get; set; }
        public LotAttributeValues Yeast { get; set; }
    }

    public class LotAttributeValues
    {
        public double? WeightedAverage { get; set; }
        public bool HasResolvedDefects { get; set; }
        public double? LabTestValue { get; set; }
        public DateTime? AttributeDate { get; set; }

        public static LotAttributeValues Empty { get { return new LotAttributeValues(); } }
    }

    public class LotCustomerAllowanceModel
    {
        public string CustomerKey { get; set; }
        public string CustomerName { get; set; }
    }
}