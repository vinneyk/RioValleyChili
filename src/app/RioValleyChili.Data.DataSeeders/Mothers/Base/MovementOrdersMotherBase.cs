using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.Base
{
    public abstract class MovementOrdersMotherBase<T> : EntityMotherLogBase<T, MovementOrdersMotherBase<T>.CallbackParameters> where T : class
    {
        protected readonly NewContextHelper NewContextHelper;

        protected abstract int[] TransTypeIDs { get; }

        protected virtual OrderStatus GetOrderStatus(tblOrderStatus? orderStatus)
        {
            return orderStatus.ToOrderStatus();
        }

        protected MovementOrdersMotherBase(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback)
            : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            NewContextHelper = new NewContextHelper(newContext);
        }

        protected enum EntityTypes
        {
            TreatmentOrder,
            InterWarehouseOrder,
            ConsignmentOrder,

            InventoryShipmentOrder,
            PickOrder,
            PickOrderItem,
            PickedInventory,
            PickedInventoryItem,
            ShipmentInformation,
        }

        protected readonly MotherLoadCount<EntityTypes> LoadCount = new MotherLoadCount<EntityTypes>();

        protected InventoryShipmentOrder SetOrderProperties(InventoryShipmentOrder newOrder, MovementOrderDTO oldOrder)
        {
            newOrder.OrderType = GetOrderType(oldOrder.TTypeID);
            newOrder.MoveNum = oldOrder.MoveNum;
            newOrder.PurchaseOrderNumber = oldOrder.PONbr;
            newOrder.DateReceived = oldOrder.DateRecd;
            newOrder.RequestedBy = oldOrder.From;
            newOrder.TakenBy = oldOrder.TakenBy;
            newOrder.OrderStatus = GetOrderStatus(oldOrder.Status.ToTblOrderStatus());

            return newOrder;
        }

        protected InventoryPickOrder CreateInventoryPickOrder(IInventoryPickOrderKey pickOrderKey, MovementOrderDTO order)
        {
            LoadCount.AddRead(EntityTypes.PickOrder);

            return new InventoryPickOrder
                {
                    DateCreated = pickOrderKey.InventoryPickOrderKey_DateCreated,
                    Sequence = pickOrderKey.InventoryPickOrderKey_Sequence,

                    Items = CreateInventoryPickOrderItems(pickOrderKey, order).ToList()
                };
        }
        
        protected PickedInventory CreatePickedInventory(IPickedInventoryKey pickedInventoryKey, MovementOrderDTO order, ShipmentStatus shipmentStatus)
        {
            LoadCount.AddRead(EntityTypes.PickedInventory);

            int? employeeId = null;
            DateTime? timestamp = null;

            var latestMoveDetail = order.tblMoveDetails.OrderByDescending(d => d.MDetail).FirstOrDefault();
            if(latestMoveDetail != null)
            {
                employeeId = latestMoveDetail.EmployeeID;
                timestamp = latestMoveDetail.MDetail.ConvertLocalToUTC();
            }

            if(employeeId == null)
            {
                employeeId = employeeId ?? order.EmployeeID;
                timestamp = timestamp ?? order.EntryDate.ConvertLocalToUTC();

                if(employeeId == null || timestamp == null)
                {
                    var latestOutgoing = order.tblOutgoings.OrderByDescending(d => d.EntryDate).FirstOrDefault();
                    if(latestOutgoing != null)
                    {
                        employeeId = employeeId ?? latestOutgoing.EmployeeID;
                        timestamp = timestamp ?? latestOutgoing.EntryDate.ConvertLocalToUTC();
                    }
                }
            }
            
            if(timestamp == null)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryNoTimestamp)
                    {
                        MovementOrder = order
                    });
                return null;
            }
            
            employeeId = employeeId ?? NewContextHelper.DefaultEmployee.EmployeeId;
            if(employeeId == NewContextHelper.DefaultEmployee.EmployeeId)
            {
                Log(new CallbackParameters(CallbackReason.PickedInventoryDefaultEmployee)
                    {
                        MovementOrder = order,
                        DefaultEmployeeId = employeeId.Value
                    });
            }
            
            return new PickedInventory
                {
                    EmployeeId = employeeId.Value,
                    TimeStamp = timestamp.Value,

                    DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                    Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,

                    PickedReason = ((TransType)order.TTypeID).ToPickedReason(),
                    Archived = order.Status > (int)tblOrderStatus.Staged,
                    Items = CreatePickedInventoryItemsFromMoveDetails(pickedInventoryKey, order, shipmentStatus).ToList()
                };
        }

        protected ShipmentInformation CreateShipmentInformation(IShipmentInformationKey shipmentInformationKey, MovementOrderDTO order)
        {
            LoadCount.AddRead(EntityTypes.ShipmentInformation);

            var truncated = false;
            var shipmentInfo = new ShipmentInformation
                {
                    DateCreated = shipmentInformationKey.ShipmentInfoKey_DateCreated,
                    Sequence = shipmentInformationKey.ShipmentInfoKey_Sequence,
                    
                    Status = ShipmentStatusExtensions.FromOrderStatID(order.Status),
                    PalletWeight = (double?) order.PalletOR,
                    PalletQuantity = (int)(order.PalletQty ?? 0),
                    FreightBillType = order.FreightBillType.AnyTruncate(Constants.StringLengths.FreightBillType, ref truncated),
                    ShipmentMethod = order.ShipVia.AnyTruncate(Constants.StringLengths.ShipmentMethod, ref truncated),
                    DriverName = order.Driver.AnyTruncate(Constants.StringLengths.DriverName, ref truncated),
                    CarrierName = order.Carrier.AnyTruncate(Constants.StringLengths.CarrierName, ref truncated),
                    TrailerLicenseNumber = order.TrlNbr.AnyTruncate(Constants.StringLengths.TrailerLicenseNumber, ref truncated),
                    ContainerSeal = order.ContSeal.AnyTruncate(Constants.StringLengths.ContainerSeal, ref truncated),

                    RequiredDeliveryDate = order.DelDueDate,
                    ShipmentDate = order.Date ?? order.SchdShipDate,

                    InternalNotes = order.InternalNotes.AnyTruncate(Constants.StringLengths.ShipmentInformationNotes, ref truncated),
                    ExternalNotes = order.ExternalNotes.AnyTruncate(Constants.StringLengths.ShipmentInformationNotes, ref truncated),
                    SpecialInstructions = order.SpecialInstructions.AnyTruncate(Constants.StringLengths.ShipmentInformationNotes, ref truncated),

                    ShipFrom = new ShippingLabel
                        {
                            Name = order.CCompany.AnyTruncate(Constants.StringLengths.ShippingLabelName, ref truncated),
                            Address = new Address
                            {
                                AddressLine1 = order.CAddress1.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine2 = order.CAddress2.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine3 = order.CAddress3.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                City = order.CCity.AnyTruncate(Constants.StringLengths.AddressCity, ref truncated),
                                State = order.CState.AnyTruncate(Constants.StringLengths.AddressState, ref truncated),
                                PostalCode = order.CZip.AnyTruncate(Constants.StringLengths.AddressPostalCode, ref truncated),
                                Country = order.CCountry.AnyTruncate(Constants.StringLengths.AddressCountry, ref truncated)
                            }
                        },
                    ShipTo = new ShippingLabel
                        {
                            Name = order.SCompany.AnyTruncate(Constants.StringLengths.ShippingLabelName, ref truncated),
                            Phone = order.SPhone.AnyTruncate(Constants.StringLengths.PhoneNumber, ref truncated),
                            Address = new Address
                            {
                                AddressLine1 = order.SAddress1.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine2 = order.SAddress2.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine3 = order.SAddress3.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                City = order.SCity.AnyTruncate(Constants.StringLengths.AddressCity, ref truncated),
                                State = order.SState.AnyTruncate(Constants.StringLengths.AddressState, ref truncated),
                                PostalCode = order.SZip.AnyTruncate(Constants.StringLengths.AddressPostalCode, ref truncated),
                                Country = order.SCountry.AnyTruncate(Constants.StringLengths.AddressCountry, ref truncated)
                            }
                        },
                    FreightBill = new ShippingLabel
                        {
                            Name = order.Company.AnyTruncate(Constants.StringLengths.ShippingLabelName, ref truncated),
                            Phone = order.Phone.AnyTruncate(Constants.StringLengths.PhoneNumber, ref truncated),
                            EMail = order.Email.AnyTruncate(Constants.StringLengths.Email, ref truncated),
                            Fax = order.Fax.AnyTruncate(Constants.StringLengths.Fax, ref truncated),
                            Address = new Address
                            {
                                AddressLine1 = order.Address1.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine2 = order.Address2.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                AddressLine3 = order.Address3.AnyTruncate(Constants.StringLengths.AddressLine, ref truncated),
                                City = order.City.AnyTruncate(Constants.StringLengths.AddressCity, ref truncated),
                                State = order.State.AnyTruncate(Constants.StringLengths.AddressState, ref truncated),
                                PostalCode = order.Zip.AnyTruncate(Constants.StringLengths.AddressPostalCode, ref truncated),
                                Country = order.Country.AnyTruncate(Constants.StringLengths.AddressCountry, ref truncated)
                            }
                        }
                };

            return shipmentInfo;
        }

        protected List<MovementOrderDTO> SelectMovementOrdersToLoad()
        {
            var transTypes = TransTypeIDs;

            return OldContext.CreateObjectSet<tblMove>()
                .Where(m => transTypes.Contains(m.TTypeID))
                .Select(m => new MovementOrderDTO
                    {
                        Serialized = m.Serialized,
                        
                        TTypeID = m.TTypeID,
                        MoveNum = m.MoveNum,
                        EmployeeID = m.EmployeeID,
                        EntryDate = m.EntryDate,
                        FromWHID = m.WHID,
                        ToWHID = m.C2WHID,
                        PONbr = m.PONbr,
                        Date = m.Date,
                        DateRecd = m.DateRecd,
                        From = m.From,
                        TakenBy = m.TakenBy,
                        Returned = m.Returned,

                        Status = m.Status,
                        PalletOR = m.PalletOR,
                        PalletQty = m.PalletQty,
                        FreightBillType = m.FreightBillType,
                        ShipVia = m.ShipVia,
                        Driver = m.Driver,
                        Carrier = m.Carrier,
                        TrlNbr = m.TrlNbr,
                        ContSeal = m.ContSeal,

                        CCompany = m.CCompany,
                        CAddress1 = m.CAddress1,
                        CAddress2 = m.CAddress2,
                        CAddress3 = m.CAddress3,
                        CCity = m.CCity,
                        CState = m.CState,
                        CZip = m.CZip,
                        CCountry = m.CCountry,

                        SCompany = m.SCompany,
                        SAddress1 = m.SAddress1,
                        SAddress2 = m.SAddress2,
                        SAddress3 = m.SAddress3,
                        SCity = m.SCity,
                        SState = m.SState,
                        SZip = m.SZip,
                        SCountry = m.SCountry,
                        SPhone = m.SPhone,

                        Company = m.Company,
                        Address1 = m.Address1,
                        Address2 = m.Address2,
                        Address3 = m.Address3,
                        City = m.City,
                        State = m.State,
                        Zip = m.Zip,
                        Country = m.Country,
                        Email = m.Email,
                        Phone = m.Phone,
                        Fax = m.Fax,

                        SchdShipDate = m.SchdShipDate,
                        DelDueDate = m.DelDueDate,
                        InternalNotes = m.InternalNotes,
                        ExternalNotes = m.ExternalNotes,
                        SpecialInstructions = m.SpclInstr,

                        tblMoveOrderDetails = m.tblMoveOrderDetails.Select(o => new tblMoveOrderDetail
                            {
                                MOrderDetail = o.MOrderDetail,
                                MoveNum = o.MoveNum,
                                ProdID = o.ProdID,
                                PkgID = o.PkgID,
                                TrtmtID = o.TrtmtID,
                                Quantity = o.Quantity,
                                CustomerID = o.CustomerID,
                                CustLot = o.CustLot,
                                CustProductCode = o.CustProductCode,
                            }),
                        tblMoveDetails = m.tblMoveDetails.Select(d => new tblMoveDetail
                            {
                                EmployeeID = d.EmployeeID,
                                MDetail = d.MDetail,
                                MoveNum = d.MoveNum,
                                Lot = d.Lot,
                                PkgID = d.PkgID,
                                Quantity = d.Quantity,
                                LocID = d.LocID,
                                Move2 = d.Move2,
                                TrtmtID = d.TrtmtID,
                                PType = d.tblLot.PTypeID,
                                CustLot = d.CustLot,
                                CustProductCode = d.CustProductCode
                            }),
                        tblOutgoings = m.tblOutgoings.Select(o => new tblOutgoing
                            {
                                EntryDate = o.EntryDate,
                                EmployeeID = o.EmployeeID,
                                ID = o.ID,
                                Lot = o.Lot,
                                PkgID = o.PkgID,
                                LocID = o.LocID,
                                TrtmtID = o.TrtmtID,
                                Tote = o.Tote,
                                Quantity = o.Quantity,
                                TTypeID = o.TTypeID
                            })
                    }).ToList();
        }

        protected static InventoryShipmentOrderTypeEnum GetOrderType(int transTypeId)
        {
            return ((TransType)transTypeId).ToOrderType();
        }

        protected static EntityTypes GetEntityType(int transTypeId)
        {
            switch((TransType)transTypeId)
            {
                case TransType.ToTrmt: return EntityTypes.TreatmentOrder;
                case TransType.Move: return EntityTypes.InterWarehouseOrder;
                case TransType.OnConsignment: return EntityTypes.ConsignmentOrder;
                default: throw new ArgumentOutOfRangeException("transTypeId");
            }
        }

        private IEnumerable<InventoryPickOrderItem> CreateInventoryPickOrderItems(IInventoryPickOrderKey pickOrderKey, MovementOrderDTO order)
        {
            var sequence = 0;
            foreach(var orderDetail in order.tblMoveOrderDetails)
            {
                LoadCount.AddRead(EntityTypes.PickOrderItem);

                var product = NewContextHelper.GetProduct(orderDetail.ProdID);
                if(product == null)
                {
                    Log(new CallbackParameters(CallbackReason.OrderItemProductNotFound)
                        {
                            MovementOrder = order,
                            MoveOrderDetail = orderDetail
                        });
                    continue;
                }

                var packaging = NewContextHelper.GetPackagingProduct(orderDetail.PkgID);
                var treatment = NewContextHelper.GetInventoryTreatment(orderDetail.TrtmtID);

                Models.Company customer = null;
                if(!string.IsNullOrWhiteSpace(orderDetail.CustomerID))
                {
                    customer = NewContextHelper.GetCompany(orderDetail.CustomerID, CompanyType.Customer);
                    if(customer == null)
                    {
                        Log(new CallbackParameters(CallbackReason.OrderItemCustomerNotFound)
                            {
                                MovementOrder = order,
                                MoveOrderDetail = orderDetail
                            });
                        continue;
                    }
                }

                yield return new InventoryPickOrderItem
                    {
                        DateCreated = pickOrderKey.InventoryPickOrderKey_DateCreated,
                        OrderSequence = pickOrderKey.InventoryPickOrderKey_Sequence,
                        ItemSequence = ++sequence,

                        ProductId = product.ProductKey.ProductKey_ProductId,
                        PackagingProductId = packaging.Id,
                        TreatmentId = treatment.Id,
                        Quantity = (int)orderDetail.Quantity,
                        CustomerId = customer == null ? (int?)null : customer.Id,
                        CustomerLotCode = orderDetail.CustLot,
                        CustomerProductCode = orderDetail.CustProductCode
                    };
            }
        }

        private IEnumerable<PickedInventoryItem> CreatePickedInventoryItemsFromMoveDetails(IPickedInventoryKey pickedInventoryKey, MovementOrderDTO order, ShipmentStatus shipmentStatus)
        {
            var sequence = 0;

            var moveDetails = order.tblMoveDetails.ToList();
            foreach(var moveDetail in moveDetails)
            {
                LoadCount.AddRead(EntityTypes.PickedInventoryItem);

                var lotKey = LotNumberParser.ParseLotNumber(moveDetail.Lot);
                if(lotKey == null)
                {
                    Log(new CallbackParameters(CallbackReason.MoveDetailInvalidLotNumber)
                    {
                        MoveDetail = moveDetail
                    });
                    continue;
                }

                if(!NewContextHelper.LotLoaded(lotKey))
                {
                    Log(new CallbackParameters(CallbackReason.MoveDetailLotNotLoaded)
                    {
                        MoveDetail = moveDetail
                    });
                    continue;
                }

                var packaging = moveDetail.PType == (int?)LotTypeEnum.Packaging ? NewContextHelper.NoPackagingProduct : NewContextHelper.GetPackagingProduct(moveDetail.PkgID);
                if(packaging == null)
                {
                    Log(new CallbackParameters(CallbackReason.MoveDetailPackagingNotLoaded)
                    {
                        MoveDetail = moveDetail
                    });
                    continue;
                }

                var pickedLocation = NewContextHelper.GetLocation(moveDetail.LocID);
                if(pickedLocation == null)
                {
                    pickedLocation = NewContextHelper.UnknownFacilityLocation;
                    if(pickedLocation == null)
                    {
                        Log(new CallbackParameters(CallbackReason.MoveDetailUnknownWarehouseLocationNotLoaded)
                        {
                            MoveDetail = moveDetail
                        });
                        continue;
                    }

                    Log(new CallbackParameters(CallbackReason.MoveDetailDefaultUnknownWarehouseLocation)
                    {
                        MoveDetail = moveDetail,
                        Location = pickedLocation
                    });
                }

                var currentLocation = shipmentStatus == ShipmentStatus.Shipped ? NewContextHelper.GetLocation(moveDetail.Move2) ?? pickedLocation : pickedLocation;

                var treatment = NewContextHelper.GetInventoryTreatment(moveDetail.TrtmtID);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.MoveDetailTreatmentNotLoaded)
                    {
                        MoveDetail = moveDetail
                    });
                    continue;
                }

                const string tote = "";

                yield return new PickedInventoryItem
                {
                    DateCreated = pickedInventoryKey.PickedInventoryKey_DateCreated,
                    Sequence = pickedInventoryKey.PickedInventoryKey_Sequence,
                    ItemSequence = ++sequence,
                    DetailID = moveDetail.MDetail,

                    LotDateCreated = lotKey.LotKey_DateCreated,
                    LotDateSequence = lotKey.LotKey_DateSequence,
                    LotTypeId = lotKey.LotKey_LotTypeId,
                    PackagingProductId = packaging.Id,
                    TreatmentId = treatment.Id,
                    ToteKey = tote,
                    Quantity = (int)moveDetail.Quantity,
                    FromLocationId = pickedLocation.Id,
                    CurrentLocationId = currentLocation.Id,
                    CustomerLotCode = moveDetail.CustLot,
                    CustomerProductCode = moveDetail.CustProductCode
                };
            }
        }

        public class MovementOrderDTO
        {
            public string Serialized { get; set; }
            public string SerializedKey { get; set; }

            public int TTypeID { get; set; }
            public int MoveNum { get; set; }
            public int? EmployeeID { get; set; }
            public DateTime? EntryDate { get; set; }
            public string PONbr { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? DateRecd { get; set; }
            public string From { get; set; }
            public string TakenBy { get; set; }
            public DateTime? Returned { get; set; }
            public int? FromWHID { get; set; }
            public int? ToWHID { get; set; }
            public int? Status { get; set; }
            public decimal? PalletOR { get; set; }
            public decimal? PalletQty { get; set; }
            public string FreightBillType { get; set; }
            public string ShipVia { get; set; }
            public string Driver { get; set; }
            public string Carrier { get; set; }
            public string TrlNbr { get; set; }
            public string ContSeal { get; set; }

            public string CCompany { get; set; }
            public string CAddress1 { get; set; }
            public string CAddress2 { get; set; }
            public string CAddress3 { get; set; }
            public string CCity { get; set; }
            public string CState { get; set; }
            public string CZip { get; set; }
            public string CCountry { get; set; }

            public string SCompany { get; set; }
            public string SAddress1 { get; set; }
            public string SAddress2 { get; set; }
            public string SAddress3 { get; set; }
            public string SCity { get; set; }
            public string SState { get; set; }
            public string SZip { get; set; }
            public string SCountry { get; set; }
            public string SPhone { get; set; }

            public string Company { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string Country { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }

            public DateTime? SchdShipDate { get; set; }
            public DateTime? DelDueDate { get; set; }
            public string InternalNotes { get; set; }
            public string ExternalNotes { get; set; }
            public string SpecialInstructions { get; set; }

            public IEnumerable<tblMoveOrderDetail> tblMoveOrderDetails { get; set; }
            public IEnumerable<tblMoveDetail> tblMoveDetails { get; set; }
            public IEnumerable<tblOutgoing> tblOutgoings { get; set; }

        }

        public class tblMoveOrderDetail
        {
            public DateTime MOrderDetail { get; set; }
            public int? MoveNum { get; set; }
            public int? ProdID { get; set; }
            public int PkgID { get; set; }
            public int TrtmtID { get; set; }
            public decimal? Quantity { get; set; }
            public string CustomerID { get; set; }
            public string CustLot { get; set; }
            public string CustProductCode { get; set; }
        }

        public class tblMoveDetail
        {
            public int? EmployeeID { get; set; }
            public DateTime MDetail { get; set; }
            public int? MoveNum { get; set; }
            public int Lot { get; set; }
            public int? PType { get; set; }
            public int PkgID { get; set; }
            public decimal? Quantity { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public int? Move2 { get; set; }
            public string CustLot { get; set; }
            public string CustProductCode { get; set; }
        }

        public class tblOutgoing
        {
            public DateTime EntryDate { get; set; }
            public int? EmployeeID { get; set; }
            public int ID { get; set; }
            public int Lot { get; set; }
            public int PkgID { get; set; }
            public int LocID { get; set; }
            public int TrtmtID { get; set; }
            public string Tote { get; set; }
            public decimal? Quantity { get; set; }
            public int? TTypeID { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            TreatmentNotDetermined,
            TreatmentFaciltyNotLoaded,
            MoveDetailDefaultUnknownWarehouseLocation,
            MoveDetailInvalidLotNumber,
            MoveDetailPackagingNotLoaded,
            MoveDetailTreatmentNotLoaded,
            NullEntryDate,
            DestinationWarehouseNotLoaded,
            MoveDetailUnknownWarehouseLocationNotLoaded,
            MoveDetailLotNotLoaded,
            PickedInventoryNoTimestamp,
            PickedInventoryDefaultEmployee,
            StringTruncated,
            SourceWarehouseNotLoaded,
            UndeterminedShipmentDate,
            OrderItemCustomerNotFound,
            OrderItemProductNotFound
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public MovementOrderDTO MovementOrder { get; set; }
            public tblMoveDetail MoveDetail { get; set; }
            public tblMoveOrderDetail MoveOrderDetail { get; set; }
            public Location Location { get; set; }
            public int DefaultEmployeeId { get; set; }

            protected override CallbackReason ExceptionReason { get { return MovementOrdersMotherBase<T>.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return MovementOrdersMotherBase<T>.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return MovementOrdersMotherBase<T>.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case MovementOrdersMotherBase<T>.CallbackReason.Exception:
                        return ReasonCategory.Error;
                        
                    case MovementOrdersMotherBase<T>.CallbackReason.TreatmentNotDetermined:
                    case MovementOrdersMotherBase<T>.CallbackReason.TreatmentFaciltyNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailUnknownWarehouseLocationNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailInvalidLotNumber:
                    case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailPackagingNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailTreatmentNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.MoveDetailLotNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.NullEntryDate:
                    case MovementOrdersMotherBase<T>.CallbackReason.DestinationWarehouseNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.SourceWarehouseNotLoaded:
                    case MovementOrdersMotherBase<T>.CallbackReason.PickedInventoryNoTimestamp:
                    case MovementOrdersMotherBase<T>.CallbackReason.UndeterminedShipmentDate:
                    case MovementOrdersMotherBase<T>.CallbackReason.OrderItemCustomerNotFound:
                    case MovementOrdersMotherBase<T>.CallbackReason.OrderItemProductNotFound:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}