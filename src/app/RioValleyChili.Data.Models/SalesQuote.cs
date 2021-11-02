using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.Models
{
    public class SalesQuote : EmployeeIdentifiableBase, ISalesQuoteKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual int? QuoteNum { get; set; }
        public virtual DateTime QuoteDate { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime? DateReceived { get; set; }
        public virtual string CalledBy { get; set; }
        public virtual string TakenBy { get; set; }
        [StringLength(Constants.StringLengths.PaymentTerms)]
        public virtual string PaymentTerms { get; set; }

        public virtual int? SourceFacilityId { get; set; }
        public virtual int? CustomerId { get; set; }
        public virtual int? BrokerId { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime ShipmentInfoDateCreated { get; set; }
        public virtual int ShipmentInfoSequence { get; set; }

        public virtual ShippingLabel SoldTo { get; set; }

        [ForeignKey("SourceFacilityId")]
        public virtual Facility SourceFacility { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }
        [ForeignKey("ShipmentInfoDateCreated, ShipmentInfoSequence")]
        public virtual ShipmentInformation ShipmentInformation { get; set; }

        public virtual ICollection<SalesQuoteItem> Items { get; set; }

        public DateTime SalesQuoteKey_DateCreated { get { return DateCreated; } }
        public int SalesQuoteKey_Sequence { get { return Sequence; } }

        public SalesQuote()
        {
            SoldTo = new ShippingLabel();
        }
    }
}