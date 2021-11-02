using System;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class AttributeInfoParameters : ILotAttributeInfoParameters
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }
}