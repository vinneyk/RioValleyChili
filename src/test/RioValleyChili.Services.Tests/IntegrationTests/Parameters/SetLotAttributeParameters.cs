using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class SetLotAttributeParameters : ISetLotAttributeParameters
    {
        public string UserToken { get; set; }
        public string LotKey { get; set; }
        public string Notes { get; set; }
        public IDictionary<string, IAttributeValueParameters> Attributes { get; set; }
        public bool OverrideOldContextLotAsCompleted { get; set; }
    }

    public class AttributeValueParameters : IAttributeValueParameters
    {
        public ILotAttributeInfoParameters AttributeInfo { get; set; }
        public IDefectResolutionParameters Resolution { get; set; }
    }

    public class DefectResolutionParameters : IDefectResolutionParameters
    {
        public ResolutionTypeEnum ResolutionType { get; set; }
        public string Description { get; set; }
    }

    public class AttributeInfoParameters : ILotAttributeInfoParameters
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }
}