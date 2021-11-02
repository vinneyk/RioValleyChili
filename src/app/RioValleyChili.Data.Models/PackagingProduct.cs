using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class PackagingProduct : IPackagingProductKey, IProductDerivative
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual double Weight { get; set; }
        public virtual double PackagingWeight { get; set; }
        public virtual double PalletWeight { get; set; }
        
        [ForeignKey("Id")]
        public virtual Product Product { get; set; }

        #region Implementation of IProductKey

        public int ProductKey_ProductId { get { return Id; } }

        #endregion

        #region Implementation of IPackagingProductKey

        public int PackagingProductKey_ProductId { get { return Id; } }

        #endregion

    }
}