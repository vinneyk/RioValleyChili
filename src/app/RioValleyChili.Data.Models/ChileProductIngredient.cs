using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ChileProductIngredient : EmployeeIdentifiableBase, IChileProductIngredientKey, IChileProductKey, IAdditiveTypeKey
    {
        [Key, Column(Order = 0)]
        public virtual int ChileProductId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int AdditiveTypeId { get; set; }

        public double Percentage { get; set; }

        [ForeignKey("ChileProductId")]
        public ChileProduct ChileProduct { get; set; }
        [ForeignKey("AdditiveTypeId")]
        public AdditiveType AdditiveType { get; set; }

        #region Key Interface Implementations

        #region IChileProductIngredientKey

        public int ChileProductIngredientKey_ChileProductId { get { return ChileProductId; } }
        public int ChileProductIngredientKey_AdditiveTypeId { get { return AdditiveTypeId; } }

        #endregion

        public int ChileProductKey_ProductId { get { return ChileProductId; } }
        public int AdditiveTypeKey_AdditiveTypeId { get { return AdditiveTypeId; } }

        #endregion
    }
}
