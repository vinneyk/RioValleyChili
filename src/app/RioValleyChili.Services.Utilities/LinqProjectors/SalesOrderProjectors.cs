// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.LinqPredicates;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SalesOrderProjectors
    {
        internal static Expression<Func<SalesOrder, SalesOrderKeyReturn>> SelectKey()
        {
            return o => new SalesOrderKeyReturn
                {
                    SalesOrderKey_DateCreated = o.DateCreated,
                    SalesOrderKey_Sequence = o.Sequence
                };
        }

        internal static IEnumerable<Expression<Func<SalesOrder, SalesOrderDetailReturn>>> SplitSelectDetail(IInventoryShipmentOrderUnitOfWork inventoryUnitOfWork, DateTime currentDate)
        {
            var company = CompanyProjectors.SelectSummary();
            return new Projectors<SalesOrder, SalesOrderDetailReturn>
                {
                    { InventoryShipmentOrderProjectors.SplitSelectInventoryShipmentOrderDetailBase(inventoryUnitOfWork, currentDate, InventoryOrderEnum.CustomerOrder, inventoryUnitOfWork), p => p.Translate().To<SalesOrder, SalesOrderDetailReturn>(o => o.InventoryShipmentOrder) },
                    o => new SalesOrderDetailReturn
                        {
                            IsMiscellaneous = o.InventoryShipmentOrder.OrderType == InventoryShipmentOrderTypeEnum.MiscellaneousOrder,
                            SalesOrderStatus = o.OrderStatus,
                            PaymentTerms = o.PaymentTerms,
                            ShipFromReplace = o.SoldTo,
                            CreditMemo = o.CreditMemo,
                            InvoiceDate = o.InvoiceDate,
                            InvoiceNotes = o.InvoiceNotes,
                            FreightCharge = o.FreightCharge,

                            Customer = new[] { o.Customer }.Where(c => c != null).Select(c => company.Invoke(c.Company)).FirstOrDefault(),
                            Broker = new[] { o.Broker }.Where(b => b != null).Select(b => company.Invoke(b)).FirstOrDefault()
                        },
                    { SplitSelectPickOrderDetail(), p => o => new SalesOrderDetailReturn
                        {
                            PickOrder = p.Invoke(o)
                        } }
                };
        }

        internal static IEnumerable<Expression<Func<SalesOrder, SalesOrderSummaryReturn>>> SplitSelectSummary()
        {
            var company = CompanyProjectors.SelectSummary();

            return new Projectors<SalesOrder, SalesOrderSummaryReturn>
                {
                    InventoryShipmentOrderProjectors.SelectInventoryShipmentOrderSummary().Translate().To<SalesOrder, SalesOrderSummaryReturn>(c => c.InventoryShipmentOrder),
                    c => new SalesOrderSummaryReturn
                        {
                            SalesOrderStatus = c.OrderStatus,
                            PaymentTerms = c.PaymentTerms,
                            DateOrderReceived = c.InventoryShipmentOrder.DateReceived,
                            InvoiceDate = c.InvoiceDate,
                            CreditMemo = c.CreditMemo,
                            Customer = new[] { c.Customer }.Where(u => u != null).Select(u => company.Invoke(u.Company)).FirstOrDefault(),
                            Broker = new[] { c.Broker }.Where(b => b != null).Select(b => company.Invoke(b)).FirstOrDefault()
                        }
                };
        }

        internal static Expression<Func<SalesOrder, Contract, CustomerContractOrderReturn>> SelectCustomerContractOrder()
        {
            var key = SelectKey();
            var customerOrderItemsPredicate = CustomerOrderItemPredicates.ByContract();
            var contractOrderItems = SalesOrderItemProjectors.SelectContractOrderItem();

            return (order, contract) => new CustomerContractOrderReturn
                {
                    SalesOrderKeyReturn = key.Invoke(order),

                    SalesOrderStatus = order.OrderStatus,
                    ShipmentStatus = order.InventoryShipmentOrder.ShipmentInformation.Status,

                    Items = order.SalesOrderItems.Where(i => customerOrderItemsPredicate.Invoke(i, contract)).Select(i => contractOrderItems.Invoke(i))
                };
        }

        internal static Expression<Func<SalesOrder, SalesOrderInternalAcknowledgementReturn>> SelectWarehouseAcknowlegement()
        {
            var note = CustomerNoteProjectors.Select();
            var orderItemContracKey = SalesOrderItemProjectors.SelectInternalAcknoweldgement();

            return Projector<SalesOrder>.To(o => new SalesOrderInternalAcknowledgementReturn
                {
                    PaymentTerms = o.PaymentTerms,
                    Broker = new[] { o.Broker }.Select(b => b.Name).FirstOrDefault(),
                    SoldToShippingLabel = o.SoldTo,
                    CustomerNotes = new[] { o.Customer }.Where(c => c != null).SelectMany(c => c.Notes.Select(n => note.Invoke(n))),
                    OrderItems = o.SalesOrderItems.Select(i => orderItemContracKey.Invoke(i))
                });
        }

        internal static IEnumerable<Expression<Func<SalesOrder, SalesOrderAcknowledgementReturn>>> SplitSelectAcknowledgement()
        {
            var key = SelectKey();
            var shipment = ShipmentInformationProjectors.SelectShipmentInformation();
            var pickOrderItem = SalesOrderItemProjectors.SplitSelect();

            return new Projectors<SalesOrder, SalesOrderAcknowledgementReturn>
                {
                    o => new SalesOrderAcknowledgementReturn
                        {
                            SalesOrderKeyReturn = key.Invoke(o),
                            MovementNumber = o.InventoryShipmentOrder.MoveNum,
                            
                            PurchaseOrderNumber = o.InventoryShipmentOrder.PurchaseOrderNumber,
                            DateReceived = o.InventoryShipmentOrder.DateReceived,
                            RequestedBy = o.InventoryShipmentOrder.RequestedBy,
                            TakenBy = o.InventoryShipmentOrder.TakenBy,
                            PaymentTerms = o.PaymentTerms,
                            Broker = new[] { o.Broker }.Select(b => b.Name).FirstOrDefault(),
                            SoldToShippingLabel = o.SoldTo,
                            OriginFacility = o.InventoryShipmentOrder.SourceFacility.Name
                        },
                    o => new SalesOrderAcknowledgementReturn
                        {
                            ShipmentInformation = shipment.Invoke(o.InventoryShipmentOrder.ShipmentInformation)
                        },
                    { pickOrderItem, p => Projector<SalesOrder>.To(o => new SalesOrderAcknowledgementReturn
                        {
                            PickOrderItems = o.SalesOrderItems.Select(i => p.Invoke(i))
                        }) },
                    o => new SalesOrderAcknowledgementReturn
                        {
                            TotalQuantity = o.InventoryShipmentOrder.InventoryPickOrder.Items.Any() ? o.InventoryShipmentOrder.InventoryPickOrder.Items.Sum(i => i.Quantity) : 0,
                            NetWeight = o.InventoryShipmentOrder.InventoryPickOrder.Items.Any() ? o.InventoryShipmentOrder.InventoryPickOrder.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0,
                            TotalGrossWeight = o.InventoryShipmentOrder.InventoryPickOrder.Items.Any() ? o.InventoryShipmentOrder.InventoryPickOrder.Items.Sum(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight)) : 0,
                            PalletWeight = o.InventoryShipmentOrder.InventoryPickOrder.Items.Any() ? o.InventoryShipmentOrder.InventoryPickOrder.Items.Sum(i => i.Quantity * (i.PackagingProduct.PalletWeight)) : 0,
                            EstimatedShippingWeight = o.InventoryShipmentOrder.InventoryPickOrder.Items.Any() ? o.InventoryShipmentOrder.InventoryPickOrder.Items.Sum(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight + i.PackagingProduct.PalletWeight)) : 0,
                        }
                };
        }

        internal static Expression<Func<SalesOrder, PendingCustomerOrderDetail>> SelectPendingOrderDetails()
        {
            var item = SalesOrderItemProjectors.SelectPending();

            return Projector<SalesOrder>.To(o => new PendingCustomerOrderDetail
                {
                    Name = new[] { o.Customer }.Where(c => c != null).Select(c => c.Company.Name).FirstOrDefault(),
                    DateRecd = o.InventoryShipmentOrder.DateReceived,
                    ShipmentDate = o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate,
                    Origin = o.InventoryShipmentOrder.SourceFacility.Name,
                    Status = o.InventoryShipmentOrder.OrderStatus,
                    Sample = o.PreShipmentSampleRequired,
                    MoveNum = o.InventoryShipmentOrder.MoveNum,
                    Items = o.SalesOrderItems.Select(i => item.Invoke(i))
                });
        }

        internal static Expression<Func<SalesOrder, SalesOrderInvoiceReturn>> SelectCustomerOrderInvoice()
        {
            var contractKey = ContractProjectors.SelectKey();
            var loBac = LotProjectors.SelectLoBac();
            var salesOrderItemKey = SalesOrderItemProjectors.SelectKey();

            return o => new SalesOrderInvoiceReturn
                {
                    MovementNumber = o.InventoryShipmentOrder.MoveNum,
                    InvoiceDateReturn = o.InvoiceDate,
                    PONumber = o.InventoryShipmentOrder.PurchaseOrderNumber,
                    FreightCharge = o.FreightCharge,
                    InvoiceNotes = o.InvoiceNotes,
                    CreditMemo = o.CreditMemo,

                    Origin = o.InventoryShipmentOrder.SourceFacility.Name,
                    ShipDate = o.InventoryShipmentOrder.ShipmentInformation.ShipmentDate,
                    Broker = new[] { o.Broker }.Where(b => b != null).Select(b => b.Name).FirstOrDefault(),
                    PaymentTerms = o.PaymentTerms,
                    Freight = o.InventoryShipmentOrder.ShipmentInformation.FreightBillType,
                    ShipVia = o.InventoryShipmentOrder.ShipmentInformation.ShipmentMethod,

                    SoldTo = o.SoldTo,
                    ShipTo = o.InventoryShipmentOrder.ShipmentInformation.ShipTo,

                    PickedItems = o.SalesOrderPickedItems.Select(i => new SalesOrderInvoicePickedItemReturn
                        {
                            SalesOrderItemKeyReturn = salesOrderItemKey.Invoke(i.SalesOrderItem),
                            ProductCode = i.PickedInventoryItem.Lot.ChileLot.ChileProduct.Product.ProductCode,
                            ProductName = i.PickedInventoryItem.Lot.ChileLot.ChileProduct.Product.Name,
                            ProductType = i.PickedInventoryItem.Lot.ChileLot.ChileProduct.ChileType.Description,

                            CustomerProductCode = i.PickedInventoryItem.CustomerProductCode,
                            PackagingName = i.PickedInventoryItem.PackagingProduct.Product.Name,
                            TreatmentNameShort = i.PickedInventoryItem.Treatment.ShortName,
                            QuantityShipped = i.PickedInventoryItem.Quantity,
                            NetWeight = i.PickedInventoryItem.PackagingProduct.Weight * i.PickedInventoryItem.Quantity,
                            LoBac = loBac.Invoke(i.PickedInventoryItem.Lot)
                        }),
                    OrderItems = o.SalesOrderItems.Select(i => new SalesOrderInvoiceOrderItemReturn
                        {
                            ProductCode = i.InventoryPickOrderItem.Product.ProductCode,
                            ProductName = i.InventoryPickOrderItem.Product.Name,
                            PackagingName = i.InventoryPickOrderItem.PackagingProduct.Product.Name,
                            NetWeight = i.InventoryPickOrderItem.PackagingProduct.Weight * i.InventoryPickOrderItem.Quantity,

                            SalesOrderItemKeyReturn = salesOrderItemKey.Invoke(i),
                            ContractKeyReturn = new[] { i.ContractItem }.Where(n => n != null).Select(n => contractKey.Invoke(n.Contract)).FirstOrDefault(),
                            ContractId = new[] { i.ContractItem }.Where(n => n != null).Select(n => n.Contract.ContractId).FirstOrDefault(),
                            QuantityOrdered = i.InventoryPickOrderItem.Quantity,

                            PriceBase = i.PriceBase,
                            PriceFreight = i.PriceFreight,
                            PriceTreatment = i.PriceTreatment,
                            PriceWarehouse = i.PriceWarehouse,
                            PriceRebate = i.PriceRebate
                        })
                };
        }

        private static IEnumerable<Expression<Func<SalesOrder, CustomerPickOrderReturn>>> SplitSelectPickOrderDetail()
        {
            var key = InventoryPickOrderProjectors.SelectKey();

            return new Projectors<SalesOrder, CustomerPickOrderReturn>
                {
                    o => new CustomerPickOrderReturn
                        {
                            InventoryPickOrderKeyReturn = key.Invoke(o.InventoryShipmentOrder.InventoryPickOrder)
                        },
                    { SalesOrderItemProjectors.SplitSelect(), p => o => new CustomerPickOrderReturn
                        {
                            PickOrderItems = o.SalesOrderItems.Select(i => p.Invoke(i))
                        } }
                };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup