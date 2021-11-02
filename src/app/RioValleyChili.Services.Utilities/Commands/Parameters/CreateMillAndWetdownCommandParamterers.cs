using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateMillAndWetdownParameters : SetMillAndWetdownParameters<ICreateMillAndWetdownParameters> { }

    internal class UpdateMillAndWetdownParameters : SetMillAndWetdownParameters<IUpdateMillAndWetdownParameters>
    {
        internal LotKey LotKey { get; set; }
    }

    internal abstract class SetMillAndWetdownParameters<TParams>
        where TParams : ISetMillAndWetdownParameters
    {
        internal TParams Params { get; set; }

        internal ChileProductKey ChileProductKey { get; set; }
        internal LocationKey ProductionLineKey { get; set; }
        internal List<CreateMillAndWetdownResultItemCommandParameters> ResultItems { get; set; }
        internal List<PickedInventoryParameters> PickedItems { get; set; }
    }
}