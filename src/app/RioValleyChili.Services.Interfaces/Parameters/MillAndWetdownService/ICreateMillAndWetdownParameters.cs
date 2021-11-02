using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService
{
    public interface ICreateMillAndWetdownParameters : ISetMillAndWetdownParameters
    {
        DateTime ProductionDate { get; }
        int LotSequence { get; }
    }

    public interface IUpdateMillAndWetdownParameters : ISetMillAndWetdownParameters
    {
        string LotKey { get; }
    }

    public interface ISetMillAndWetdownParameters : IUserIdentifiable
    {
        string ChileProductKey { get; }
        string ShiftKey { get; }
        string ProductionLineKey { get; }
        DateTime ProductionBegin { get; }
        DateTime ProductionEnd { get; }

        IEnumerable<IMillAndWetdownResultItemParameters> ResultItems { get; }
        IEnumerable<IMillAndWetdownPickedItemParameters> PickedItems { get; }
    }
}