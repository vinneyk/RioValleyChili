using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Contact : IContactKey
    {
        [Key, Column(Order = 0)]
        public virtual int CompanyId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ContactId { get; set; }

        [StringLength(Constants.StringLengths.ContactName)]
        public virtual string Name { get; set; }
        [StringLength(Constants.StringLengths.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }
        [StringLength(Constants.StringLengths.Email)]
        public virtual string EMailAddress { get; set; }

        [Obsolete("For data load purposes only. -RI 2016-08-23")]
        public int? OldContextID { get; set; }

        public virtual ICollection<ContactAddress> Addresses { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        #region IContactKey

        public int CompanyKey_Id { get { return CompanyId; } }
        public int ContactKey_Id { get { return ContactId; } }

        #endregion
    }
}