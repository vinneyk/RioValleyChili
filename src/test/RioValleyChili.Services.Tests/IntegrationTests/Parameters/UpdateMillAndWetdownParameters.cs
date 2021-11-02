using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class UpdateMillAndWetdownParameters : IUpdateMillAndWetdownParameters
    {
        public string LotKey { get; set; }
        public string ChileProductKey { get; set; }
        public string UserToken { get; set; }
        public string ShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }
        public IEnumerable<IMillAndWetdownResultItemParameters> ResultItems { get; set; }
        public IEnumerable<IMillAndWetdownPickedItemParameters> PickedItems { get; set; }
    }
}