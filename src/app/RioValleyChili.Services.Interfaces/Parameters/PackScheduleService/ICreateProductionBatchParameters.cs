using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.PackScheduleService
{
    public interface ICreateProductionBatchParameters : IUserIdentifiable, IProductionBatchTargetParameters
    {
        string PackScheduleKey { get; }
        string Notes { get; }
        string[] Instructions { get; }

        LotTypeEnum? LotType { get; }
        DateTime? LotDateCreated { get; }
        int? LotSequence { get; }
    }
}