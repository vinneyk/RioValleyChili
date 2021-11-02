using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileMaterialsReceivedItemReturn : DehydratedMaterialsReceivedItemBaseReturn, IChileMaterialsReceivedItemReturn
    {
        public string ItemKey { get { return ChileMaterialsReceivedItemKeyReturn.DehydratedMaterialsReceivedItemKey; } }
        public int Quantity { get; internal set; }
        public int TotalWeight { get; internal set; }
        public IPackagingProductReturn PackagingProduct { get; internal set; }
        public ILocationReturn Location { get; internal set; }

        #region Internal Parts

        internal ChileMaterialsReceivedItemKeyReturn ChileMaterialsReceivedItemKeyReturn { get; set; }

        #endregion
    }
}