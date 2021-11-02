// ReSharper disable RedundantExtendsListEntry

using Solutionhead.Data;

namespace RioValleyChili.Data.Interfaces.UnitsOfWork
{
    public interface IInventoryShipmentOrderUnitOfWork : IUnitOfWork,
        ITreatmentOrderUnitOfWork,
        ISalesUnitOfWork
    { }
}

// ReSharper restore RedundantExtendsListEntry