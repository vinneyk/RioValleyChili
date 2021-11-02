using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class AttributeName : IAttributeNameKey
    {
        [Key, StringLength(Constants.StringLengths.AttributeShortName)]
        public virtual string ShortName { get; set; }

        [StringLength(25)]
        public virtual string Name { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool ActualValueRequired { get; set; }
        public virtual DefectTypeEnum DefectType { get; set; }
        public virtual double? LoBacLimit { get; set; }

        public virtual bool ValidForChileInventory { get; set; }
        public virtual bool ValidForAdditiveInventory { get; set; }
        public virtual bool ValidForPackagingInventory { get; set; }

        public ICollection<InventoryTreatmentForAttribute> ValidTreatments { get; set; }

        #region IAttributeNameKey implementation.

        public string AttributeNameKey_ShortName { get { return ShortName; } }

        #endregion
    }
}