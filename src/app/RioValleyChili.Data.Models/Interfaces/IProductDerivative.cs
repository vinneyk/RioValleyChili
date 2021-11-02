using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IProductDerivative : IProductKey
    {
        int Id { get; set; }

        Product Product { get; set; }
    }
}