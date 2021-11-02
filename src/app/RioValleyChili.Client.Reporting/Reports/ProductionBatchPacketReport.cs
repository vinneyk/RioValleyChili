using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using System;
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ProductionBatchPacketReport.
    /// </summary>
    public partial class ProductionBatchPacketReport : Report, IEntityReport<ProductionBatchPacketReportModel>
    {
        public ProductionBatchPacketReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            BindHeader();
        }

        private void BindHeader()
        {
            BatchType.Value = this.Field(m => m.Header.BatchType);
            LotNumber.Value = this.Field(m => m.Header.LotNumber);
            PackScheduleKey.Value = this.Field(m => m.Header.PackScheduleKey);
            PSNum.Value = this.Field(m => m.Header.PSNum);
            PackScheduleDate.Value = this.Field(m => m.Header.PackScheduleDate, "{0:MM/dd/yy}");
            ProductNameDisplay.Value = this.Field(m => m.Header.ProductNameDisplay);
            Description.Value = this.Field(m => m.Header.Description);
            TargetWeight.Value = this.Field(m => m.Header.TargetWeight, "{0:#,###}");
            TargetAsta.Value = this.Field(m => m.Header.TargetAsta, "{0:#,###}");
            TargetScoville.Value = this.Field(m => m.Header.TargetScoville, "{0:#,###}");
            TargetScan.Value = this.Field(m => m.Header.TargetScan, "{0:#,###}");
            CalculatedAsta.Value = this.Field(m => m.Header.CalculatedAsta, "{0:#,###}");
            CalculatedScan.Value = this.Field(m => m.Header.CalculatedScan, "{0:#,###}");
            CalculatedScoville.Value = this.Field(m => m.Header.CalculatedScoville, "{0:#,###}");
            BatchNotes.Value = this.Field(m => m.Header.BatchNotes);
        }

        #region picked inventory binding functions

        #region Picked chile inventory

        public static
            IEnumerable<KeyValuePair<string, IEnumerable<ProductionBatchPacketReportModel.PickedChileInventoryItem>>>
            GetPickedChileInventory(object sender)
        {
            return GetPickedInventory
                <ProductionBatchPacketReportModel,
                    IEnumerable<ProductionBatchPacketReportModel.PickedChileInventoryItem>>(
                        sender, m => m.PickedChileInventoryItemsByChileType);
        }

        public static IEnumerable<ProductionBatchPacketReportModel.PickedChileInventoryItem>
            GetPickedChileInventoryItems(object sender)
        {
            return GetPickedInventoryItems<ProductionBatchPacketReportModel.PickedChileInventoryItem>(sender,
                m => m.Value);
        }

        public static double SumPickedWeightForChileInventoryItems(object sender)
        {
            return
                SumPickedWeightForPickedInventoryItems<ProductionBatchPacketReportModel.PickedChileInventoryItem>(
                    sender, m => m.Value);
        }

        #endregion

        #region picked additive inventory

        public static
            IEnumerable<KeyValuePair<string, IEnumerable<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>>>
            GetPickedAdditiveInventory(object sender)
        {
            return GetPickedInventory
                <ProductionBatchPacketReportModel,
                    IEnumerable<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>>(
                        sender, m => m.PickedAdditiveInventoryItemsByAdditiveType);
        }

        public static IEnumerable<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>
            GetPickedAdditiveInventoryItems(object sender)
        {
            return GetPickedInventoryItems<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>(sender,
                m => m.Value);
        }

        public static double SumPickedWeightForAdditiveInventoryItems(object sender)
        {
            return
                SumPickedWeightForPickedInventoryItems<ProductionBatchPacketReportModel.PickedAdditiveInventoryItem>(
                    sender, m => m.Value);
        }

        #endregion

        #region picked packaging inventory

        public static IEnumerable<ProductionBatchPacketReportModel.PickedPackagingInventoryItem>
            GetPickedPackaingInventoryItems(object sender)
        {
            return
                GetDataByPropertyName<IEnumerable<ProductionBatchPacketReportModel.PickedPackagingInventoryItem>>(
                    sender,
                    GetPropertyName
                        <ProductionBatchPacketReportModel,
                            IEnumerable<ProductionBatchPacketReportModel.PickedPackagingInventoryItem>>(
                                m => m.PickedPackagingInventoryItems));
        }

        #endregion

        private static IEnumerable<KeyValuePair<string, TPickedInventoryItem>> GetPickedInventory<TModel, TPickedInventoryItem>(
            object sender, Expression<Func<TModel, IEnumerable<KeyValuePair<string, TPickedInventoryItem>>>> expression)
        {
            return GetDataByPropertyName<IEnumerable<KeyValuePair<string, TPickedInventoryItem>>>(sender, GetPropertyName(expression));
        }

        private static IEnumerable<TPickedItem> GetPickedInventoryItems<TPickedItem>(object sender, Expression<Func<KeyValuePair<string, IEnumerable<TPickedItem>>, IEnumerable<TPickedItem>>> expression)
        {
            return GetDataByPropertyName<IEnumerable<TPickedItem>>(sender, GetPropertyName(expression));
        }

        private static double SumPickedWeightForPickedInventoryItems<TPickedItem>(object sender, Expression<Func<KeyValuePair<string, IEnumerable<TPickedItem>>, IEnumerable<TPickedItem>>> expression)
            where TPickedItem : IPickedPounds
        {
            if (sender == null) return 0;
            var pickedItems = GetDataByPropertyName<IEnumerable<TPickedItem>>(sender, GetPropertyName(expression));
            return pickedItems.Sum(i => i.WeightPicked);
        }

        #endregion

        private static string GetPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(
                    string.Format("The provided expression does not refer to a property. Expression received: \"{0}\"",
                        expression));

            var propInfo = memberExpression.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("The provided expression \"{0}\" refers to a field, not a property.",
                    expression));

            return propInfo.Name;
        }

        private static TOutput GetDataByPropertyName<TOutput>(object sender, string propertyName) where TOutput : class
        {
            var dataObject = (Telerik.Reporting.Processing.IDataObject)sender;
            return dataObject[propertyName] as TOutput;
        }
    }
}