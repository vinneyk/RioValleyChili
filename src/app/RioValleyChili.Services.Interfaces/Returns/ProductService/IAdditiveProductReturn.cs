namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IAdditiveProductReturn : IProductReturn
    {
        string AdditiveTypeDescription { get; }
        string AdditiveTypeKey { get; }
    }
}