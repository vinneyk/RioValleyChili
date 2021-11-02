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
    public class SampleOrder : EmployeeIdentifiableBase, ISampleOrderKey
    {
        [Key, Column(Order = 0)]
        public virtual int Year { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }
        
        [Column(TypeName="date")]
        public virtual DateTime DateDue { get; set; }
        [Column(TypeName="date")]
        public virtual DateTime DateReceived { get; set; }
        [Column(TypeName = "date")]
        public virtual DateTime? DateCompleted { get; set; }
        
        public virtual SampleOrderStatus Status { get; set; }
        public virtual bool Active { get; set; }

        public virtual string Comments { get; set; }
        public virtual string PrintNotes { get; set; }
        public virtual double Volume { get; set; }

        [StringLength(Constants.StringLengths.ShipmentMethod)]
        public virtual string ShipmentMethod { get; set; }
        [StringLength(Constants.StringLengths.FOB)]
        public virtual string FOB { get; set; }

        [StringLength(Constants.StringLengths.CompanyName)]
        public virtual string ShipToCompany { get; set; }
        public virtual ShippingLabel ShipTo { get; set; }
        public virtual ShippingLabel Request { get; set; }

        public virtual int? RequestCustomerId { get; set; }
        public virtual int? BrokerId { get; set; }

        [ForeignKey("RequestCustomerId")]
        public virtual Customer RequestCustomer { get; set; }
        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }

        public virtual ICollection<SampleOrderJournalEntry> JournalEntries { get; set; }
        public virtual ICollection<SampleOrderItem> Items { get; set; }

        [Obsolete("For referencing old context. - RI 2016/9/13")]
        public int? SampleID { get; set; }

        int ISampleOrderKey.SampleOrderKey_Year { get { return Year; } }
        int ISampleOrderKey.SampleOrderKey_Sequence { get { return Sequence; } }
    }
}