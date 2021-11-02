using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class SalesOrder : ISalesOrderKey, IInventoryShipmentOrderKey, ICustomerKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int Sequence { get; set; }

        public virtual int? CustomerId { get; set; }
        public virtual int? BrokerId { get; set; }

        public virtual SalesOrderStatus OrderStatus { get; set; }
        [StringLength(Constants.StringLengths.PaymentTerms)]
        public virtual string PaymentTerms { get; set; }
        public virtual bool PreShipmentSampleRequired { get; set; }
        public virtual ShippingLabel SoldTo { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime? InvoiceDate { get; set; }
        [StringLength(Constants.StringLengths.InvoiceNotes)]
        public virtual string InvoiceNotes { get; set; }
        public virtual bool CreditMemo { get; set; }
        public virtual float FreightCharge { get; set; }

        [ForeignKey("DateCreated, Sequence")]
        public virtual InventoryShipmentOrder InventoryShipmentOrder { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }

        /// <summary>
        /// The items to be fulfilled for the order.
        /// </summary>
        public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; }

        /// <summary>
        /// References to the actual inventory items picked for the order.
        /// </summary>
        public virtual ICollection<SalesOrderPickedItem> SalesOrderPickedItems { get; set; }

        public virtual ICollection<LotSalesOrderAllowance> LotAllowances { get; set; }

        #region Key Interface Implementations.

        #region ICustomerOrderKey.

        public DateTime SalesOrderKey_DateCreated { get { return DateCreated; } }
        public int SalesOrderKey_Sequence { get { return Sequence; } }

        #endregion

        #region IInventoryShipmentOrderKey

        public DateTime InventoryShipmentOrderKey_DateCreated { get { return DateCreated; } }
        public int InventoryShipmentOrderKey_Sequence { get { return Sequence; } }

        #endregion
        
        public int CustomerKey_Id { get { return CustomerId.Value; } }

        #endregion

        public SalesOrder()
        {
            SoldTo = new ShippingLabel();
        }

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}