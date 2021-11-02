using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.ProductService
{
    public interface IRemoveChileProductIngredientParameters : IUserIdentifiable
    {
        string ChileProductKey { get; }
        string AdditiveTypeKey { get; }
    }
}