using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class AdditiveProduct : IAdditiveProductKey, IAdditiveTypeKey, IProductDerivative
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }
        
        public virtual int AdditiveTypeId { get; set; }

        [ForeignKey("AdditiveTypeId")]
        public virtual AdditiveType AdditiveType { get; set; }
        [ForeignKey("Id")]
        public virtual Product Product { get; set; }

        #region Implementation of Key Interfaces

        public int ProductKey_ProductId { get { return Id; } }
        public int AdditiveProductKey_Id { get { return Id; } }

        #endregion

        public int AdditiveTypeKey_AdditiveTypeId { get { return AdditiveTypeId; } }
    }
}
