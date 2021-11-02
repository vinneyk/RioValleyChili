using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public class LotAttributeDefect : LotKeyEntityBase, ILotAttributeDefectKey, IAttributeDefect
    {
        [Key, Column(Order = 3)]
        public virtual int DefectId { get; set; }
        [Key, Column(Order = 4), StringLength(Constants.StringLengths.AttributeShortName)]
        public virtual string AttributeShortName { get; set; }

        public virtual double OriginalAttributeValue { get; set; }
        public virtual double OriginalAttributeMinLimit { get; set; }
        public virtual double OriginalAttributeMaxLimit { get; set; }

        #region Navigational Properties

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId, DefectId")]
        public virtual LotDefect LotDefect { get; set; }
        [ForeignKey("AttributeShortName")]
        public virtual AttributeName AttributeName { get; set; }

        #endregion

        #region Key Interface Implementations

        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }
        public int LotKey_DateSequence { get { return LotDateSequence; } }
        public int LotKey_LotTypeId { get { return LotTypeId; } }
        public int LotDefectKey_Id { get { return DefectId; } }
        public string AttributeNameKey_ShortName { get { return AttributeShortName; } }

        #endregion

        #region IAttributeDefect

        double IAttributeDefect.Value { get { return OriginalAttributeValue; } }
        double IAttributeRange.RangeMin { get { return OriginalAttributeMinLimit; } }
        double IAttributeRange.RangeMax { get { return OriginalAttributeMaxLimit; } }
        DefectTypeEnum IAttributeDefect.DefectType { get { return LotDefect.DefectType; } }
        bool IAttributeDefect.HasResolution { get { return LotDefect.Resolution != null; } }

        #endregion
    }
}