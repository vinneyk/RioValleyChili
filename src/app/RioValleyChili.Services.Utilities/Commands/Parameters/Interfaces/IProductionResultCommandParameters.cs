using System;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    internal interface IProductionResultCommandParameters
    {
        string User { get; }

        ILocationKey ProductionLineLocationKey { get; }

        DateTime ProductionStart { get; }

        DateTime ProductionEnd { get; }

        string ShiftKey { get; }
    }
}