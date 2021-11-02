using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class ContractItem : IContractItemKey, IChileProductKey , IPackagingProductKey, IInventoryTreatmentKey
    {
        [Key, Column(Order = 0)]
        public virtual int ContractYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ContractSequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ContractItemSequence { get; set; }

        public virtual int ChileProductId { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int TreatmentId { get; set; }

        public virtual bool UseCustomerSpec { get; set; }
        public virtual int Quantity { get; set; }
        public virtual double PriceBase { get; set; }
        public virtual double PriceFreight { get; set; }
        public virtual double PriceTreatment { get; set; }
        public virtual double PriceWarehouse { get; set; }
        public virtual double PriceRebate { get; set; }

        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public virtual string CustomerProductCode { get; set; }

        [Obsolete("For data-load/sync purposes. RI - 2014/11/18")]
        public DateTime? KDetailID { get; set; }

        [ForeignKey("ContractYear, ContractSequence")]
        public virtual Contract Contract { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }

        public virtual ICollection<SalesOrderItem> OrderItems { get; set; }

        #region Key Interface Implementations

        #region IContractKey

        public int ContractKey_Year { get { return ContractYear; } }
        public int ContractKey_Sequence { get { return ContractSequence; } }

        #endregion

        public int ContractItemKey_Sequence { get { return ContractItemSequence; } }
        public int ChileProductKey_ProductId { get { return ChileProductId; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}