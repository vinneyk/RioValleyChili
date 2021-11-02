namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IDerivedLot : ILotContainer
    {
        IProduct LotProduct { get; }
    }
}