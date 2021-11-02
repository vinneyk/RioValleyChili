using System;
using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters.Interfaces
{
    internal interface ICreateProductionResultCommandParameters : IProductionResultCommandParameters
    {
        LotKey ProductionBatchKey { get; }

        DateTime DateTimeEntered { get; }
    }
}