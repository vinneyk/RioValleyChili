using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class LotDefectResolution : LotKeyEmployeeIdentifiableBase, ILotDefectResolutionKey
    {
        [Key, Column(Order = 3)]
        public virtual int DefectId { get; set; }

        public virtual ResolutionTypeEnum ResolutionType { get; set; }

        [Required(AllowEmptyStrings = true), MaxLength]
        public virtual string Description { get; set; }

        [ForeignKey("LotDateCreated, LotDateSequence, LotTypeId, DefectId")]
        public virtual LotDefect Defect { get; set; }

        #region Implementation of ILotDefectResolutionKey

        public int LotDefectKey_Id { get { return DefectId; } }

        #endregion
    }
}