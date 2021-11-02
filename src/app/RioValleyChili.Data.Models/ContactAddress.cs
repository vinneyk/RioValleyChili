using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.Models
{
    public class ContactAddress : IContactAddressKey
    {
        [Key, Column(Order = 0)]
        public virtual int CompanyId { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ContactId { get; set; }
        [Key, Column(Order = 2)]
        public virtual int AddressId { get; set; }

        [Obsolete("For data load purposes only. -RI 2016-08-22")]
        public virtual int? OldContextID { get; set; }

        [StringLength(Constants.StringLengths.AddressDescription)]
        public virtual string AddressDescription { get; set; }

        public virtual Address Address { get; set; }

        [ForeignKey("CompanyId, ContactId")]
        public virtual Contact Contact { get; set; }

        #region IContactAddressKey

        public virtual int CompanyKey_Id { get { return CompanyId; } }
        public virtual int ContactKey_Id { get { return ContactId; } }
        public virtual int ContactAddressKey_Id { get { return AddressId; } }

        #endregion
    }
}