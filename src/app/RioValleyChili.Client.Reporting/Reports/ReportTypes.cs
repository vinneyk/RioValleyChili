namespace RioValleyChili.Client.Reporting.Reports
{
    public static class ReportTypes
    {
        public static class ProductionReports
        {
            public static string ProductionBatchPacketReport = typeof(ProductionBatchPacketReport).AssemblyQualifiedName;
            public static string PackSchedulePickSheetReport = typeof(PackSchedulePickSheetReport).AssemblyQualifiedName;
            public static string ProductionRecapReport = typeof(ProductionRecapReport).AssemblyQualifiedName;
            public static string ProductionAdditiveInputsReport = typeof(ProductionAdditiveInputsReport).AssemblyQualifiedName;
            public static string ProductionScheduleReport = typeof(ProductionScheduleReport).AssemblyQualifiedName;
        }

        public static class SalesReports
        {
            public static string CustomerContractReport = typeof(CustomerContract).AssemblyQualifiedName;
            public static string CustomerContractDrawSummaryReport = typeof(CustomerContractDrawSummaryReport).AssemblyQualifiedName;
            public static string CustomerOrderAcknowledgementReport = typeof(CustomerOrderAcknowledgementReport).AssemblyQualifiedName;
            public static string CustomerOrderInvoiceReport = typeof(CustomerOrderInvoiceReport).AssemblyQualifiedName;
            public static string InHouseInvoiceCopyReport = typeof(InHouseInvoiceCopyReport).AssemblyQualifiedName;
            public static string PendingOrderDetailsReport = typeof(PendingOrderDetailsReport).AssemblyQualifiedName;
            public static string InHouseOrderAcknowledgementReport = typeof(WarehouseOrderAcknowledgementReport).AssemblyQualifiedName;

            public static string MiscOrderInvoiceReport = typeof(MiscOrderInvoiceReport).AssemblyQualifiedName;
            public static string MiscOrderCustomerAcknowledgementReport = typeof(MiscOrderCustomerAcknowledgementReport).AssemblyQualifiedName;
            public static string MiscOrderInternalAcknowledgementReport = typeof(MiscOrderInternalAcknowledgementReport).AssemblyQualifiedName;

            public static string SalesQuoteReport = typeof(SalesQuoteReport).AssemblyQualifiedName;
        }

        public static class ShipmentOrderReports
        {
            public static string WarehouseOrderAcknowledgementReport = typeof(WarehouseOrderAcknowledgementReport).AssemblyQualifiedName;
            public static string BillOfLadingReport = typeof(BillOfLadingReport).AssemblyQualifiedName;
            public static string PackingListReport = typeof(WarehouseOrderPackingListReport).AssemblyQualifiedName;
            public static string PackingListBarcodeReport = typeof(PackingListBarcodeReport).AssemblyQualifiedName;
            public static string PickSheetReport = typeof(WarehouseOrderPickSheetReport).AssemblyQualifiedName;
            public static string CertificateOfAnalysisReport = typeof(CertificateOfAnalysisReport).AssemblyQualifiedName;
            public static string PendingOrderDetailsReport = typeof(PendingOrderDetailsReport).AssemblyQualifiedName;
        }

        public static class QualityControlReports
        {
            public static string LabResultsReport = typeof(LabResultsReport).AssemblyQualifiedName;
        }

        public static class SampleOrderReports
        {
            public static string MatchSummaryReport = typeof(SampleMatchSummary).AssemblyQualifiedName;
            public static string SampleRequestReport = typeof(SampleRequestReport).AssemblyQualifiedName;
        }

        public static class MaterialsReceivedReports
        {
            public static string ChileRecapReport = typeof(ChileProductReceivedRecapReport).AssemblyQualifiedName;
        }

        public static class InventoryReports
        {
            public static string InventoryCycleCountReport = typeof(InventoryCycleCountReport).AssemblyQualifiedName;
        }
    }
}
