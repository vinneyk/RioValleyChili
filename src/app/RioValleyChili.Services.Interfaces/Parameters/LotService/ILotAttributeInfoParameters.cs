using System;

namespace RioValleyChili.Services.Interfaces.Parameters.LotService
{
    public interface ILotAttributeInfoParameters
    {
        double Value { get; }
        DateTime Date { get; }
    }
}