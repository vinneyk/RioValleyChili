// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class InventoryShipmentOrderProjectors
    {
        internal static Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderKeyReturn>> SelectKey()
        {
            return i => new InventoryShipmentOrderKeyReturn
            {
                InventoryShipmentOrderKey_DateCreated = i.DateCreated,
                InventoryShipmentOrderKey_Sequence = i.Sequence
            };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, ShipmentOrderSummaryReturn>>> SplitSelectSummary()
        {
            var key = SelectKey();
            return new Projectors<InventoryShipmentOrder, ShipmentOrderSummaryReturn>
                {
                    o => new ShipmentOrderSummaryReturn
                        {
                            InventoryShipmentOrderKeyReturn = key.Invoke(o),
                            OrderType = o.OrderType
                        },
                    o => new ShipmentOrderSummaryReturn
                        {
                            Status = o.ShipmentInformation.Status,
                            PalletWeight = o.ShipmentInformation.PalletWeight,
                            PalletQuantity = o.ShipmentInformation.PalletQuantity,
                            TransitInformation = new TransitInformation
                                                    {
                                                        ShipmentMethod = o.ShipmentInformation.ShipmentMethod,
                                                        FreightType = o.ShipmentInformation.FreightBillType,
                                                        DriverName = o.ShipmentInformation.DriverName,
                                                        CarrierName = o.ShipmentInformation.CarrierName,
                                                        TrailerLicenseNumber = o.ShipmentInformation.TrailerLicenseNumber,
                                                        ContainerSeal = o.ShipmentInformation.ContainerSeal
                                                    },
                            ShippingInstructions = new ShippingInstructions
                                                        {
                                                            ShipFromOrSoldToShippingLabel = o.ShipmentInformation.ShipFrom,
                                                            ShipToShippingLabel = o.ShipmentInformation.ShipTo,
                                                            FreightBillToShippingLabel = o.ShipmentInformation.FreightBill,
                                                            ShipmentDate = o.ShipmentInformation.ShipmentDate,
                                                            RequiredDeliveryDateTime = o.ShipmentInformation.RequiredDeliveryDate,
                                                            InternalNotes = o.ShipmentInformation.InternalNotes,
                                                            ExternalNotes = o.ShipmentInformation.ExternalNotes,
                                                            SpecialInstructions = o.ShipmentInformation.SpecialInstructions
                                                        }   
                        },
                    o => new ShipmentOrderSummaryReturn
                        {
                            TotalWeightOrdered = o.InventoryPickOrder.Items.Any() ? o.InventoryPickOrder.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0,
                            TotalWeightPicked = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0.0,
                        }
                };
        }

        internal static Expression<Func<InventoryShipmentOrder, InventoryShipmenOrderSummaryReturn>> SelectInventoryShipmentOrderSummary()
        {
            var pickOrder = InventoryPickOrderProjectors.SelectSummary();
            var pickedInventory = PickedInventoryProjectors.SelectSummary();
            var shipmentInfo = ShipmentInformationProjectors.SelectSummary();

            return SelectShipmentOrderBase().Merge(i => new InventoryShipmenOrderSummaryReturn
            {
                PickOrder = pickOrder.Invoke(i.InventoryPickOrder),
                PickedInventory = pickedInventory.Invoke(i.PickedInventory),
                Shipment = shipmentInfo.Invoke(i.ShipmentInformation)
            });
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderDetailReturn>>> SplitSelectInventoryShipmentOrderDetail(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, InventoryOrderEnum inventoryOrder)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var pickOrder = InventoryPickOrderProjectors.SplitSelectDetails();

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderDetailReturn>
                {
                    { SplitSelectInventoryShipmentOrderDetailBase(inventoryUnitOfWork, currentDate, inventoryOrder), p => p.Translate().To<InventoryShipmentOrderDetailReturn>() },
                    { pickOrder, s => i => new InventoryShipmentOrderDetailReturn
                        {
                            PickOrder = s.Invoke(i.InventoryPickOrder),
                        } }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderDetailBaseReturn>>> SplitSelectInventoryShipmentOrderDetailBase(IInventoryUnitOfWork inventoryUnitOfWork, DateTime currentDate, InventoryOrderEnum inventoryOrder, ISalesUnitOfWork salesUnitOfWork = null)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }

            var pickedInventory = PickedInventoryProjectors.SplitSelectDetail(inventoryUnitOfWork, currentDate, salesUnitOfWork);
            var shipmentInfo = ShipmentInformationProjectors.SelectDetail(inventoryOrder);

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderDetailBaseReturn>
                {
                    SelectShipmentOrderBase().Merge(o => new InventoryShipmentOrderDetailBaseReturn
                        {
                            PurchaseOrderNumber = o.PurchaseOrderNumber,
                            DateOrderReceived = o.DateReceived,
                            OrderRequestedBy = o.RequestedBy,
                            OrderTakenBy = o.TakenBy
                        }),
                    { pickedInventory, s => i => new InventoryShipmentOrderDetailBaseReturn
                        {
                            PickedInventory = s.Invoke(i.PickedInventory)
                        } },
                    i => new InventoryShipmentOrderDetailBaseReturn
                        {
                            Shipment = shipmentInfo.Invoke(i.ShipmentInformation)
                        }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InternalOrderAcknowledgementReturn>>> SplitSelectAcknowledgement(IQueryable<SalesOrder> salesOrders)
        {
            var key = SelectKey();
            var shipment = ShipmentInformationProjectors.SelectShipmentInformation();
            var pickOrderItem = InventoryPickOrderItemProjectors.SplitSelect();
            var customerNotes = CustomerProjectors.SelectNotes();
            var salesOrder = SalesOrderProjectors.SelectWarehouseAcknowlegement();

            return new Projectors<InventoryShipmentOrder, InternalOrderAcknowledgementReturn>
                {
                    o => new InternalOrderAcknowledgementReturn
                        {
                            InventoryShipmentOrderKeyReturn = key.Invoke(o),
                            MovementNumber = o.MoveNum,
                            OrderType = o.OrderType,
                            PurchaseOrderNumber = o.PurchaseOrderNumber,
                            DateReceived = o.DateReceived,
                            RequestedBy = o.RequestedBy,
                            TakenBy = o.TakenBy,
                            OriginFacility = o.SourceFacility.Name
                        },
                    o => new InternalOrderAcknowledgementReturn
                        {
                            ShipmentInformation = shipment.Invoke(o.ShipmentInformation)
                        },
                    { pickOrderItem, p => Projector<InventoryShipmentOrder>.To(o => new InternalOrderAcknowledgementReturn
                        {
                            PickOrderItems = o.InventoryPickOrder.Items.Select(i => p.Invoke(i))
                        }) },
                    o => new InternalOrderAcknowledgementReturn
                        {
                            TotalQuantity = o.InventoryPickOrder.Items.Select(i => i.Quantity).DefaultIfEmpty(0).Sum(),
                            NetWeight = o.InventoryPickOrder.Items.Select(i => i.Quantity * i.PackagingProduct.Weight).DefaultIfEmpty(0).Sum(),
                            TotalGrossWeight = o.InventoryPickOrder.Items.Select(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight)).DefaultIfEmpty(0).Sum(),
                            PalletWeight = o.InventoryPickOrder.Items.Select(i => i.Quantity * (i.PackagingProduct.PalletWeight)).DefaultIfEmpty(0).Sum(),
                            EstimatedShippingWeight = o.InventoryPickOrder.Items.Select(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight + i.PackagingProduct.PalletWeight)).DefaultIfEmpty(0).Sum()
                        },
                    o => new InternalOrderAcknowledgementReturn
                        {
                            CustomerNotes = o.InventoryPickOrder.Items.Select(i => i.Customer).Distinct()
                                .Where(c => c != null && c.Notes.Any())
                                .Select(c => customerNotes.Invoke(c))
                        },
                    o => new InternalOrderAcknowledgementReturn
                        {
                            SalesOrder = salesOrders.Where(c => c.DateCreated == o.DateCreated && c.Sequence == o.Sequence)
                                .Select(c => salesOrder.Invoke(c)).FirstOrDefault()
                        }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderPackingListReturn>>> SplitSelectPackingList()
        {
            var key = SelectKey();
            var item = PickedInventoryItemProjectors.SplitSelectPackingListItem();

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderPackingListReturn>
                {
                    o => new InventoryShipmentOrderPackingListReturn
                        {
                            OrderType = o.OrderType,
                            InventoryShipmentOrderKeyReturn = key.Invoke(o),
                            MovementNumber = o.MoveNum,
                            ShipmentDate = o.ShipmentInformation.ShipmentDate,
                            PurchaseOrderNumber = o.PurchaseOrderNumber,
                            ShipFromOrSoldToShippingLabel = o.ShipmentInformation.ShipFrom,
                            ShipToShippingLabel = o.ShipmentInformation.ShipTo
                        },
                    o => new InventoryShipmentOrderPackingListReturn
                        {
                            TotalQuantity = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity) : 0,
                            PalletQuantity = o.ShipmentInformation.PalletQuantity,
                            PalletWeight = o.ShipmentInformation.PalletWeight,
                            ItemSumPalletWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * (i.PackagingProduct.PalletWeight)) : 0,
                            TotalGrossWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight)) : 0,
                            TotalNetWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0,
                        },
                    { item, p => Projector<InventoryShipmentOrder>.To(o => new InventoryShipmentOrderPackingListReturn
                        {
                            Items = o.PickedInventory.Items.Select(i => p.Invoke(i))
                        }) }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderBillOfLadingReturn>>> SplitSelectBillOfLading()
        {
            var key = SelectKey();
            var item = PickedInventoryItemProjectors.SplitSelectPackingListItem();
            var shipment = ShipmentInformationProjectors.SelectShipmentInformation();

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderBillOfLadingReturn>
                {
                    o => new InventoryShipmentOrderBillOfLadingReturn
                        {
                            OrderType = o.OrderType,
                            SourceFacilityLabelName = o.SourceFacility.ShippingLabelName,
                            SourceFacilityAddress = o.SourceFacility.Address,
                            InventoryShipmentOrderKeyReturn = key.Invoke(o),
                            MoveNum = o.MoveNum,
                            ShipmentInformation = shipment.Invoke(o.ShipmentInformation),
                            PurchaseOrderNumber = o.PurchaseOrderNumber
                        },
                    o => new InventoryShipmentOrderBillOfLadingReturn
                        {
                            TotalQuantity = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity) : 0,
                            PalletWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * (i.PackagingProduct.PalletWeight)) : 0,
                            TotalGrossWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * (i.PackagingProduct.Weight + i.PackagingProduct.PackagingWeight)) : 0,
                            TotalNetWeight = o.PickedInventory.Items.Any() ? o.PickedInventory.Items.Sum(i => i.Quantity * i.PackagingProduct.Weight) : 0,
                        },
                    { item, p => Projector<InventoryShipmentOrder>.To(o => new InventoryShipmentOrderBillOfLadingReturn
                        {
                            Items = o.PickedInventory.Items.Select(i => p.Invoke(i))
                        }) }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderPickSheetReturn>>> SplitSelectPickSheet()
        {
            var key = SelectKey();
            var item = PickedInventoryItemProjectors.SplitSelectPickSheetItem().Merge(); //Necessary to avoid likely EF-Split bug in subsequent group selection. -RI 2015/04/15
            var customerNotes = CustomerProjectors.SelectNotes();
            var shipment = ShipmentInformationProjectors.SelectShipmentInformation();

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderPickSheetReturn>
                {
                    o => new InventoryShipmentOrderPickSheetReturn
                        {
                            OrderType = o.OrderType,
                            InventoryShipmentOrderKeyReturn = key.Invoke(o),
                            MovementNumber = o.MoveNum,
                            PurchaseOrderNumber = o.PurchaseOrderNumber,
                            ShipmentInformation = shipment.Invoke(o.ShipmentInformation)
                        },
                    o => new InventoryShipmentOrderPickSheetReturn
                        {
                            CustomerNotes = o.InventoryPickOrder.Items.Select(i => i.Customer).Distinct()
                                .Where(c => c != null && c.Notes.Any())
                                .Select(c => customerNotes.Invoke(c))
                        },
                    o => new InventoryShipmentOrderPickSheetReturn
                        {
                            Items = o.PickedInventory.Items.Select(i => item.Invoke(i))
                        }
                };
        }

        internal static IEnumerable<Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderCertificateOfAnalysisReturn>>> SplitSelectCertificateOfAnalysisReturn()
        {
            var key = SelectKey();
            var item = PickedInventoryItemProjectors.SplitSelectCOAItem();

            return new Projectors<InventoryShipmentOrder, InventoryShipmentOrderCertificateOfAnalysisReturn>
                {
                    o => new InventoryShipmentOrderCertificateOfAnalysisReturn
                        {
                            OrderKeyReturn = key.Invoke(o),
                            MovementNumber = o.MoveNum,
                            PurchaseOrderNumber = o.PurchaseOrderNumber,
                            ShipmentDate = o.ShipmentInformation.ShipmentDate,
                            OrderType = o.OrderType,
                            DestinationName = new[] { o.DestinationFacility }.Where(f => f != null).Select(f => f.Name).FirstOrDefault()
                        },
                    { item, p => Projector<InventoryShipmentOrder>.To(o => new InventoryShipmentOrderCertificateOfAnalysisReturn
                        {
                            _Items = o.PickedInventory.Items.Where(i => i.Lot.ChileLot != null).Select(i => p.Invoke(i))
                        }) }
                };
        }

        internal static Expression<Func<InventoryShipmentOrder, PendingWarehouseOrderDetail>> SelectPendingWarehouseOrder()
        {
            var orderItemSelect = InventoryPickOrderItemProjectors.SelectPickPending();
            var pickedItemSelect = PickedInventoryItemProjectors.ItemSelect();

            return Projector<InventoryShipmentOrder>.To(o => new PendingWarehouseOrderDetail
            {
                From = o.SourceFacility.Name,
                To = new[] { o.DestinationFacility }.Where(f => f != null).Select(f => f.Name).FirstOrDefault(),
                DateRecd = o.DateReceived,
                ShipmentDate = o.ShipmentInformation.ShipmentDate,
                MoveNum = o.MoveNum,
                Status = o.OrderStatus,

                Items = o.InventoryPickOrder.Items.Select(i => orderItemSelect.Invoke(i)),
                PickedItemSelect = o.PickedInventory.Items.Select(i => pickedItemSelect.Invoke(i))
            });
        }

        private static Expression<Func<InventoryShipmentOrder, InventoryShipmentOrderBaseReturn>> SelectShipmentOrderBase()
        {
            var key = SelectKey();
            var facility = FacilityProjectors.Select(false, true);

            return i => new InventoryShipmentOrderBaseReturn
            {
                InventoryShipmentOrderKeyReturn = key.Invoke(i),

                MoveNum = i.MoveNum,
                OrderStatus = i.OrderStatus,
                DateCreated = i.DateCreated,
                ShipmentDate = i.ShipmentInformation.ShipmentDate,
                OriginFacility = facility.Invoke(i.SourceFacility),
                DestinationFacility = new[] { i.DestinationFacility }.Where(f => f != null).Select(f => facility.Invoke(f)).FirstOrDefault()
            };
        }
    }
}

// ReSharper restore ConvertClosureToMethodGroup