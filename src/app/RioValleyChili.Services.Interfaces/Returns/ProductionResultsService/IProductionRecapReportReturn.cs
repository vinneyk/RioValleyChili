using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionRecapReportReturn
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }

        IProductionRecapWeightGroup BatchWeights { get; }
        IProductionRecapWeightGroup LineWeights { get; }
        IProductionRecapTimeGroup BatchTimes { get; }
        IProductionRecapTimeGroup LineTimes { get; }
        IProductionRecapTestGroup BatchTests { get; }
        IProductionRecapTestGroup LineTests { get; }

        IProductionRecap_ByLineProduct LineProductWeights { get; }
        IProductionRecap_LotGroupSection ProductDetails { get; }
        IProductionRecap_LotGroupSection LineDetails { get; }
    }

    public interface IProductionRecapWeightItem
    {
        string Name { get; }
        int Target { get; }
        int Produced { get; }
    }

    public interface IProductionRecapTestItem
    {
        string Name { get; }
        int Passed { get; }
        int Failed { get; }
        int NonCntrl { get; }
        int InProc { get; }
    }

    public interface IProductionRecapTimeItem
    {
        string Name { get; }
        float Actual { get; }
    }

    public interface IProductionRecapTestGroup
    {
        IEnumerable<IProductionRecapTestItem> Items { get; }
    }

    public interface IProductionRecapTimeGroup
    {
        IEnumerable<IProductionRecapTimeItem> Items { get; }
    }

    public interface IProductionRecapWeightGroup : IProductionRecapWeightItem
    {
        IEnumerable<IProductionRecapWeightItem> Items { get; }
    }

    public interface IProductionRecap_ByLineProduct_ByLine_ByType
    {
        string Type { get; }
        IEnumerable<IProductionRecapWeightItem> ItemsByProduct { get; }
    }

    public interface IProductionRecap_ByLineProduct_ByLine
    {
        string Line { get; }
        IEnumerable<IProductionRecap_ByLineProduct_ByLine_ByType> ItemsByType { get; }
    }

    public interface IProductionRecap_ByLineProduct
    {
        IEnumerable<IProductionRecap_ByLineProduct_ByLine> ItemsByLine { get; }
    }

    public interface IProductionRecap_Lot
    {
        string Name { get; }
        string Lot { get; }
        string Line { get; }
        string Shift { get; }
        string PSNum { get; }
        string BatchType { get; }
        string LotStat { get; }
        int TargetWeight { get; }
        int ProducedWeight { get; }
        float ProductionTime { get; }
    }

    public interface IProductionRecap_LotGroup
    {
        string Name { get; }
        IEnumerable<IProductionRecap_Lot> Items { get; }
    }

    public interface IProductionRecap_LotGroupSection
    {
        IEnumerable<IProductionRecap_LotGroup> Items { get; }
    }

    public enum TestResults
    {
        Pass,
        Fail,
        NonCntrl,
        InProc
    }
}