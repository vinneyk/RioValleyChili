using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public static class TblOrderExtensions
    {
        public static void SetSalesOrder(this tblOrder tblOrder, SalesOrder salesOrder)
        {
            tblOrder.WHID = salesOrder.InventoryShipmentOrder.SourceFacility.WHID;
            tblOrder.Company_IA = salesOrder.Customer == null ? null : salesOrder.Customer.Company.Name;
            tblOrder.Broker = salesOrder.Broker == null ? null : salesOrder.Broker.Name;
            tblOrder.PayTerms = salesOrder.PaymentTerms;
            tblOrder.Status = (int?) DetermineOrderStatus(salesOrder);
            tblOrder.SetShippingInformation(salesOrder);
            tblOrder.SerializedKey = salesOrder.ToSalesOrderKey();
            tblOrder.InvoiceDate = salesOrder.InvoiceDate;
            tblOrder.InvoiceNotes = salesOrder.InvoiceNotes;
            tblOrder.FCharge = (decimal?) salesOrder.FreightCharge;
            tblOrder.CreditMemo = salesOrder.CreditMemo;
        }

        public static void SetShippingInformation(this tblOrder tblOrder, SalesOrder salesOrder)
        {
            tblOrder.PONbr = salesOrder.InventoryShipmentOrder.PurchaseOrderNumber;
            tblOrder.DateRecd = salesOrder.InventoryShipmentOrder.DateReceived;
            tblOrder.From = salesOrder.InventoryShipmentOrder.RequestedBy;
            tblOrder.TakenBy = salesOrder.InventoryShipmentOrder.TakenBy;

            tblOrder.Carrier = salesOrder.InventoryShipmentOrder.ShipmentInformation.CarrierName;
            tblOrder.ShipVia = salesOrder.InventoryShipmentOrder.ShipmentInformation.ShipmentMethod;
            tblOrder.FreightBillType = salesOrder.InventoryShipmentOrder.ShipmentInformation.FreightBillType;
            tblOrder.TrlNbr = salesOrder.InventoryShipmentOrder.ShipmentInformation.TrailerLicenseNumber;
            tblOrder.Driver = salesOrder.InventoryShipmentOrder.ShipmentInformation.DriverName;
            tblOrder.ContSeal = salesOrder.InventoryShipmentOrder.ShipmentInformation.ContainerSeal;

            tblOrder.PalletOR = (decimal?)salesOrder.InventoryShipmentOrder.ShipmentInformation.PalletWeight;
            tblOrder.PalletQty = salesOrder.InventoryShipmentOrder.ShipmentInformation.PalletQuantity;

            tblOrder.SpclInstr = salesOrder.InventoryShipmentOrder.ShipmentInformation.SpecialInstructions;
            tblOrder.InternalNotes = salesOrder.InventoryShipmentOrder.ShipmentInformation.InternalNotes;
            tblOrder.ExternalNotes = salesOrder.InventoryShipmentOrder.ShipmentInformation.ExternalNotes;

            tblOrder.SetShippingLabel(salesOrder.InventoryShipmentOrder.ShipmentInformation.FreightBill);
            tblOrder.SetSShippingLabel(salesOrder.InventoryShipmentOrder.ShipmentInformation.ShipTo);
            tblOrder.SetCShippingLabel(salesOrder.SoldTo);
            
            tblOrder.DelDueDate = salesOrder.InventoryShipmentOrder.ShipmentInformation.RequiredDeliveryDate;
            tblOrder.SchdShipDate = salesOrder.InventoryShipmentOrder.ShipmentInformation.ShipmentDate;
            tblOrder.Date = salesOrder.InventoryShipmentOrder.ShipmentInformation.ShipmentDate;
        }

        public static void SetShippingLabel(this tblOrder tblOrder, ShippingLabel shippingLabel)
        {
            tblOrder.Company = shippingLabel.Name;
            tblOrder.Phone = shippingLabel.Phone;
            tblOrder.Email = shippingLabel.EMail;
            tblOrder.Fax = shippingLabel.Fax;
            tblOrder.Address1 = shippingLabel.Address.AddressLine1;
            tblOrder.Address2 = shippingLabel.Address.AddressLine2;
            tblOrder.Address3 = shippingLabel.Address.AddressLine3;
            tblOrder.City = shippingLabel.Address.City;
            tblOrder.State = shippingLabel.Address.State;
            tblOrder.Zip = shippingLabel.Address.PostalCode;
            tblOrder.Country = shippingLabel.Address.Country;
        }

        public static void SetCShippingLabel(this tblOrder tblOrder, ShippingLabel shippingLabel)
        {
            tblOrder.CCompany = shippingLabel.Name;
            tblOrder.CAddress1 = shippingLabel.Address.AddressLine1;
            tblOrder.CAddress2 = shippingLabel.Address.AddressLine2;
            tblOrder.CAddress3 = shippingLabel.Address.AddressLine3;
            tblOrder.CCity = shippingLabel.Address.City;
            tblOrder.CState = shippingLabel.Address.State;
            tblOrder.CZip = shippingLabel.Address.PostalCode;
            tblOrder.CCountry = shippingLabel.Address.Country;
        }

        public static void SetSShippingLabel(this tblOrder tblOrder, ShippingLabel shippingLabel)
        {
            tblOrder.SCompany = shippingLabel.Name;
            tblOrder.SAddress1 = shippingLabel.Address.AddressLine1;
            tblOrder.SAddress2 = shippingLabel.Address.AddressLine2;
            tblOrder.SAddress3 = shippingLabel.Address.AddressLine3;
            tblOrder.SCity = shippingLabel.Address.City;
            tblOrder.SState = shippingLabel.Address.State;
            tblOrder.SZip = shippingLabel.Address.PostalCode;
            tblOrder.SCountry = shippingLabel.Address.Country;
            tblOrder.SPhone = shippingLabel.Phone;
        }

        public static tblOrderStatus DetermineOrderStatus(SalesOrder salesOrder)
        {
            if(salesOrder.InventoryShipmentOrder.OrderStatus == OrderStatus.Void)
            {
                return tblOrderStatus.Void;
            }

            switch(salesOrder.OrderStatus)
            {
                case SalesOrderStatus.Ordered:
                    switch(salesOrder.InventoryShipmentOrder.ShipmentInformation.Status)
                    {
                        case ShipmentStatus.Shipped: return tblOrderStatus.Shipped;
                        default: return tblOrderStatus.Scheduled;
                    }
                case SalesOrderStatus.Invoiced: return tblOrderStatus.Invoiced;
                default: throw new ArgumentOutOfRangeException("CustomerOrderStatus");
            }
        }
    }
}
