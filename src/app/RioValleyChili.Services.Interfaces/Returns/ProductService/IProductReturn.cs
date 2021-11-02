using RioValleyChili.Core;

namespace RioValleyChili.Services.Interfaces.Returns.ProductService
{
    public interface IProductReturn : IProductKeyNameReturn
    {
        string ProductNameFull { get; }
        string ProductCode { get; }
        bool IsActive { get; }
        string ProductCodeAndName { get; }
        ProductTypeEnum ProductType { get; }
        LotTypeEnum LotType { get; }
    }
}
