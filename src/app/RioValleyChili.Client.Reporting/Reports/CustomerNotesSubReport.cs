using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for CustomerNotesSubReport.
    /// </summary>
    public partial class CustomerNotesSubReport : Report, IEntityReport<ICustomerNotesContainer>
    {
        public CustomerNotesSubReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.Table(TableCustomerNotes, m => m.CustomerNotes)
                .AddSort(m => m.Name)
                .With
                (
                    t => TableCustomerNotes_Name.Value = t.Field(m => m.Name),
                    t => t.Table(TableCustomerNotes_TableNotes, m => m.CustomerNotes)
                        .AddSort(m => m.Type)
                        .With
                        (
                            t2 => TableCustomerNotes_TableNotes_Type.Value = t2.Field(m => m.Type),
                            t2 => TableCustomerNotes_TableNotes_Text.Value = t2.Field(m => m.Text),
                            t2 => t2.Bind(TableCustomerNotes_TableNotes_Text, n => n.Style.Font.Bold, m => m.Bold)
                        )
                );
        }
    }
}