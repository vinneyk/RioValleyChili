using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class SalesQuoteItem : ISalesQuoteItemKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        [Key, Column(Order = 2)]
        public virtual int ItemSequence { get; set; }

        public virtual int ProductId { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int TreatmentId { get; set; }
        
        public virtual int Quantity { get; set; }
        public virtual double PriceBase { get; set; }
        public virtual double PriceFreight { get; set; }
        public virtual double PriceTreatment { get; set; }
        public virtual double PriceWarehouse { get; set; }
        public virtual double PriceRebate { get; set; }

        [StringLength(Constants.StringLengths.CustomerProductCode)]
        public virtual string CustomerProductCode { get; set; }

        [Obsolete("For data-load/sync purposes. RI - 2016-12-13")]
        public DateTime? QDetailID { get; set; }

        [ForeignKey("DateCreated, Sequence")]
        public virtual SalesQuote SalesQuote { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("TreatmentId")]
        public virtual InventoryTreatment Treatment { get; set; }

        public DateTime SalesQuoteKey_DateCreated { get { return DateCreated; } }
        public int SalesQuoteKey_Sequence { get { return Sequence; } }
        public int SalesQuoteItemKey_ItemSequence { get { return ItemSequence; } }
    }
}