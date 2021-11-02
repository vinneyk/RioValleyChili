using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

namespace RioValleyChili.Client.Reporting.Reports
{
    /// <summary>
    /// Summary description for LabResultsReport.
    /// </summary>
    public partial class LabResultsReport : Report, IEntityReport<LabResultsReportModel>
    {
        public LabResultsReport()
        {
            InitializeComponent();

            QualityStatus.Value = this.Field(m => m.QualityStatusString);
            WorkType.Value = this.Field(m => m.WorkType);
            ProductionLineHeader.Value = this.Field(m => m.FormattedProductionLineDescription);
            ProductName.Value = this.Field(m => m.ProductName);
            LotKey.Value = this.Field(m => m.LotKey);
            ProductCode.Value = this.Field(m => m.ProductCode);
            ProductionState.Value = this.Field(m => m.ProductionStatus);
            Customer.Value = this.Field(m => m.CustomerName);
            ProductionLine.Value = this.Field(m => m.FormattedProductionLineDescription);
            Shift.Value = this.Field(m => m.ProductionShift);
            PackScheduleNumber.Value = this.Field(m => m.PackScheduleKey);
            ProductionDate.Value = this.Field(m => m.ProductionEndDate, "{0:d}");
            Allowances.Value = this.Field(m => m.Allowances);
            LowBacteria.Value = this.Field(m => m.LoBac);
            Hold.Value = this.Field(m => m.HasHold);
            HoldDescription.Value = this.Field(m => m.HoldDescription);
            Notes.Value = this.Field(m => m.NotesString);

            Bind(m => m.ProductSpec.Asta, m => m.LotAttributeValues.Asta, AstaMinSpec, AstaMaxSpec, AstaTarget, AstaValue, AstaTestDate, AstaCalcValue);
            Bind(m => m.ProductSpec.H2O, m => m.LotAttributeValues.H2O, H2OMinSpec, H2OMaxSpec, H2OTarget, H2OValue, H2OTestDate, H2OCalcValue);
            Bind(m => m.ProductSpec.Scan, m => m.LotAttributeValues.Scan, ScanMinSpec, ScanMaxSpec, ScanTarget, ScanValue, ScanTestDate, ScanCalcValue);
            Bind(m => m.ProductSpec.AB, m => m.LotAttributeValues.AB, ABMinSpec, ABMaxSpec, ABTarget, ABValue, ABTestDate, ABCalcValue);
            Bind(m => m.ProductSpec.Gran, m => m.LotAttributeValues.Gran, GranMinSpec, GranMaxSpec, GranTarget, GranValue, GranTestDate, GranCalcValue);
            Bind(m => m.ProductSpec.Ash, m => m.LotAttributeValues.Ash, AshMinSpec, AshMaxSpec, AshTarget, AshValue, AshTestDate, AshCalcValue);
            Bind(m => m.ProductSpec.AIA, m => m.LotAttributeValues.AIA, AIAMinSpec, AIAMaxSpec, AIATarget, AIAValue, AIATestDate, AIACalcValue);
            Bind(m => m.ProductSpec.AToxin, m => m.LotAttributeValues.AToxin, AToxinMinSpec, AToxinMaxSpec, AToxinTarget, AToxinValue, AToxinTestDate, AToxinCalcValue);
            Bind(m => m.ProductSpec.ColiF, m => m.LotAttributeValues.ColiF, ColiFMinSpec, ColiFMaxSpec, ColiFTarget, ColiFValue, ColiFTestDate, ColiFCalcValue);
            Bind(m => m.ProductSpec.EColi, m => m.LotAttributeValues.EColi, EColiMinSpec, EColiMaxSpec, EColiTarget, EColiValue, EColiTestDate, EColiCalcValue);
            Bind(m => m.ProductSpec.Gluten, m => m.LotAttributeValues.Gluten, GlutenMinSpec, GlutenMaxSpec, GlutenTarget, GlutenValue, GlutenTestDate, GlutenCalcValue);
            Bind(m => m.ProductSpec.Lead, m => m.LotAttributeValues.Lead, LeadMinSpec, LeadMaxSpec, LeadTarget, LeadValue, LeadTestDate, LeadCalcValue);
            Bind(m => m.ProductSpec.Mold, m => m.LotAttributeValues.Mold, MoldMinSpec, MoldMaxSpec, MoldTarget, MoldValue, MoldTestDate, MoldCalcValue);
            Bind(m => m.ProductSpec.Sal, m => m.LotAttributeValues.Sal, SalMinSpec, SalMaxSpec, SalTarget, SalValue, SalTestDate, SalCalcValue);
            Bind(m => m.ProductSpec.Scov, m => m.LotAttributeValues.Scov, ScovMinSpec, ScovMaxSpec, ScovTarget, ScovValue, ScovTestDate, ScovCalcValue);
            Bind(m => m.ProductSpec.TPC, m => m.LotAttributeValues.TPC, TPCMinSpec, TPCMaxSpec, TPCTarget, TPCValue, TPCTestDate, TPCCalcValue);
            Bind(m => m.ProductSpec.Yeast, m => m.LotAttributeValues.Yeast, YeastMinSpec, YeastMaxSpec, YeastTarget, YeastValue, YeastTestDate, YeastCalcValue);
            Bind(m => m.ProductSpec.BI, m => m.LotAttributeValues.BI, BIMinSpec, BIMaxSpec, BITarget, BIValue, BITestDate, BICalcValue);
        }

