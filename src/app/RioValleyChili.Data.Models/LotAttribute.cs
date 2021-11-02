// ReSharper disable RedundantExtendsListEntry

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotAttribute : LotKeyEmployeeIdentifiableBase, ILotAttributeKey, IAttributeNameKey
    {
        [Key, Column(Order = 3), StringLength(Constants.StringLengths.AttributeShortName)]
        public virtual string AttributeShortName { get; set; }
        
        public virtual double AttributeValue { get; set; }

        [Index, Column(TypeName = "Date")]
        public virtual DateTime AttributeDate { get; set; }

        public virtual bool Computed { get; set; }

        [ForeignKey("AttributeShortName")]
        public virtual AttributeName AttributeName { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }

        #region Implementation of ILotAttributeKey

        public string AttributeNameKey_ShortName { get { return AttributeShortName; } }

        #endregion
    }
}

// ReSharper restore RedundantExtendsListEntry