using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IChileProductReturn : IProductReturn, IChileTypeSummaryReturn
    {
        ChileStateEnum ChileState { get; }
        string ChileStateName { get; }
    }
}