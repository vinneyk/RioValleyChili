namespace RioValleyChili.Services.Interfaces.Returns.SampleOrderService
{
    public interface ISampleOrderItemMatchReturn
    {
        string Gran { get; }
        string AvgAsta { get; }
        string AoverB { get; }
        string AvgScov { get; }
        string H2O { get; }
        string Scan { get; }
        string Yeast { get; }
        string Mold { get; }
        string Coli { get; }
        string TPC { get; }
        string EColi { get; }
        string Sal { get; }
        string InsPrts { get; }
        string RodHrs { get; }

        string Notes { get; }
    }
}