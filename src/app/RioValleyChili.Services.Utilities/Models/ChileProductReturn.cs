// ReSharper disable RedundantExtendsListEntry

using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileProductReturn : ProductBaseReturn, IChileProductDetailReturn, IChileProductReturn
    {
        #region Summary

        public ChileStateEnum ChileState { get; set; }
        public string ChileTypeDescription { get; set; }

        public string ChileStateName { get { return ChileState.ToChileStateName(); } }
        public string ChileTypeKey { get { return ChileTypeKeyReturn.ChileTypeKey; } }

        #endregion

        #region Detail

        public double? Mesh { get; set; }
        public string IngredientsDescription { get; set; }
        public IEnumerable<IProductAttributeRangeReturn> AttributeRanges { get; set; }
        public IEnumerable<IProductIngredientReturn> ProductIngredients { get; set; }

        #endregion

        internal ChileTypeKeyReturn ChileTypeKeyReturn { get; set; }

        public override LotTypeEnum LotType { get { return ChileState.ToLotType(); } }
    }
}

// ReSharper restore RedundantExtendsListEntry