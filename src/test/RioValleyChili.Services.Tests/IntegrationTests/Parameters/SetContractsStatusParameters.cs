﻿using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetContractsStatusParameters : ISetContractsStatusParameters
    {
        public ContractStatus ContractStatus { get; set; }
        public IEnumerable<string> ContractKeys { get; set; }
    }
}