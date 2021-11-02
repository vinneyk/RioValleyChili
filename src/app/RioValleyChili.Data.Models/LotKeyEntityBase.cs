using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public abstract class LotKeyEntityBase : ILotKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime LotDateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int LotDateSequence { get; set; }
        [Key, Column(Order = 2)]
        [Obsolete("Use LotTypeEnum until http://stackoverflow.com/q/12220956 is resolved.")]
        public virtual int LotTypeId
        {
            get { return (int)((LotTypeEnum)_lotType); }
            set { _lotType = (int)((LotTypeEnum)value); }
        }

        [NotMapped]
        public LotTypeEnum LotTypeEnum
        {
            get { return (LotTypeEnum)_lotType; }
            set { _lotType = (int)value; }
        }

        #region Implementation of ILotKey.

        public int LotKey_LotTypeId { get { return LotTypeId; } }
        public DateTime LotKey_DateCreated { get { return LotDateCreated; } }
        public int LotKey_DateSequence { get { return LotDateSequence; } }

        #endregion

        #region Private Parts

        private int _lotType;

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}