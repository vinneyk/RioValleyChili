using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.OldContextSynchronization.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Synchronize
{
    [Sync(NewContextMethod.SetShipmentInformation)]
    public class SyncSetShipmentInformation : SyncCommandBase<IInventoryShipmentOrderUnitOfWork, InventoryShipmentOrderKey>
    {
        public SyncSetShipmentInformation(IInventoryShipmentOrderUnitOfWork unitOfWork) : base(unitOfWork) { }

        public override void Synchronize(Func<InventoryShipmentOrderKey> getInput)
        {
            if(getInput == null) { throw new ArgumentNullException("getInput"); }
            
            var newOrderKey = getInput();
            var newOrder = UnitOfWork.InventoryShipmentOrderRepository
                .FindByKey(newOrderKey, o => o.ShipmentInformation);
            if(newOrder == null)
            {
                throw new Exception(string.Format("Could not find InventoryShipmentOrder[{0}].", newOrderKey));
            }

            if(newOrder.MoveNum == null)
            {
                throw new Exception(string.Format("InventoryShipmentOrder[{0}].MoveNum is null.", newOrderKey));
            }

            switch(newOrder.OrderType)
            {
                case InventoryShipmentOrderTypeEnum.InterWarehouseOrder:
                case InventoryShipmentOrderTypeEnum.TreatmentOrder:
                    SetMoveShipment(newOrder);
                    OldContext.SaveChanges();
                    Console.WriteLine(ConsoleOutput.SetShipmentTblMove, newOrder.MoveNum);
                    break;

                case InventoryShipmentOrderTypeEnum.SalesOrder:
                case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                    SetOrderShipment(newOrder);
                    OldContext.SaveChanges();
                    Console.WriteLine(ConsoleOutput.SetShipmentTblOrder, newOrder.MoveNum);
                    break;

                default: throw new ArgumentOutOfRangeException("order.OrderType");
            }
        }

        public static void SetOrderShipment(InventoryShipmentOrder newOrder, tblMove oldOrder)
        {
            oldOrder.PalletOR = (decimal?)newOrder.ShipmentInformation.PalletWeight;
            oldOrder.PalletQty = newOrder.ShipmentInformation.PalletQuantity;
            oldOrder.FreightBillType = newOrder.ShipmentInformation.FreightBillType;
            oldOrder.ShipVia = newOrder.ShipmentInformation.ShipmentMethod;
            oldOrder.Driver = newOrder.ShipmentInformation.DriverName;
            oldOrder.Carrier = newOrder.ShipmentInformation.CarrierName;
            oldOrder.TrlNbr = newOrder.ShipmentInformation.TrailerLicenseNumber;
            oldOrder.ContSeal = newOrder.ShipmentInformation.ContainerSeal;
            oldOrder.InternalNotes = newOrder.ShipmentInformation.InternalNotes;
            oldOrder.ExternalNotes = newOrder.ShipmentInformation.ExternalNotes;
            oldOrder.SpclInstr = newOrder.ShipmentInformation.SpecialInstructions;

            oldOrder.PONbr = newOrder.PurchaseOrderNumber;
            oldOrder.DateRecd = newOrder.DateReceived;
            oldOrder.From = newOrder.RequestedBy;
            oldOrder.TakenBy = newOrder.TakenBy;

            oldOrder.DelDueDate = newOrder.ShipmentInformation.RequiredDeliveryDate;
            oldOrder.SchdShipDate = newOrder.ShipmentInformation.ShipmentDate;
            oldOrder.Date = newOrder.ShipmentInformation.ShipmentDate;

            oldOrder.CCompany = newOrder.ShipmentInformation.ShipFrom.Name;
            //oldOrder.CPhone = newOrder.ShipmentInformation.ShipFrom.Phone;
            //oldOrder.CEmail = newOrder.ShipmentInformation.ShipFrom.EMail;
            //oldOrder.CFax = newOrder.ShipmentInformation.ShipFrom.Fax;
            oldOrder.CAddress1 = newOrder.ShipmentInformation.ShipFrom.Address.AddressLine1;
            oldOrder.CAddress2 = newOrder.ShipmentInformation.ShipFrom.Address.AddressLine2;
            oldOrder.CAddress3 = newOrder.ShipmentInformation.ShipFrom.Address.AddressLine3;
            oldOrder.CCity = newOrder.ShipmentInformation.ShipFrom.Address.City;
            oldOrder.CState = newOrder.ShipmentInformation.ShipFrom.Address.State;
            oldOrder.CZip = newOrder.ShipmentInformation.ShipFrom.Address.PostalCode;
            oldOrder.CCountry = newOrder.ShipmentInformation.ShipFrom.Address.Country;

            oldOrder.SCompany = newOrder.ShipmentInformation.ShipTo.Name;
            oldOrder.SPhone = newOrder.ShipmentInformation.ShipTo.Phone;
            //oldOrder.SEmail = newOrder.ShipmentInformation.ShipTo.EMail;
            //oldOrder.SFax = newOrder.ShipmentInformation.ShipTo.Fax;
            oldOrder.SAddress1 = newOrder.ShipmentInformation.ShipTo.Address.AddressLine1;
            oldOrder.SAddress2 = newOrder.ShipmentInformation.ShipTo.Address.AddressLine2;
            oldOrder.SAddress3 = newOrder.ShipmentInformation.ShipTo.Address.AddressLine3;
            oldOrder.SCity = newOrder.ShipmentInformation.ShipTo.Address.City;
            oldOrder.SState = newOrder.ShipmentInformation.ShipTo.Address.State;
            oldOrder.SZip = newOrder.ShipmentInformation.ShipTo.Address.PostalCode;
            oldOrder.SCountry = newOrder.ShipmentInformation.ShipTo.Address.Country;

            oldOrder.Company = newOrder.ShipmentInformation.FreightBill.Name;
            oldOrder.Phone = newOrder.ShipmentInformation.FreightBill.Phone;
            oldOrder.Email = newOrder.ShipmentInformation.FreightBill.EMail;
            oldOrder.Fax = newOrder.ShipmentInformation.FreightBill.Fax;
            oldOrder.Address1 = newOrder.ShipmentInformation.FreightBill.Address.AddressLine1;
            oldOrder.Address2 = newOrder.ShipmentInformation.FreightBill.Address.AddressLine2;
            oldOrder.Address3 = newOrder.ShipmentInformation.FreightBill.Address.AddressLine3;
            oldOrder.City = newOrder.ShipmentInformation.FreightBill.Address.City;
            oldOrder.State = newOrder.ShipmentInformation.FreightBill.Address.State;
            oldOrder.Zip = newOrder.ShipmentInformation.FreightBill.Address.PostalCode;
            oldOrder.Country = newOrder.ShipmentInformation.FreightBill.Address.Country;
        }

        private void SetMoveShipment(InventoryShipmentOrder newOrder)
        {
            var oldOrder = OldContext.tblMoves.FirstOrDefault(m => m.MoveNum == newOrder.MoveNum.Value);
            if(oldOrder == null)
            {
                throw new Exception(string.Format("Could not find tblMove[{0}].", newOrder.MoveNum.Value));
            }

            SetOrderShipment(newOrder, oldOrder);
        }

        private void SetOrderShipment(InventoryShipmentOrder newOrder)
        {
            var customerOrder = UnitOfWork.SalesOrderRepository
                .Filter(o => o.InventoryShipmentOrder.DateCreated == newOrder.DateCreated && o.InventoryShipmentOrder.Sequence == newOrder.Sequence)
                .FirstOrDefault();
            if(customerOrder.InventoryShipmentOrder == null)
            {
                customerOrder.InventoryShipmentOrder = newOrder;
            }

            var oldOrder = OldContext.tblOrders.FirstOrDefault(m => m.OrderNum == newOrder.MoveNum.Value);
            if(oldOrder == null)
            {
                throw new Exception(string.Format("Could not find tblOrder[{0}].", newOrder.MoveNum.Value));
            }

            oldOrder.SetShippingInformation(customerOrder);
        }
    }
}