        private void Bind(Expression<Func<LabResultsReportModel, ProductAttributeSpec>> selectSpec, Expression<Func<LabResultsReportModel, LotAttributeValues>> selectAttribute,
            TextBox minSpec, TextBox maxSpec, TextBox target, TextBox value, TextBox date, TextBox computed)
        {
            new LabResultsAttribute(this, selectSpec, selectAttribute).Bind(minSpec, maxSpec, target, value, date, computed);
        }

        #region Report Functions

        public static DateTime DefaultStartDate()
        {
#if (DEBUG)
            return new DateTime(2014, 6, 16);
#endif
            return DateTime.Now.Date.AddDays(-1);
        }

        public static DateTime DefaultEndDate()
        {
#if (DEBUG)
            return new DateTime(2014, 6, 17);
#endif
            return DateTime.Now.Date;
        }

        public static string DescribeParameters(string lotKey, DateTime? startDate, DateTime? endDate)
        {
            return string.IsNullOrWhiteSpace(lotKey)
                ? startDate.HasValue && endDate.HasValue
                    ? string.Format("Tested Between: {0:d} and {1:d}", startDate.Value, endDate.Value)
                    : ""
                : lotKey;
        }

        public static string AttributeName(string key, IDictionary<string, ProductAttributeSpec> productSpec)
        {
            return productSpec[key].AttributeName;
        }

        public static IEnumerable<IDehydratedInputReturn> GetWIPSourceDataSource(object sender)
        {
            var dataObject = (Telerik.Reporting.Processing.IDataObject) sender;
            var wipSource = dataObject["DehydratedInputs"] as IEnumerable<IDehydratedInputReturn>;
            return wipSource;
        }

        public static bool IsDefective(LotAttributeValues attributeValue, ProductAttributeSpec productSpec)
        {
            if(attributeValue == null || productSpec == null || attributeValue.HasResolvedDefects)
            {
                return false;
            }

            return attributeValue.LabTestValue < productSpec.MinValue || attributeValue.LabTestValue > productSpec.MaxValue;
        }

        public static bool IsResolved(LotAttributeValues attributeValue)
        {
            return attributeValue != null && attributeValue.HasResolvedDefects;
        }

        public static double? GetMeanValue(ProductAttributeSpec spec)
        {
            if (spec == null) return null;
            if (spec.MaxValue.HasValue && spec.MinValue.HasValue)
            {
                return (new[] {spec.MaxValue.Value, spec.MinValue.Value}).Average();
            }

            return spec.MinValue ?? spec.MaxValue;
        }

        #endregion

        private class LabResultsAttribute
        {
            private readonly LabResultsReport _report;
            private readonly Expression<Func<LabResultsReportModel, ProductAttributeSpec>> _productSpec;
            private readonly Expression<Func<LabResultsReportModel, LotAttributeValues>> _attribute;

            public LabResultsAttribute(LabResultsReport report,
                Expression<Func<LabResultsReportModel, ProductAttributeSpec>> productSpec,
                Expression<Func<LabResultsReportModel, LotAttributeValues>> attribute)
            {
                _report = report;
                _productSpec = productSpec;
                _attribute = attribute;
            }

            public void Bind(TextBox minSpec, TextBox maxSpec, TextBox target, TextBox value, TextBox testDate, TextBox computed)
            {
                minSpec.Value = _report.Field(FromSpec(m => m.MinValue));
                maxSpec.Value = _report.Field(FromSpec(m => m.MaxValue));
                target.Value = string.Format("=GetMeanValue({0})", _report.PartialField(_productSpec));

                value.Value = _report.Field(FromAttribute(a => a.LabTestValue));

                var defectiveRule = new FormattingRule();
                defectiveRule.Filters.Add(string.Format("=IsDefective({0}, {1})", _report.PartialField(_attribute), _report.PartialField(_productSpec)),
                    FilterOperator.Equal, "true");
                defectiveRule.Style.Color = Color.Red;

                var resolvedRule = new FormattingRule();
                resolvedRule.Filters.Add(string.Format("=IsResolved({0})", _report.PartialField(_attribute)),
                    FilterOperator.Equal, "true");
                resolvedRule.Style.Color = Color.Blue;

                value.ConditionalFormatting.Add(defectiveRule);
                value.ConditionalFormatting.Add(resolvedRule);

                testDate.Value = _report.Field(FromAttribute(a => a.AttributeDate));
                computed.Value = _report.Field(FromAttribute(a => a.WeightedAverage));
            }

            private Expression<Func<LabResultsReportModel, TResult>> FromSpec<TResult>(Expression<Func<ProductAttributeSpec, TResult>> select)
            {
                Expression<Func<LabResultsReportModel, TResult>> fromAttribute = m => select.Invoke(_productSpec.Invoke(m));
                return fromAttribute.Expand().Expand();
            }

            private Expression<Func<LabResultsReportModel, TResult>> FromAttribute<TResult>(Expression<Func<LotAttributeValues, TResult>> select)
            {
                Expression<Func<LabResultsReportModel, TResult>> fromAttribute = m => select.Invoke(_attribute.Invoke(m));
                return fromAttribute.Expand().Expand();
            }
        }
    }
}