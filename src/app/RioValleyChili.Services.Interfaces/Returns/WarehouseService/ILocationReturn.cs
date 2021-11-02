using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Services.Interfaces.Returns.WarehouseService
{
    public interface ILocationReturn : IFacilitySummaryReturn, ILocationDescription
    {
        LocationStatus? Status { get; }
    }
}