using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class InHouseInvoiceCopyReport : Report, IEntityReport<SalesOrderInvoice>
    {
        public InHouseInvoiceCopyReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            MoveNum.Value = this.Field(m => m.MovementNumber);
            InvoiceDate.Value = this.Field(m => m.InvoiceDate, "{0:M/d/yyyy}");

            this.Bind(SoldToSubReport, o => o.SoldTo.Address).SubReport<AddressLabelReport>();
            this.Bind(ShipToSubReport, o => o.ShipTo.Address).SubReport<AddressLabelReport>();

            Origin.Value = this.Field(m => Origin);
            ShipDate.Value = this.Field(m => m.ShipDate, "{0:M/d/yyyy}");
            Broker.Value = this.Field(m => m.Broker);
            PaymentTerms.Value = this.Field(m => m.PaymentTerms);
            Freight.Value = this.Field(m => m.Freight);
            ShipVia.Value = this.Field(m => m.ShipVia);
            PO.Value = this.Field(m => m.PONumber);
            InvoiceNotes.Value = this.Field(m => m.InvoiceNotes);

            this.Table(ItemDetails, m => m.InhouseInvoicePickedItems)
                .AddSort(m => m.ProductName)
                .With
                (
                    t => Items_Contract.Trimmed().Value = t.Field(m => m.Contract),
                    t => Items_Product.Trimmed().Value = t.Field(m => m.ProductCode),
                    t => Items_ProductName.Trimmed().Value = t.Field(m => m.ProductName),
                    t => Items_Packaging.Trimmed().Value = t.Field(m => m.PackagingName),
                    t => Items_Treatment.Trimmed().Value = t.Field(m => m.TreatmentNameShort),
                    t => Items_Ordered.Trimmed().Value = t.Field(m => m.QuantityOrdered, "{0:#,###}"),
                    t => Items_Shipped.Trimmed().Value = t.Field(m => m.QuantityShipped, "{0:#,###}"),
                    t => Items_NetWeight.Trimmed().Value = t.Field(m => m.NetWeight, "{0:#,###}"),

                    t => Items_Base.Trimmed().Value = t.Field(m => m.BaseValue, "{0:N2}"),
                    t => Items_Freight.Trimmed().Value = t.Field(m => m.FreightValue, "{0:N2}"),
                    t => Items_TreatmentValue.Trimmed().Value = t.Field(m => m.TreatmentValue, "{0:N2}"),
                    t => Items_WarehouseCost.Trimmed().Value = t.Field(m => m.WarehouseValue, "{0:N2}"),

                    t => Items_Price.Trimmed().Value = t.Field(m => m.TotalPrice, "{0:C3}"),
                    t => Items_Value.Trimmed().Value = t.Field(m => m.TotalValue, "{0:N2}"),
                    
                    t => Items_TotalOrdered.Value = t.SumOfField(m => m.QuantityOrdered, "{0:#,###}"),
                    t => Items_TotalShipped.Value = t.SumOfField(m => m.QuantityShipped, "{0:N0}"),
                    t => Items_TotalNetWgt.Value = t.SumOfField(m => m.NetWeight, "{0:N0}"),
                    t => Items_TotalBase.Value = t.SumOfField(m => m.BaseValue, "{0:N2}"),
                    t => Items_TotalFreight.Value = t.SumOfField(m => m.FreightValue, "{0:N2}"),
                    t => Items_TotalTreatment.Value = t.SumOfField(m => m.TreatmentValue, "{0:N2}"),
                    t => Items_TotalWHCost.Value = t.SumOfField(m => m.WarehouseValue, "{0:N2}"),
                    t => Items_TotalValue.Value = t.SumOfField(m => m.TotalValue, "{0:N2}")
               );

            txtFreight.Value = this.Field(o => o.FreightCharge, "{0:N2}");
            txtTotalDue.Value = this.Field(o => o.TotalDue, "{0:N2}");

            txtPaprika.Value = this.Field(o => o.SumPaprika);
            txtPaprikaETO.Value = this.Field(o => o.SumPaprikaETO);
            txtPaprikaGamma.Value = this.Field(o => o.SumPaprikaGamma);

            txtPowders.Value = this.Field(o => o.SumPowder);
            txtPowdersETO.Value = this.Field(o => o.SumPowderETO);
            txtPowdersGamma.Value = this.Field(o => o.SumPowderGamma);

            txtPepper.Value = this.Field(o => o.SumPepper);
            txtPepperETO.Value = this.Field(o => o.SumPepperETO);
            txtPepperGamma.Value = this.Field(o => o.SumPepperGamma);

            txtHots.Value = this.Field(o => o.SumHots);
            txtHotsETO.Value = this.Field(o => o.SumHotsETO);
            txtHotsGamma.Value = this.Field(o => o.SumHotsGamma);

            txtFreightRegular.Value = this.Field(o => o.FreightRegular);
            txtFreightInProduct.Value = this.Field(o => o.FreightInProduct);
            txtFreightUPS.Value = this.Field(o => o.FreightUPS);
            txtFreightWH.Value = this.Field(o => o.FreightWH);
            txtRentWH.Value = this.Field(o => o.RentWH);

            MoveNumBottom.Value = this.Field(m => m.MovementNumber);
        }
    }
}