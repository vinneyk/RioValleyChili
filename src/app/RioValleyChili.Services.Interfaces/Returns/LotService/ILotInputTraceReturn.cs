using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILotInputTraceReturn
    {
        IEnumerable<string> LotPath { get; }
        string Treatment { get;  }
    }
}