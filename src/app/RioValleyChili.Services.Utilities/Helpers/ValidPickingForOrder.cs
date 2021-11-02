using RioValleyChili.Core.Attributes;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Models;

namespace RioValleyChili.Services.Utilities.Helpers
{
    internal class ValidPickingForOrder : IPickableInventoryInitializer
    {
        internal ValidPickingForOrder(PickingValidatorContext validatorContext)
        {
            _context = validatorContext;
        }

        private readonly PickingValidatorContext _context;

        [Issue("Client expressed desire to see inventory that was still invalid to pick in some cases." +
               "Implemented by using some validation rules to intialize ValidToPick flag during projection instead of filtering Inventory," +
               "and taking that into consideration in IPickableInventoryInitializer implementations. - RI 2016-10-05",
               References = new[] { "RVCADMIN-1332" })]
        void IPickableInventoryInitializer.Initialize(IPickableInventorySummaryReturn inventory)
        {
            var pickableInventory = (PickableInventoryItemReturn) inventory;
            var validator = new PickingValidator(pickableInventory);
            pickableInventory.ValidForPicking = pickableInventory.ValidForPicking && validator.ValidForPicking(_context).Success;
        }
    }

    internal class DoNothingValidator : IPickableInventoryInitializer
    {
        public void Initialize(IPickableInventorySummaryReturn inventory) { }
    }
}