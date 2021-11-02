using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.Models
{
    public class Facility : IFacilityKey
    {
        public Facility()
        {
            Address = new Address();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual FacilityType FacilityType { get; set; }
        [StringLength(Constants.StringLengths.FacilityName), Required]
        public virtual string Name { get; set; }
        public virtual bool Active { get; set; }

        [StringLength(Constants.StringLengths.ShippingLabelName)]
        public virtual string ShippingLabelName { get; set; }
        [StringLength(Constants.StringLengths.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }
        [StringLength(Constants.StringLengths.Email)]
        public virtual string EMailAddress { get; set; }

        public virtual Address Address { get; set; }

        [Obsolete("For data load/sync purposes. -RI 2015/3/2")]
        public virtual int? WHID { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
        
        #region Implementation of IWarehouseKey

        public int FacilityKey_Id { get { return Id; } }

        #endregion
    }
}
