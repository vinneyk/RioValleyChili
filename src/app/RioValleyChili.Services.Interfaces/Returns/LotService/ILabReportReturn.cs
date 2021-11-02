using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.LotService
{
    public interface ILabReportReturn
    {
        IEnumerable<string> AttributeNames { get; }
        IDictionary<string, ILabReportChileProduct> ChileProducts { get; }
        IEnumerable<ILabReportChileLotReturn> ChileLots { get; }
    }
}