using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Enumerations;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public interface ISchedulePickOrderItemParameter : IInventoryPickOrderItemKey
    {
        InventoryPickOrderKey PickOrderKey { get; set; }
        CustomerKey CustomerKey { get; set; }
        int ItemSequence { get; set; }
        int ProductId { get; set; }
        int PackagingProductId { get; set; }
        int? TreatmentId { get; set; }
        int Quantity { get; set; }
        ScheduledStatus Status { get; set; }
        string CustomerLotCode { get; set; }
        string CustomerProductCode { get; set; }

        int MatchedFieldsCount { get; set; }
    }

    public class InventoryPickOrderItemParameter : IProductKey, IPackagingProductKey, IInventoryTreatmentKey, ISchedulePickOrderItemParameter
    {
        public int ProductId { get; set; }
        public int PackagingProductId { get; set; }
        public int? TreatmentId { get; set; }
        public int Quantity { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
        public CustomerKey CustomerKey { get; set; }

        InventoryPickOrderKey ISchedulePickOrderItemParameter.PickOrderKey { get; set; }
        int ISchedulePickOrderItemParameter.ItemSequence { get; set; }
        ScheduledStatus ISchedulePickOrderItemParameter.Status { get; set; }
        int ISchedulePickOrderItemParameter.MatchedFieldsCount { get; set; }

        public InventoryPickOrderItemParameter(InventoryPickOrderItem orderItem) : this(orderItem, orderItem, orderItem, orderItem.Quantity, orderItem.Customer, orderItem.CustomerProductCode, orderItem.CustomerLotCode)
        {
            var cast = ((ISchedulePickOrderItemParameter)this);
            cast.PickOrderKey = new InventoryPickOrderKey(orderItem);
            cast.ItemSequence = orderItem.ItemSequence;
        }

        public InventoryPickOrderItemParameter(IProductKey productKey, IPackagingProductKey packagingProductKey, IInventoryTreatmentKey treatmentKey, int quantity, ICustomerKey customer, string customerProductCode, string customerLotCode)
        {
            ProductId = productKey.ProductKey_ProductId;
            PackagingProductId = packagingProductKey.PackagingProductKey_ProductId;
            TreatmentId = treatmentKey == null ? (int?)null : treatmentKey.InventoryTreatmentKey_Id;
            Quantity = quantity;
            CustomerKey = customer != null ? new CustomerKey(customer) : null;
            CustomerProductCode = customerProductCode;
            CustomerLotCode = customerLotCode;
        }

        #region Key Interface Implementations.

        public int ProductKey_ProductId { get { return ProductId; } }
        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }
        public int InventoryTreatmentKey_Id { get { return TreatmentId ?? -1; } }
        public DateTime InventoryPickOrderKey_DateCreated { get { return ((ISchedulePickOrderItemParameter) this).PickOrderKey.InventoryPickOrderKey_DateCreated; } }
        public int InventoryPickOrderKey_Sequence { get { return ((ISchedulePickOrderItemParameter) this).PickOrderKey.InventoryPickOrderKey_Sequence; } }
        public int InventoryPickOrderItemKey_Sequence { get { return ((ISchedulePickOrderItemParameter) this).ItemSequence; } }

        #endregion
    }
}