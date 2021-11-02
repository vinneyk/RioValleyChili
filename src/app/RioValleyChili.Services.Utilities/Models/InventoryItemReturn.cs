// ReSharper disable RedundantExtendsListEntry

using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryItemReturn : LotSummaryReturn, IInventorySummaryReturn
    {
        public string InventoryKey { get { return InventoryKeyReturn.InventoryKey; } }
        public string ToteKey { get; internal set; }
        public int Quantity { get; internal set; }

        public IPackagingProductReturn PackagingReceived { get; internal set; }
        public IPackagingProductReturn PackagingProduct { get; internal set; }
        public ILocationReturn Location { get; internal set; }
        public IInventoryTreatmentReturn InventoryTreatment { get; internal set; }

        internal InventoryKeyReturn InventoryKeyReturn { get; set; }
    }

    internal class PickableInventoryItemReturn : InventoryItemReturn, IPickableInventorySummaryReturn
    {
        public bool ValidForPicking { get; internal set; }

        internal IEnumerable<ContractKeyReturn> ContractAllowances { get; set; }
        internal IEnumerable<SalesOrderKeyReturn> CustomerOrderAllowances { get; set; }
        internal IEnumerable<CustomerKeyReturn> CustomerAllowances { get; set; }
    }

    internal class PickableInventoryReturn : IPickableInventoryReturn
    {
        public IQueryable<IPickableInventorySummaryReturn> Items { get; set; }
        public IPickableInventoryInitializer Initializer { get; set; }
    }
}

// ReSharper restore RedundantExtendsListEntry