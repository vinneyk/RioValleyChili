using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    [Table("Products")]
    public class Product : IProductKey, IProduct
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        [StringLength(150)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        public ProductTypeEnum ProductType { get; set; }

        /// <summary>
        /// Rio Valley Chili defined code
        /// </summary>
        public virtual string ProductCode { get; set; }

        #region Implementation of IProductKey

        public int ProductKey_ProductId { get { return Id; } }

        #endregion
    }
}