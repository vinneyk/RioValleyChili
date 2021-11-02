using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LabReportReturn : ILabReportReturn
    {
        public IEnumerable<string> AttributeNames { get; internal set; }

        public IDictionary<string, ILabReportChileProduct> ChileProducts { get; internal set; }

        public IEnumerable<ILabReportChileLotReturn> ChileLots { get; internal set; }
    }
}