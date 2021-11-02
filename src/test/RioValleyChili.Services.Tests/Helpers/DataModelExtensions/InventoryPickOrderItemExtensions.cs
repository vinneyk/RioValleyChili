using System;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.Tests.Helpers.DataModelExtensions
{
    internal static class InventoryPickOrderItemExtensions
    {
        internal static InventoryPickOrderItem SetProduct(this InventoryPickOrderItem item, IProductKey productKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(productKey != null)
            {
                item.Product = null;
                item.ProductId = productKey.ProductKey_ProductId;
            }
            
            return item;
        }

        internal static InventoryPickOrderItem SetOrder(this InventoryPickOrderItem item, IInventoryPickOrderKey pickOrderKey)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(pickOrderKey != null)
            {
                item.InventoryPickOrder = null;
                item.DateCreated = pickOrderKey.InventoryPickOrderKey_DateCreated;
                item.OrderSequence = pickOrderKey.InventoryPickOrderKey_Sequence;
            }

            return item;
        }

        internal static InventoryPickOrderItem SetCustomer(this InventoryPickOrderItem item, ICustomerKey customer)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(customer != null)
            {
                item.Customer = null;
                item.CustomerId = customer == null ? (int?) null : customer.CustomerKey_Id;
            }

            return item;
        }

        internal static InventoryPickOrderItem SetPackaging(this InventoryPickOrderItem item, IPackagingProductKey packaging)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.PackagingProduct = null;
            item.PackagingProductId = packaging.PackagingProductKey_ProductId;

            return item;
        }

        internal static InventoryPickOrderItem SetTreatment(this InventoryPickOrderItem item, IInventoryTreatmentKey treatment)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            item.InventoryTreatment = null;
            item.TreatmentId = treatment.InventoryTreatmentKey_Id;

            return item;
        }
    }
}