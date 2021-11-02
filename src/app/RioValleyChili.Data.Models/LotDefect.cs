// ReSharper disable RedundantExtendsListEntry
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotDefect : LotKeyEntityBase, ILotDefectKey, ILotDefectResolutionKey
    {
        [Key, Column(Order = 3)]
        public virtual int DefectId { get; set; }

        public virtual DefectTypeEnum DefectType { get; set; }
        [StringLength(50)]
        public virtual string Description { get; set; }

        #region Navigational Properties

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId")]
        public virtual Lot Lot { get; set; }
        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId, DefectId")]
        public virtual LotDefectResolution Resolution { get; set; }

        #endregion

        #region Key Interface Implementations

        public int LotDefectKey_Id { get { return DefectId; } }

        #endregion
    }
}

// ReSharper restore RedundantExtendsListEntry