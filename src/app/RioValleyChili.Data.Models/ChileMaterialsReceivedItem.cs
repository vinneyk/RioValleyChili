using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ChileMaterialsReceivedItem : LotKeyEntityBase, IChileMaterialsReceivedItemKey, IPackagingProductKey, ILocationKey
    {
        [Key, Column(Order = 3)]
        public virtual int ItemSequence { get; set; }

        [StringLength(Constants.StringLengths.GrowerCode)]
        public virtual string GrowerCode { get; set; }
        [StringLength(Constants.StringLengths.ToteKeyLength)]
        public virtual string ToteKey { get; set; }
        [Index(IsUnique = false), StringLength(Constants.StringLengths.ChileVariety)]
        public virtual string ChileVariety { get; set; }

        public virtual int Quantity { get; set; }
        public virtual int LocationId { get; set; }
        public virtual int PackagingProductId { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual ChileMaterialsReceived ChileMaterialsReceived { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }

        #region Key Interface Implementations

        #region IDehydratedMaterialsReceivedItemKey

        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }
        public int LotKey_DateSequence { get { return LotDateSequence; } }
        public int LotKey_LotTypeId { get { return LotTypeId; } }
        public int ChileMaterialsReceivedKey_ItemSequence { get { return ItemSequence; } }

        #endregion
        
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int LocationKey_Id { get { return LocationId; } }

        #endregion
    }
}