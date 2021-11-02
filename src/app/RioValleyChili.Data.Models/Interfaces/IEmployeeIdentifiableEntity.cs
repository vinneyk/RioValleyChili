using System;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IEmployeeIdentifiableEntity : IEmployeeKey
    {
        DateTime TimeStamp { get; }
    }
}