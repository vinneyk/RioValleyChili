using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.ChileProductIngredients),
    Obsolete("Use SyncProduct instead. -RI 2016-09-05")]
    public class SyncChileProductIngredients : SyncCommandBase<IProductUnitOfWork, SyncProductParameters>
    {
        public SyncChileProductIngredients(IProductUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<SyncProductParameters> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }

            var parameters = getInput();
            var chileProduct = UnitOfWork.ChileProductRepository.FindByKey(parameters.ProductKey,
                c => c.Product,
                c => c.Ingredients);

            var syncIngredients = new SyncChileProductHelper(OldContext);
            var prodId = int.Parse(chileProduct.Product.ProductCode);

            var oldProduct = syncIngredients.GetProduct(prodId);
            syncIngredients.SyncIngredients(chileProduct, oldProduct, parameters.DeletedIngredients);

            OldContext.SaveChanges();

            Console.Write(ConsoleOutput.SynchedTblProduct, prodId);
        }
    }
}