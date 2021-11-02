using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for ShipmentDetails.
    /// </summary>
    public partial class PendingOrderDetailsReport : Report, IEntityReport<PendingOrderDetails>
    {
        public PendingOrderDetailsReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Bind(ps_Label, l => l.Visible, m => m.HasPendingCustomerOrders);
            this.Bind(ps_Panel, l => l.Visible, m => m.HasPendingCustomerOrders);
            this.Bind(labelPendingWarehouse, l => l.Visible, m => m.HasPendingWarheouseOrders);
            this.Bind(panelPendingWarehouse, l => l.Visible, m => m.HasPendingWarheouseOrders);

            textBetweenDates.Value = this.Field(m => m.BetweenDatesText);
            textOrdersScheduled.Value = this.Field(m => m.ScheduledText);
            textOrdersScheduledValue.Value = this.Field(m => m.ScheduledAmount);
            textOrdersShipped.Value = this.Field(m => m.ShippedText);
            textOrdersShippedValue.Value = this.Field(m => m.ShippedAmount);
            textOrdersRemaining.Value = this.Field(m => m.RemainingText);
            textOrdersRemainingValue.Value = this.Field(m => m.RemainingAmount);
            textNewOrders.Value = this.Field(m => m.NewText);
            textNewOrdersValue.Value = this.Field(m => m.NewAmount);
            
            this.Table(ps_Table, m => m.PendingShipmentItems)
                .With
                (
                    t => ps_txtSectionLabel.Value = t.Field(m => m.Section.SectionLabel),
                    t => ps_txtSectionDescription.Value = t.Field(m => m.Section.SectionDescription),
                    t => t.Bind(ps_pnlSection, p => p.Style.BackgroundColor, m => m.Section.SectionColor),
            
                    t => ps_txtCustomer.Value = t.Field(m => m.Order.Name),
                    t => ps_txtDateRecd.Value = t.Field(m => m.Order.DateRecd),
                    t => ps_txtDateSchShip.Value = t.Field(m => m.Order.ShipmentDate),
                    t => ps_txtOrderNum.Value = t.Field(m => m.Order.OrderNum),
                    t => ps_txtOrigin.Value = t.Field(m => m.Order.Origin),
                    t => ps_txtStatus.Value = t.Field(m => m.Order.Status),
                    t => ps_chkSample.Value = t.Field(m => m.Order.Sample),
            
                    t => ps_txtPicked.Value = t.Field(m => m.Item.QuantityPicked),
                    t => ps_txtOrdered.Value = t.Field(m => m.Item.QuantityOrdered),
                    t => ps_txtPackaging.Value = t.Field(m => m.Item.Packaging),
                    t => ps_txtProduct.Value = t.Field(m => m.Item.Product),
                    t => ps_txtTreatment.Value = t.Field(m => m.Item.Treatment),
                    t => ps_txtLbsToShip.Value = t.Field(m => m.Item.LbsToShip)
                );

            this.Table(wo_Table, m => m.PendingWarehouseItems)
                .With
                (
                    t => wo_txtSectionLabel.Value = t.Field(m => m.Section.SectionLabel),
                    t => wo_txtSectionDescription.Value = t.Field(m => m.Section.SectionDescription),
                    t => t.Bind(wo_pnlSection, p => p.Style.BackgroundColor, m => m.Section.SectionColor),

                    t => wo_txtFrom.Value = t.Field(m => m.Order.From),
                    t => wo_txtTo.Value = t.Field(m => m.Order.To),
                    t => wo_txtShipDate.Value = t.Field(m => m.Order.ShipmentDate),
                    t => wo_txtOrderNum.Value = t.Field(m => m.Order.OrderNum),
                    t => wo_txtStatus.Value = t.Field(m => m.Order.Status),

                    t => wo_txtOrdered.Value = t.Field(m => m.Item.QuantityOrdered),
                    t => wo_txtPackaging.Value = t.Field(m => m.Item.Packaging),
                    t => wo_txtPicked.Value = t.Field(m => m.Item.QuantityPicked),
                    t => wo_txtProduct.Value = t.Field(m => m.Item.Product),
                    t => wo_txtTreatment.Value = t.Field(m => m.Item.Treatment),
                    t => wo_txtLbsToShip.Value = t.Field(m => m.Item.LbsToShip)
                );
        }
    }
}