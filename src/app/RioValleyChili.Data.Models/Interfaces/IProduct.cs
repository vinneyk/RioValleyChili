using RioValleyChili.Core;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IProduct 
    {
        int Id { get; set; }
        string Name { get; }
        bool IsActive { get; }
        string ProductCode { get; }
        ProductTypeEnum ProductType { get; }
    }
}