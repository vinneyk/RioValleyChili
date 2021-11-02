using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.ProductionResultsService
{
    public interface IProductionAdditiveInputsReportReturn
    {
        DateTime ProductionStart { get; }
        DateTime ProductionEnd { get; }
        IEnumerable<IProductionAdditiveInputs_ByDateReturn> ByDates { get; }
    }

    public interface IProductionAdditiveInputs_ByDateReturn
    {
        DateTime ProductionDate { get; }
        IEnumerable<IProductionLotAdditiveInputs> Lots { get; }
        IEnumerable<IProductionAdditiveInputs_Totals> Totals { get; }
    }

    public interface IProductionLotAdditiveInputs
    {
        string LotKey { get; }
        string Product { get; }
        IEnumerable<IProductionAdditiveInputs_ByAdditiveTypeReturn> Additives { get; }
    }

    public interface IProductionAdditiveInputs_ByAdditiveTypeReturn
    {
        string AdditiveType { get; }
        IEnumerable<IProductionAdditiveInputPicked> PickedItems { get; }
    }

    public interface IProductionAdditiveInputPicked
    {
        string AdditiveType { get; }
        string LotKey { get; }
        int TotalPoundsPicked { get; }
        string UserResultEntered { get; }
    }

    public interface IProductionAdditiveInputs_Totals
    {
        string AdditiveType { get; }
        double TotalPoundsPicked { get; }
    }
}