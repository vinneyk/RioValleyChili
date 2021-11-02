using RioValleyChili.Client.Reporting.Models;

namespace RioValleyChili.Client.Reporting.Reports
{
    using Telerik.Reporting;

    /// <summary>
    /// Summary description for SampleMatchSummary.
    /// </summary>
    public partial class SampleMatchSummary : Report, IEntityReport<SampleOrderMatchingSummaryReportModel>
    {
        public SampleMatchSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            txtSampleOrderKey.Value = this.Field(m => m.SampleOrderKey);
            txtCompany.Value = this.Field(m => m.Company);
            txtContact.Value = this.Field(m => m.Contact);

            this.Table(listItems, m => m.Items)
                .With
                (
                    t => txtSampleMatch.Value = t.Field(m => m.SampleMatch),

                    t => t.Table(tblItemAttributes, m => m.This)
                    .With
                    (
                        t2 => txtAstaColorSpec.Value = t2.Field(m => m.AstaColorSpec),
                        t2 => txtMoistureSpec.Value = t2.Field(m => m.MoistureSpec),
                        t2 => txtParticleSpec.Value = t2.Field(m => m.ParticleSpec),
                        t2 => txtSurfaceColorSpec.Value = t2.Field(m => m.SurfaceColorSpec),
                        t2 => txtABSpec.Value = t2.Field(m => m.ABSpec),
                        t2 => txtSHUSpec.Value = t2.Field(m => m.SHUSpec),
                        t2 => txtTPCSpec.Value = t2.Field(m => m.TPCSpec),
                        t2 => txtYeastSpec.Value = t2.Field(m => m.YeastSpec),
                        t2 => txtMoldSpec.Value = t2.Field(m => m.MoldSpec),
                        t2 => txtColiformsSpec.Value = t2.Field(m => m.ColiformsSpec),
                        t2 => txtEColiSpec.Value = t2.Field(m => m.EColiSpec),
                        t2 => txtSalmonellaSpec.Value = t2.Field(m => m.SalmonellaSpec),

                        t2 => txtAstaColorSample.Value = t2.Field(m => m.AstaColorSample),
                        t2 => txtMoistureSample.Value = t2.Field(m => m.MoistureSample),
                        t2 => txtParticleSample.Value = t2.Field(m => m.ParticleSample),
                        t2 => txtSurfaceColorSample.Value = t2.Field(m => m.SurfaceColorSample),
                        t2 => txtABSample.Value = t2.Field(m => m.ABSample),
                        t2 => txtSHUSample.Value = t2.Field(m => m.SHUSample),
                        t2 => txtTPCSample.Value = t2.Field(m => m.TPCSample),
                        t2 => txtYeastSample.Value = t2.Field(m => m.YeastSample),
                        t2 => txtMoldSample.Value = t2.Field(m => m.MoldSample),
                        t2 => txtColiformsSample.Value = t2.Field(m => m.ColiformsSample),
                        t2 => txtEColiSample.Value = t2.Field(m => m.EColiSample),
                        t2 => txtSalmonellaSample.Value = t2.Field(m => m.SalmonellaSample),

                        t2 => txtAstaColorMatch.Value = t2.Field(m => m.AstaColorMatch),
                        t2 => txtMoistureMatch.Value = t2.Field(m => m.MoistureMatch),
                        t2 => txtParticleMatch.Value = t2.Field(m => m.ParticleMatch),
                        t2 => txtSurfaceColorMatch.Value = t2.Field(m => m.SurfaceColorMatch),
                        t2 => txtABMatch.Value = t2.Field(m => m.ABMatch),
                        t2 => txtSHUMatch.Value = t2.Field(m => m.SHUMatch),
                        t2 => txtTPCMatch.Value = t2.Field(m => m.TPCMatch),
                        t2 => txtYeastMatch.Value = t2.Field(m => m.YeastMatch),
                        t2 => txtMoldMatch.Value = t2.Field(m => m.MoldMatch),
                        t2 => txtColiformsMatch.Value = t2.Field(m => m.ColiformsMatch),
                        t2 => txtEColiMatch.Value = t2.Field(m => m.EColiMatch),
                        t2 => txtSalmonellaMatch.Value = t2.Field(m => m.SalmonellaMatch)
                    ),

                    t => t.Table(tblItemDetails, m => m.This)
                    .With
                    (
                        t2 => txtStatus.Value = t2.Field(m => m.Status),
                        t2 => txtProductName.Value = t2.Field(m => m.ProductName),
                        t2 => txtLotNumber.Value = t2.Field(m => m.LotNumber),
                        t2 => txtQuantity.Value = t2.Field(m => m.QuantitySent),
                        t2 => txtEmployee.Value = t2.Field(m => m.Employee),
                        t2 => txtReceived.Value = t2.Field(m => m.Received, "{0:M/dd/yyyy}"),
                        t2 => txtSampleDate.Value = t2.Field(m => m.SampleDate, "{0:M/dd/yyyy}"),
                        t2 => txtDateCompleted.Value = t2.Field(m => m.Completed, "{0:M/dd/yyyy}")
                    )
                );
        }
    }
}