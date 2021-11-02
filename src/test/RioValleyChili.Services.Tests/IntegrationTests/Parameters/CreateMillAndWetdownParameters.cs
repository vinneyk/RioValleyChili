using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.MillAndWetdownService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class CreateMillAndWetdownParameters : ICreateMillAndWetdownParameters
    {
        public string UserToken { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ShiftKey { get; set; }
        public string ProductionLineKey { get; set; }
        public DateTime ProductionBegin { get; set; }
        public DateTime ProductionEnd { get; set; }
        public IEnumerable<IMillAndWetdownResultItemParameters> ResultItems { get; set; }
        public IEnumerable<IMillAndWetdownPickedItemParameters> PickedItems { get; set; }
        public int LotSequence { get; set; }
        public string ChileProductKey { get; set; }
    }
}