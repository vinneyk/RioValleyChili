using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class ChileProduct : IChileProductKey, IChileTypeKey, IProductDerivative
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual int ChileTypeId { get; set; }
        public virtual ChileStateEnum ChileState { get; set; }
        public virtual double? Mesh { get; set; }
        public virtual string IngredientsDescription { get; set; }

        #region Navigational Properties
        
        [ForeignKey("ChileTypeId")]
        public virtual ChileType ChileType { get; set; }
        [ForeignKey("Id")]
        public virtual Product Product { get; set; }

        public virtual ICollection<ChileProductIngredient> Ingredients { get; set; }
        public virtual ICollection<ChileProductAttributeRange> ProductAttributeRanges { get; set; }
        public virtual ICollection<CustomerProductAttributeRange> CustomerProductAttributeRanges { get; set; }

        #endregion

        #region #region Key Interface Implementations

        public int ProductKey_ProductId { get { return Id; } }
        public int ChileProductKey_ProductId { get { return Id; } }
        public int ChileTypeKey_ChileTypeId { get { return ChileTypeId; } }

        #endregion
    }
}