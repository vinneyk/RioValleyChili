using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class AdditiveProductReturn : ProductBaseReturn, IAdditiveTypeKey, IAdditiveProductReturn
    {
        internal int AdditiveTypeId { get; set; }

        #region Implementation of IAdditiveProduct

        public string AdditiveTypeDescription { get; set; }

        public string AdditiveTypeKey { get { return this.ToAdditiveTypeKey(); } }

        #endregion

        #region Implementation of IAdditiveTypeKey

        int IAdditiveTypeKey.AdditiveTypeKey_AdditiveTypeId { get { return AdditiveTypeId; } }

        #endregion

        public override LotTypeEnum LotType { get { return LotTypeEnum.Additive;} }
    }
}