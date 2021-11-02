using RioValleyChili.Client.Reporting.Content;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for CustomerContract.
    /// </summary>
    public partial class CustomerContract : Report, IEntityReport<Models.CustomerContract>
    {
        public CustomerContract()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            ContractKey.Value = this.Field(m => m.CustomerContractKey);
            ContractDate.Value = this.Field(m => m.ContractDate, "{0:MM/dd/yyyy}");

            PONumber.Value = this.Field(m => m.CustomerPurchaseOrder);
            TermBegin.Value = this.Field(m => m.TermBegin, "{0:MM/dd/yyyy}");
            TermEnd.Value = this.Field(m => m.TermEnd, "{0:MM/dd/yyyy}");
            FOB.Value = this.Field(m => m.FOB);
            PaymentTerms.Value = this.Field(m => m.PaymentTerms);
            Broker.Value = this.Field(m => m.BrokerCompanyName);
            ContractType.Value = this.Field(m => m.ContractType);
            AddressLabelSubReport.Bindings.Add(new Binding("ReportSource", "= GetAddressLabelSource(ReportItem.DataObject)"));

            ItemsTable.Bindings.Add(new Binding("DataSource", this.Field(m => m.ContractItems)));
            ItemsTable.Sortings.Add(this.ChildMemberField(m => m.ContractItems, c => c.ChileProductName), SortDirection.Asc);
            Items_ProductName.Value = this.ChildMemberField(m => m.ContractItems, c => c.ChileProductName);
            Items_CustomerCode.Value = this.ChildMemberField(m => m.ContractItems, c => c.CustomerProductCode);
            Items_PackagingName.Value = this.ChildMemberField(m => m.ContractItems, c => c.PackagingProductName);
            Items_Treatment.Value = this.ChildMemberField(m => m.ContractItems, m => m.Treatment);
            Items_Quantity.Value = this.ChildMemberField(m => m.ContractItems, m => m.Quantity);
            Items_TotalWeight.Value = this.ChildMemberField(m => m.ContractItems, m => m.TotalWeight, "{0:#,###}");
            Items_Price.Value = this.ChildMemberField(m => m.ContractItems, m => m.Price, "{0:C3}");

            TotalQuantityOnContract.Value = this.Field(m => m.TotalQuantityOnContract, "{0:#,###}");
            TotalPoundsOnContract.Value = this.Field(m => m.TotalPoundsOnContract, "{0:#,###}");

            SpecialInstructions.Value = this.Field(m => m.NotesToPrint);
            SpecialInstructionsLabel.Bindings.Add(new Binding("Visible", this.Field(m => m.HasSpecialInstructions)));
            SpecialInstructions.Bindings.Add(new Binding("Visible", this.Field(m => m.HasSpecialInstructions)));

            TermsAndConditions.Value = this.Format(ReportStrings.ContractTermsAndConditionsHtml,
                this.PartialField(c => c.TermBegin, "{0:MMMM d, yyyy}"),
                this.PartialField(c => c.TermEnd, "{0:MMMM d, yyyy}"));

            ContractKeyAndNumber.Value = this.Format("{0} ({1})",
                this.PartialField(c => c.CustomerContractKey),
                this.PartialField(c => c.ContractNumber));

            CompanyNameSignatureLine.Value = this.Field(m => m.CustomerName);
        }

        public static ReportSource GetAddressLabelSource(object sender)
        {
            var dataObject = (Telerik.Reporting.Processing.IDataObject)sender;
            return new InstanceReportSource
                {
                    ReportDocument = new AddressLabelReport
                    {
                        DataSource = dataObject["AddressLabel"]
                    }
                };
        }
    }
}