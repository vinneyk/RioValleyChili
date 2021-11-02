using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class SalesOrderEntityObjectMother : EntityMotherLogBase<SalesOrderEntityObjectMother.SalesOrderLoad, SalesOrderEntityObjectMother.CallbackParameters, SalesOrderEntityObjectMother.EntityType>
    {
        public SalesOrderEntityObjectMother(RioValleyChiliDataContext newContext, ObjectContext oldContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            _newContextHelper = new NewContextHelper(newContext);
        }

        private readonly SalesOrderKey _salesOrderKey = new SalesOrderKey();
        private readonly NewContextHelper _newContextHelper;

        protected override IEnumerable<SalesOrderLoad> BirthRecords()
        {
            LoadCount.Reset();

            foreach(var oldOrder in SelectOrdersToLoad())
            {
                var order = CreateCustomerOrder(oldOrder);
                if(order != null)
                {
                    if(IsOldAndEmpty(order))
                    {
                        Log(new CallbackParameters(CallbackReason.OldAndEmpty) { Order = oldOrder });
                        continue;
                    }

                    LoadCount.AddLoaded(EntityType.SalesOrder);
                    if(order.SalesOrderItems != null)
                    {
                        LoadCount.AddLoaded(EntityType.SalesOrderItem, (uint) order.SalesOrderItems.Count);
                    }
                    if(order.SalesOrderPickedItems != null)
                    {
                        LoadCount.AddLoaded(EntityType.SalesOrderPickedItem, (uint) order.SalesOrderPickedItems.Count);
                    }

                    yield return order;
                }
            }
        }

        private static bool IsOldAndEmpty(SalesOrderLoad order)
        {
            return order.InventoryShipmentOrder.OrderType == InventoryShipmentOrderTypeEnum.SalesOrder &&
                order.DateCreated <= DateTime.UtcNow.AddMonths(-6) &&
                (order.SalesOrderPickedItems == null || !order.SalesOrderPickedItems.Any());
        }

        private SalesOrderLoad CreateCustomerOrder(tblOrderDTO oldOrder)
        {
            LoadCount.AddRead(EntityType.SalesOrder);

            if(oldOrder.EntryDate == null)
            {
                Log(new CallbackParameters(CallbackReason.EntryDateNull) { Order = oldOrder });
                return null;
            }
            
            var employeeId = _newContextHelper.DefaultEmployee.EmployeeId;
            if(oldOrder.EmployeeID != null)
            {
                employeeId = oldOrder.EmployeeID.Value;
            }
            else
            {
                Log(new CallbackParameters(CallbackReason.EmployeeIDNull) { Order = oldOrder });
            }

            Models.Company customer = null;
            if(!string.IsNullOrWhiteSpace(oldOrder.Company_IA))
            {
                customer = _newContextHelper.GetCompany(oldOrder.Company_IA, CompanyType.Customer);
                if(customer == null)
                {
                    Log(new CallbackParameters(CallbackReason.CustomerNotFound) { Order = oldOrder });
                    return null;
                }
            }

            Models.Company broker = null;
            if(!string.IsNullOrWhiteSpace(oldOrder.Broker))
            {
                broker = _newContextHelper.GetCompany(oldOrder.Broker, CompanyType.Broker);
                if(broker == null)
                {
                    Log(new CallbackParameters(CallbackReason.BrokerNotFound) { Order = oldOrder });
                    return null;
                }
            }
            
            var warehouse = _newContextHelper.GetFacility(oldOrder.WHID);
            if(warehouse == null)
            {
                Log(new CallbackParameters(CallbackReason.WarehouseNotFound) { Order = oldOrder });
                return null;
            }

            var dateReceived = oldOrder.EntryDate.Value.Date;
            if(oldOrder.DateRecd != null)
            {
                dateReceived = oldOrder.DateRecd.Value.Date;
            }
            else
            {
                Log(new CallbackParameters(CallbackReason.DateRecdNull) { Order = oldOrder });
            }

            if(oldOrder.Status.ToTblOrderStatus() == null)
            {
                Log(new CallbackParameters(CallbackReason.InvalidStatus)
                    {
                        Order = oldOrder
                    });
                return null;
            }

            var shipmentInformation = CreateShipmentInformation(oldOrder);
            if(shipmentInformation == null)
            {
                return null;
            }

            var timestamp = oldOrder.EntryDate.Value.ConvertLocalToUTC();
            var order = SetItems(new SalesOrderLoad
                {
                    CustomerId = customer == null ? (int?) null : customer.Id,
                    BrokerId = broker == null ? (int?) null : broker.Id,
                    PaymentTerms = oldOrder.PayTerms,
                    PreShipmentSampleRequired = oldOrder.PreSample,
                    OrderStatus = ((tblOrderStatus)oldOrder.Status).ToCustomerOrderStatus(),
                    InvoiceDate = oldOrder.InvoiceDate,
                    InvoiceNotes = oldOrder.InvoiceNotes,
                    CreditMemo = oldOrder.CreditMemo,
                    FreightCharge = (float)(oldOrder.FreightCharge ?? 0.0m),

                    SoldTo = oldOrder.SoldTo,
                    InventoryShipmentOrder = new InventoryShipmentOrder
                        {
                            MoveNum = oldOrder.OrderNum,
                            EmployeeId = employeeId,
                            TimeStamp = timestamp,
                            OrderType = oldOrder.TTypeID.ToOrderType(),
                            ShipmentInfoDateCreated = shipmentInformation.DateCreated,
                            ShipmentInfoSequence = shipmentInformation.Sequence,
                            ShipmentInformation = shipmentInformation,
                            DestinationFacilityId = null,
                            SourceFacilityId = warehouse.Id,
                            OrderStatus = oldOrder.Status.ToTblOrderStatus().ToOrderStatus(),

                            PurchaseOrderNumber = oldOrder.PONbr,
                            DateReceived = dateReceived,
                            RequestedBy = oldOrder.From,
                            TakenBy = oldOrder.TakenBy,

                            PickedInventory = new PickedInventory
                                {
                                    EmployeeId = employeeId,
                                    TimeStamp = timestamp,
                                    PickedReason = PickedReason.SalesOrder,
                                    Archived = oldOrder.Status > (int)tblOrderStatus.Staged,
                                },
                            InventoryPickOrder = new InventoryPickOrder()
                        }
                }, oldOrder);

            var dateCreated = oldOrder.EntryDate.Value.Date;
            int sequence;
            ISalesOrderKey key;
            if(_salesOrderKey.TryParse(oldOrder.SerializedKey, out key))
            {
                dateCreated = key.SalesOrderKey_DateCreated;
                sequence = key.SalesOrderKey_Sequence;
            }
            else
            {
                sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);
                order.RequiresKeySync = true;
            }
            SetPickedInventoryKey(order, dateCreated, sequence);

            return order;
        }

        private ShipmentInformation CreateShipmentInformation(tblOrderDTO oldOrder)
        {
            var dateCreated = oldOrder.EntryDate.Value.Date;

            return new ShipmentInformation
                {
                    DateCreated = dateCreated,
                    Sequence = _newContextHelper.ShipmentInformationKeys.GetNextSequence(dateCreated),

                    Status = ((tblOrderStatus)oldOrder.Status).ToShipmentStatus(),
                    PalletWeight = (double?) oldOrder.PalletOR,
                    PalletQuantity = (int) (oldOrder.PalletQty ?? 0),
                    FreightBillType = oldOrder.FreightBillType,
                    ShipmentMethod = oldOrder.ShipVia,
                    DriverName = oldOrder.Driver,
                    CarrierName = oldOrder.Carrier,
                    TrailerLicenseNumber = oldOrder.TrlNbr,
                    ContainerSeal = oldOrder.ContSeal,

                    RequiredDeliveryDate = oldOrder.DelDueDate,
                    ShipmentDate = oldOrder.Date ?? oldOrder.SchShipDate,

                    ShipTo = oldOrder.ShipTo,
                    FreightBill = oldOrder.BillTo
                };
        }

        private SalesOrderLoad SetItems(SalesOrderLoad order, tblOrderDTO oldOrder)
        {
            var orderItemSequence = 0;
            var pickedItemSequence = 0;
            order.SalesOrderItems = new List<SalesOrderItem>();
            order.SalesOrderPickedItems = new List<SalesOrderPickedItem>();

            var latestStaged = oldOrder.tblOrderDetails.SelectMany(d => d.tblStagedFGs)
                                       .OrderByDescending(s => s.EntryDate)
                                       .FirstOrDefault();
            if(latestStaged != null)
            {
                order.InventoryShipmentOrder.PickedInventory.TimeStamp = latestStaged.EntryDate.ConvertLocalToUTC();
                if(latestStaged.EmployeeID != null)
                {
                    order.InventoryShipmentOrder.PickedInventory.EmployeeId = latestStaged.EmployeeID.Value;
                }
            }

            foreach(var detail in oldOrder.tblOrderDetails)
            {
                LoadCount.AddRead(EntityType.SalesOrderItem);

                var product = _newContextHelper.GetProduct(detail.ProdID);
                if(product == null)
                {
                    Log(new CallbackParameters(CallbackReason.DetailProductNotFound)
                        {
                            Order = oldOrder,
                            Detail = detail
                        });
                    continue;
                }

                var packaging = _newContextHelper.GetPackagingProduct(detail.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.DetailPackagingNotFound)
                        {
                            Order = oldOrder,
                            Detail = detail
                        });
                    continue;
                }

                var treatment = _newContextHelper.NoTreatment;
                if(detail.TrtmtID != null)
                {
                    treatment = _newContextHelper.GetInventoryTreatment(detail.TrtmtID.Value);
                    if(treatment == null)
                    {
                        Log(new CallbackParameters(CallbackReason.DetailTreatmentNotFound)
                            {
                                Order = oldOrder,
                                Detail = detail
                            });
                        continue;
                    }
                }

                IContractItemKey contractItemKey = null;
                if(detail.KDetailID != null)
                {
                    contractItemKey = _newContextHelper.GetContractItemKey(detail.KDetailID);
                    if(contractItemKey == null)
                    {
                        Log(new CallbackParameters(CallbackReason.ContractItemNotFound)
                            {
                                Order = oldOrder,
                                Detail = detail
                            });
                        continue;
                    }
                }

                order.SalesOrderItems.Add(new SalesOrderItem
                    {
                        ItemSequence = orderItemSequence,
                        ContractYear = contractItemKey == null ? (int?) null : contractItemKey.ContractKey_Year,
                        ContractSequence = contractItemKey == null ? (int?) null : contractItemKey.ContractKey_Sequence,
                        ContractItemSequence = contractItemKey == null ? (int?) null : contractItemKey.ContractItemKey_Sequence,
                        PriceBase = (double) (detail.Price ?? 0),
                        PriceFreight = (double) (detail.FreightP ?? 0),
                        PriceTreatment = (double) (detail.TrtnmntP ?? 0),
                        PriceWarehouse = (double) (detail.WHCostP ?? 0),
                        PriceRebate = (double) (detail.Rebate ?? 0),
                        ODetail = detail.ODetail,

                        InventoryPickOrderItem = new InventoryPickOrderItem
                            {
                                ItemSequence = orderItemSequence,
                                ProductId = product.ProductKey.ProductKey_ProductId,
                                PackagingProductId = packaging.Id,
                                TreatmentId = treatment.Id,
                                Quantity = (int) (detail.Quantity ?? 0),
                                CustomerLotCode = detail.CustLot,
                                CustomerProductCode = detail.CustProductCode,
                            }
                    });

                foreach(var staged in detail.tblStagedFGs)
                {
                    LoadCount.AddRead(EntityType.SalesOrderPickedItem);

                    LotKey lotKey;
                    if(!LotNumberParser.ParseLotNumber(staged.Lot, out lotKey))
                    {
                        Log(new CallbackParameters(CallbackReason.StagedInvalidLotNumber)
                            {
                                Order = oldOrder,
                                Staged = staged
                            });
                        continue;
                    }

                    packaging = _newContextHelper.GetPackagingProduct(staged.PkgID);
                    if(packaging == null)
                    {
                        Log(new CallbackParameters(CallbackReason.StagedPackagingNotFound)
                            {
                                Order = oldOrder,
                                Staged = staged
                            });
                        continue;
                    }

                    var currentLocation = _newContextHelper.GetLocation(staged.LocID);
                    if(currentLocation == null)
                    {
                        Log(new CallbackParameters(CallbackReason.StagedLocationNotFound)
                            {
                                Order = oldOrder,
                                Staged = staged
                            });
                        continue;
                    }

                    var fromLocation = detail.tblOutgoing.Any() ? DeterminePickedFromLocation(detail, staged) : currentLocation;
                    if(fromLocation == null)
                    {
                        Log(new CallbackParameters(CallbackReason.UndeterminedPickedFromLocation)
                            {
                                Order = oldOrder,
                                Staged = staged
                            });
                        continue;
                    }

                    treatment = _newContextHelper.GetInventoryTreatment(staged.TrtmtID);
                    if(treatment == null)
                    {
                        Log(new CallbackParameters(CallbackReason.StagedTreatmentNotFound)
                            {
                                Order = oldOrder,
                                Staged = staged
                            });
                        continue;
                    }

                    order.SalesOrderPickedItems.Add(new SalesOrderPickedItem
                        {
                            ItemSequence = pickedItemSequence,
                            OrderItemSequence = orderItemSequence,

                            PickedInventoryItem = new PickedInventoryItem
                                {
                                    DetailID = staged.EntryDate,
                                    ItemSequence = pickedItemSequence,
                                    LotDateCreated = lotKey.LotKey_DateCreated,
                                    LotDateSequence = lotKey.LotKey_DateSequence,
                                    LotTypeId = lotKey.LotKey_LotTypeId,

                                    Quantity = (int) (staged.Quantity ?? 0),
                                    PackagingProductId = packaging.Id,
                                    FromLocationId = fromLocation.Id,
                                    CurrentLocationId = currentLocation.Id,
                                    TreatmentId = treatment.Id,
                                    ToteKey = "",

                                    CustomerLotCode = staged.CustLot, 
                                    CustomerProductCode = staged.CustProductCode
                                }
                        });
                    pickedItemSequence++;
                }
                orderItemSequence++;
            }

            return order;
        }

        private Location DeterminePickedFromLocation(tblOrderDetailDTO detail, tblStagedFGDTO staged)
        {
            var outgoing = detail.tblOutgoing.OrderBy(o => o.EntryDate)
                .FirstOrDefault(o => o.Lot == staged.Lot && o.PkgID == staged.PkgID && o.TrtmtID == staged.TrtmtID && o.Quantity == staged.Quantity);
            return outgoing == null ? null : _newContextHelper.GetLocation(outgoing.LocID);
        }

        private static void SetPickedInventoryKey(SalesOrder order, DateTime dateCreated, int sequence)
        {
            order.DateCreated = dateCreated;
            order.Sequence = sequence;

            order.InventoryShipmentOrder.DateCreated = dateCreated;
            order.InventoryShipmentOrder.Sequence = sequence;

            order.InventoryShipmentOrder.InventoryPickOrder.DateCreated = order.DateCreated;
            order.InventoryShipmentOrder.InventoryPickOrder.Sequence = order.Sequence;
            if(order.SalesOrderItems != null)
            {
                foreach(var item in order.SalesOrderItems)
                {
                    item.DateCreated = item.InventoryPickOrderItem.DateCreated = order.InventoryShipmentOrder.InventoryPickOrder.DateCreated;
                    item.Sequence = item.InventoryPickOrderItem.OrderSequence = order.InventoryShipmentOrder.InventoryPickOrder.Sequence;
                }
            }

            order.InventoryShipmentOrder.PickedInventory.DateCreated = order.DateCreated;
            order.InventoryShipmentOrder.PickedInventory.Sequence = order.Sequence;
            if(order.SalesOrderPickedItems != null)
            {
                foreach(var item in order.SalesOrderPickedItems)
                {
                    item.DateCreated = item.PickedInventoryItem.DateCreated = order.InventoryShipmentOrder.PickedInventory.DateCreated;
                    item.Sequence = item.PickedInventoryItem.Sequence = order.InventoryShipmentOrder.PickedInventory.Sequence;
                }
            }
        }

        private IEnumerable<tblOrderDTO> SelectOrdersToLoad()
        {
            return OldContext.CreateObjectSet<tblOrder>()
                .AsNoTracking()
                .Where(o => o.TTypeID == (decimal) TransType.Sale || o.TTypeID == (decimal) TransType.MiscInvoice)
                .OrderByDescending(o => o.SerializedKey != null)
                .Select(o => new tblOrderDTO
                    {
                        SerializedKey = o.SerializedKey,
                        OrderNum = o.OrderNum,
                        EntryDate = o.EntryDate,
                        EmployeeID = o.EmployeeID,
                        TTypeID = (TransType) o.TTypeID,

                        Status = o.Status,
                        Company_IA = o.Company_IA,
                        Broker = o.Broker,
                        WHID = o.WHID,
                        From = o.From,
                        TakenBy = o.TakenBy,
                        DateRecd = o.DateRecd,
                        PayTerms = o.PayTerms,
                        PreSample = o.PreSample,
                        PONbr = o.PONbr,
                        Date = o.Date,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNotes = o.InvoiceNotes,
                        CreditMemo = o.CreditMemo,
                        FreightCharge = o.FCharge,

                        PalletOR = o.PalletOR,
                        PalletQty = o.PalletQty,
                        FreightBillType = o.FreightBillType,
                        ShipVia = o.ShipVia,
                        Driver = o.Driver,
                        Carrier = o.Carrier,
                        TrlNbr = o.TrlNbr,
                        ContSeal = o.ContSeal,

                        DelDueDate = o.DelDueDate,
                        SchShipDate = o.SchdShipDate,

                        SoldTo = new SoldToLabel
                            {
                                Name = o.CCompany,
                                Address = new Address
                                    {
                                        AddressLine1 = o.CAddress1,
                                        AddressLine2 = o.CAddress2,
                                        AddressLine3 = o.CAddress3,
                                        City = o.CCity,
                                        State = o.CState,
                                        PostalCode = o.CZip,
                                        Country = o.CCountry
                                    }
                            },
                        ShipTo = new ShipToLabel
                            {
                                Name = o.SCompany,
                                Phone = o.SPhone,
                                Address = new Address
                                {
                                    AddressLine1 = o.SAddress1,
                                    AddressLine2 = o.SAddress2,
                                    AddressLine3 = o.SAddress3,
                                    City = o.SCity,
                                    State = o.SState,
                                    PostalCode = o.SZip,
                                    Country = o.SCountry
                                }
                            },
                        BillTo = new BillToLabel
                            {
                                Name = o.Company,
                                Phone = o.Phone,
                                EMail = o.Email,
                                Fax = o.Fax,
                                Address = new Address
                                {
                                    AddressLine1 = o.Address1,
                                    AddressLine2 = o.Address2,
                                    AddressLine3 = o.Address3,
                                    City = o.City,
                                    State = o.State,
                                    PostalCode = o.Zip,
                                    Country = o.Country
                                }
                            },

                        tblOrderDetails = o.tblOrderDetails.Select(d => new tblOrderDetailDTO
                            {
                                ODetail = d.ODetail,
                                ProdID = d.ProdID,
                                PkgID = d.PkgID,
                                Quantity = d.Quantity,
                                TrtmtID = d.TrtmtID,
                                Price = d.Price,
                                FreightP = d.FreightP,
                                TrtnmntP = d.TrtnmntP,
                                WHCostP = d.WHCostP,
                                Rebate = d.Rebate,
                                KDetailID = d.KDetailID,
                                CustProductCode = d.CustProductCode,
                                CustLot = d.CustLot,

                                tblStagedFGs = d.tblStagedFGs.Select(s => new tblStagedFGDTO
                                    {
                                        EntryDate = s.EntryDate,
                                        Lot = s.Lot,
                                        TTypeID = s.TTypeID,
                                        PkgID = s.PkgID,
                                        Quantity = s.Quantity,
                                        LocID = s.LocID,
                                        TrtmtID = s.TrtmtID,
                                        EmployeeID = s.EmployeeID,
                                        CustLot = s.CustLot,
                                        CustProductCode = s.CustProductCode,
                                    }),

                                tblOutgoing = d.tblOutgoing.Select(n => new tblOutgoingDTO
                                    {
                                        EntryDate = n.EntryDate,
                                        Quantity = n.Quantity,
                                        Lot = n.Lot,
                                        PkgID = n.PkgID,
                                        LocID = n.LocID,
                                        TrtmtID = n.TrtmtID
                                    })
                            })
                    });
        }

        public class tblOrderDTO
        {
            public string SerializedKey { get; set; }
            public int OrderNum { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime? EntryDate { get; set; }
            public TransType TTypeID { get; set; }

            public int? Status { get; set; }
            public string Company_IA { get; set; }
            public string Broker { get; set; }
            public int? WHID { get; set; }
            public string From { get; set; }
            public string TakenBy { get; set; }
            public DateTime? DateRecd { get; set; }
            public string PayTerms { get; set; }
            public bool PreSample { get; set; }
            public string PONbr { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public string InvoiceNotes { get; set; }
            public bool CreditMemo { get; set; }
            public decimal? FreightCharge { get; set; }

            public decimal? PalletOR { get; set; }
            public decimal? PalletQty { get; set; }
            public string FreightBillType { get; set; }
            public string ShipVia { get; set; }
            public string Driver { get; set; }
            public string Carrier { get; set; }
            public string TrlNbr { get; set; }
            public string ContSeal { get; set; }

            public DateTime? DelDueDate { get; set; }
            public DateTime? SchShipDate { get; set; }
            public ShippingLabel SoldTo { get; set; }
            public ShippingLabel ShipTo { get; set; }
            public ShippingLabel BillTo { get; set; }

            public IEnumerable<tblOrderDetailDTO> tblOrderDetails { get; set; }
        }

        public class SoldToLabel : ShippingLabel { }
        public class ShipToLabel : ShippingLabel { }
        public class BillToLabel : ShippingLabel { }

        public class tblOrderDetailDTO
        {
            public DateTime ODetail { get; set; }
            public int? ProdID { get; set; }
            public int? PkgID { get; set; }
            public decimal? Quantity { get; set; }
            public int? TrtmtID { get; set; }
            public decimal? Price { get; set; }
            public decimal? FreightP { get; set; }
            public decimal? TrtnmntP { get; set; }
            public decimal? WHCostP { get; set; }
            public decimal? Rebate { get; set; }
            public DateTime? KDetailID { get; set; }
            public string CustProductCode { get; set; }
            public string CustLot { get; set; }

            public IEnumerable<tblStagedFGDTO> tblStagedFGs { get; set; }
            public IEnumerable<tblOutgoingDTO> tblOutgoing { get; set; }
        }

        public class tblStagedFGDTO
        {
            public DateTime EntryDate { get; set; }
            public int Lot { get; set; }
            public int? TTypeID { get; set; }
            public int PkgID { get; set; }
            public decimal? Quantity { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public int? EmployeeID { get; set; }
            public string CustLot { get; set; }
            public string CustProductCode { get; set; }
        }

        public class tblOutgoingDTO
        {
            public DateTime EntryDate { get; set; }
            public decimal? Quantity { get; set; }
            public int Lot { get; set; }
            public int PkgID { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
        }

        public enum EntityType
        {
            SalesOrder,
            SalesOrderItem,
            SalesOrderPickedItem
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,

            EntryDateNull,
            EmployeeIDNull,
            CustomerNotFound,
            BrokerNotFound,
            WarehouseNotFound,
            DateRecdNull,
            InvalidStatus,

            DetailProductNotFound,
            DetailPackagingNotFound,
            DetailTreatmentNotFound,
            ContractItemNotFound,

            StagedInvalidLotNumber,
            StagedPackagingNotFound,
            StagedLocationNotFound,
            StagedTreatmentNotFound,
            UndeterminedShipmentDate,

            UndeterminedPickedFromLocation,
            OldAndEmpty
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public tblOrderDTO Order { get; set; }
            public tblOrderDetailDTO Detail { get; set; }
            public tblStagedFGDTO Staged { get; set; }

            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override CallbackReason ExceptionReason { get { return SalesOrderEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return SalesOrderEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return SalesOrderEntityObjectMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case SalesOrderEntityObjectMother.CallbackReason.EntryDateNull:
                    case SalesOrderEntityObjectMother.CallbackReason.CustomerNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.BrokerNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.WarehouseNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.InvalidStatus:
                    case SalesOrderEntityObjectMother.CallbackReason.DetailProductNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.DetailPackagingNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.DetailTreatmentNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.StagedInvalidLotNumber:
                    case SalesOrderEntityObjectMother.CallbackReason.StagedPackagingNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.StagedLocationNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.StagedTreatmentNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.UndeterminedShipmentDate:
                    case SalesOrderEntityObjectMother.CallbackReason.ContractItemNotFound:
                    case SalesOrderEntityObjectMother.CallbackReason.UndeterminedPickedFromLocation:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }

        public class SalesOrderLoad : SalesOrder
        {
            public bool RequiresKeySync = false;
        }
    }
}