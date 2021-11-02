using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class InternalOrderAcknowledgement : ICustomerNotesContainer
    {
        public string OrderKey { get; set; }
        public int? MovementNumber { get; set; }
        public InventoryShipmentOrderTypeEnum OrderType { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int TotalQuantity { get; set; }
        public double NetWeight { get; set; }
        public double TotalGrossWeight { get; set; }
        public double PalletWeight { get; set; }
        public double EstimatedShippingWeight { get; set; }
        public double TotalValue { get { return PickOrderItems.Select(i => i.TotalValue).DefaultIfEmpty(0.0).Sum(); } }
        public DateTime? DateReceived { get; set; }
        public string RequestedBy { get; set; }
        public string TakenBy { get; set; }
        public string OriginFacility { get; set; }

        public ShipmentInformation ShipmentInformation { get; set; }
        public SalesOrderInternalAcknowledgement SalesOrder { get; set; }

        public IEnumerable<InventoryPickOrderItemReturn> PickOrderItems { get; set; }
        public IEnumerable<CustomerNotesReturn> CustomerNotes { get; set; }

        public double PalletWeight_Access { get { return ShipmentInformation.PalletWeight ?? ShipmentInformation.PalletQuantity * Constants.Reporting.DefaultOrderPalletWeight; } }
        public double EstimatedShippingWeight_Access { get { return TotalGrossWeight + PalletWeight_Access; } }

        public string ReportTitleLabel
        {
            get
            {
                switch(OrderType)
                {
                    case InventoryShipmentOrderTypeEnum.SalesOrder:
                    case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                        return "Internal Order Acknowledgement";

                    default: return "Warehouse Order Acknowledgement";
                }
            }
        }

        public string OrderKeyLabel
        {
            get
            {
                switch(OrderType)
                {
                    case InventoryShipmentOrderTypeEnum.SalesOrder:
                    case InventoryShipmentOrderTypeEnum.MiscellaneousOrder:
                        return "Order Number";

                    default: return "Move Number";
                }
            }
        }
        
        public bool IsSalesOrder { get { return SalesOrder != null; } }
        public string MoveFromLabel { get { return IsSalesOrder ? "Sold To" : "Move From"; } }
        public string MoveToLabel { get { return IsSalesOrder ? "Ship To" : "Move To"; } }
        public AddressLabel MoveFromAddress { get { return IsSalesOrder ? SalesOrder.SoldToShippingLabel.Address : ShipmentInformation.ShippingInstructions.ShipFromShippingLabel.Address; } }
        public string PaymentTerms { get { return IsSalesOrder ? SalesOrder.PaymentTerms : ""; } }
        public string Broker { get { return IsSalesOrder ? SalesOrder.Broker : ""; } }
        public string MoveNumFormat { get { return IsSalesOrder ? "{0:0}" : "{0:0000-000}"; } }
        public IEnumerable<CustomerNoteGroup> GroupedCustomerNotes { get; private set; }

        public void Initialize()
        {
            if(!IsSalesOrder)
            {
                GroupedCustomerNotes = new List<CustomerNoteGroup>();
            }
            else
            {
                GroupedCustomerNotes = SalesOrder.CustomerNotes
                    .GroupBy(n => n.Type)
                    .OrderBy(g => g.Key)
                    .Select(g => new CustomerNoteGroup
                        {
                            Type = g.Key,
                            Notes = g.OrderBy(n => n.Text).ToList()
                        });
            }

            var customerOrContract = IsSalesOrder ? "Contract" : "Customer";
            var contractKeys = IsSalesOrder ? SalesOrder.OrderItems.ToDictionary(i => i.CustomerOrderItemKey) : null;
            foreach(var item in PickOrderItems)
            {
                item.CustomerOrContractLabel = customerOrContract;
                SalesOrderItemInternalAcknowledgement key;
                if(contractKeys != null && contractKeys.TryGetValue(item.OrderItemKey, out key))
                {
                    item.ContractKey = key.ContractDisplayKey;
                }

                var salesOrderItem = SalesOrder == null ? null : SalesOrder.OrderItems.FirstOrDefault(i => i.CustomerOrderItemKey == item.OrderItemKey);
                if(salesOrderItem != null)
                {
                    item.TotalPrice = salesOrderItem.TotalPrice;
                }
            }
        }
    }

    public class SalesOrderInternalAcknowledgement
    {
        public string PaymentTerms { get; set; }
        public string Broker { get; set; }
        public ShippingLabel SoldToShippingLabel { get; set; }
        public IEnumerable<CustomerNoteReturn> CustomerNotes { get; set; }
        public IEnumerable<SalesOrderItemInternalAcknowledgement> OrderItems { get; set; }
    }

    public class SalesOrderItemInternalAcknowledgement
    {
        public string CustomerOrderItemKey { get; set; }
        public string ContractKey { get; set; }
        public int? ContractId { get; set; }
        public double TotalPrice { get; set; }

        public string ContractDisplayKey { get { return string.Format("{0}{1}", ContractKey, ContractId == null ? "" : string.Format("({0})", ContractId)); } }
    }

    public class ShipmentInformation
    {
        public string ShipmentKey { get; set; }
        public ShipmentStatus Status { get; set; }
        public int PalletQuantity { get; set; }
        public double? PalletWeight { get; set; }

        public ShippingInstructions ShippingInstructions { get; set; }
        public TransitInformation TransitInformation { get; set; }
    }

    public class ShippingInstructions
    {
        public DateTime? RequiredDeliveryDateTime { get; set; }
        public DateTime? ShipmentDate { get; set; }

        public string InternalNotes { get; set; }
        public string ExternalNotes { get; set; }
        public string SpecialInstructions { get; set; }

        public ShippingLabel ShipFromShippingLabel { get; set; }
        public ShippingLabel ShipToShippingLabel { get; set; }
        public ShippingLabel FreightBillToShippingLabel { get; set; }
    }

    public class ShippingLabel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string Fax { get; set; }
        public AddressLabel Address { get; set; }
    }

    public class TransitInformation
    {
        public string ShipmentMethod { get; set; }
        public string FreightType { get; set; }
        public string DriverName { get; set; }
        public string CarrierName { get; set; }
        public string TrailerLicenseNumber { get; set; }
        public string ContainerSeal { get; set; }

        public string Prepaid
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(FreightType))
                {
                    if(FreightType.ToUpper().Contains("PREPAID"))
                    {
                        return "X";
                    }
                }
                return "";
            }
        }

        public string Collect
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(FreightType))
                {
                    if(FreightType.ToUpper().Contains("COLLECT"))
                    {
                        return "X";
                    }
                }
                return "";
            }
        }

        public string ThirdParty
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(FreightType))
                {
                    if(FreightType.ToUpper().Contains("3RD"))
                    {
                        return "X";
                    }
                }
                return "";
            }
        }
    }

    public class InventoryPickOrderItemReturn
    {
        public string OrderItemKey { get; set; }
        public string ProductKey { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PackagingProductKey { get; set; }
        public string PackagingName { get; set; }
        public double PackagingWeight { get; set; }
        public string TreatmentKey { get; set; }
        public string TreatmentNameShort { get; set; }
        public int Quantity { get; set; }
        public double TotalWeight { get; set; }
        public string CustomerLotCode { get; set; }
        public string CustomerProductCode { get; set; }
        public CompanyHeaderReturn Customer { get; set; }
        public virtual double TotalPrice { get; set; }
        public double TotalValue { get { return TotalPrice * TotalWeight; } }

        public string ContractKey { get; set; }
        public string CustomerOrContractLabel { get; set; }
        public string CustomerOrContract { get { return ContractKey ?? (Customer == null ? "" : Customer.Name); } }
    }

    public class CompanyHeaderReturn
    {
        public string CompanyKey { get; set; }
        public string Name { get; set; }
    }

    public class CustomerNotesReturn
    {
        public string CompanyKey { get; set; }
        public string Name { get; set; }
        public IEnumerable<CustomerNoteReturn> CustomerNotes { get; set; }
    }

    public class CustomerNoteReturn
    {
        public string CustomerNoteKey { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Bold { get; set; }
    }

    public class CustomerNoteGroup
    {
        public string Type { get; set; }
        public IEnumerable<CustomerNoteReturn> Notes { get; set; }
    }
}