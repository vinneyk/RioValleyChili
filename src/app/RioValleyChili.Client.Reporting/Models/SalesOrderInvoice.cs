using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Client.Reporting.Models
{
    public class SalesOrderInvoice
    {
        public string ReportTitle { get { return CreditMemo ? "Credit Memo" : "Invoice"; } }
        public int? MovementNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PONumber { get; set; }
        public string InvoiceNotes { get; set; }
        public bool CreditMemo { get; set; }

        public string Origin { get; set; }
        public DateTime? ShipDate { get; set; }
        public string Broker { get; set; }
        public string PaymentTerms { get; set; }
        public string Freight { get; set; }
        public string ShipVia { get; set; }

        public bool VisibleInvoiceNotes { get { return !string.IsNullOrWhiteSpace(InvoiceNotes); } }

        public double FreightCharge { get; set; }
        public int TotalOrdered { get { return PickedItems.Select(i => i.QuantityOrdered).DefaultIfEmpty(0).Sum(); } }
        public int TotalShipped { get { return PickedItems.Select(i => i.QuantityShipped).DefaultIfEmpty(0).Sum(); } }
        public double TotalWeight { get { return PickedItems.Select(i => i.NetWeight).DefaultIfEmpty(0.0).Sum(); } }
        public double TotalValue { get { return PickedItems.Select(i => i.TotalValue).DefaultIfEmpty(0.0).Sum(); } }
        public double TotalDue { get { return TotalValue + FreightCharge; } }
        public double TotalValueOrdered { get { return OrderItems.Select(i => i.TotalValue).DefaultIfEmpty(0.0).Sum(); } }

        public ShippingLabel SoldTo { get; set; }
        public ShippingLabel ShipTo { get; set; }

        public IEnumerable<SalesOrderInvoicePickedItem> PickedItems { get; set; }
        public IEnumerable<SalesOrderInvoiceOrderItem> OrderItems { get; set; }
        public IEnumerable<SalesOrderInvoicePickedItem> InhouseInvoicePickedItems { get { return _inhouseInvoiceItems ?? (_inhouseInvoiceItems = GetInhouseInvoiceItems()); } }
        public IEnumerable<SalesOrderInvoicePickedItem> CustomerInvoicePickedItems { get { return _customerInvoiceItems ?? (_customerInvoiceItems = GetCustomerInvoiceItems()); } }

        public string SumPaprika { get { return SumTotal("paprika"); } }
        public string SumPaprikaETO { get { return SumTotal("paprika", "et"); } }
        public string SumPaprikaGamma { get { return SumTotal("paprika", "gt"); } }

        public string SumPowder { get { return SumTotal("powder"); } }
        public string SumPowderETO { get { return SumTotal("powder", "et"); } }
        public string SumPowderGamma { get { return SumTotal("powder", "gt"); } }

        public string SumPepper { get { return SumTotal("pepper"); } }
        public string SumPepperETO { get { return SumTotal("pepper", "et"); } }
        public string SumPepperGamma { get { return SumTotal("pepper", "gt"); } }

        public string SumHots { get { return SumTotal("grp"); } }
        public string SumHotsETO { get { return SumTotal("grp", "et"); } }
        public string SumHotsGamma { get { return SumTotal("grp", "gt"); } }

        public double? FreightRegular { get { return !CheckShipVia("delivered") && !CheckShipVia("ups") ? FreightCharge : (double?)null; } }
        public double? FreightInProduct { get { return CheckShipVia("delivered") ? FreightCharge : (double?)null; } }
        public double? FreightUPS { get { return CheckShipVia("ups") ? FreightCharge : (double?)null; } }
        public double FreightWH { get { return PickedItems.Select(i => i.FreightValue).DefaultIfEmpty(0.0).Sum(); } }
        public double RentWH { get { return PickedItems.Select(i => i.WarehouseValue).DefaultIfEmpty(0.0).Sum(); } }

        public void Initialize()
        {
            var orderItems = OrderItems.ToDictionary(i => i.SalesOrderItemKey);
            foreach(var item in PickedItems)
            {
                SalesOrderInvoiceOrderItem orderItem;
                if(orderItems.TryGetValue(item.SalesOrderItemKey ?? "", out orderItem))
                {
                    item.OrderItem = orderItem;
                }
            }
        }

        private string SumTotal(string productType, string treatmentType = null)
        {
            var value = PickedItems.Where(i =>
                i.ProductType.ToUpper().Contains(productType.ToUpper()) &&
                (treatmentType == null || i.TreatmentNameShort.ToUpper() == treatmentType.ToUpper()))
                .Select(i => treatmentType == null ? i.BaseValue : i.TreatmentValue)
                .DefaultIfEmpty(0.0)
                .Sum();
            return value == 0.0 ? "" : value.ToString("#,##0.00");
        }

        private bool CheckShipVia(string shipVia)
        {
            return string.IsNullOrWhiteSpace(ShipVia) ? string.IsNullOrWhiteSpace(shipVia) :
                !string.IsNullOrWhiteSpace(shipVia) && ShipVia.ToUpper().Contains(shipVia.ToUpper());
        }

        private IEnumerable<SalesOrderInvoicePickedItem> _inhouseInvoiceItems, _customerInvoiceItems;

        private IEnumerable<SalesOrderInvoicePickedItem> GetInhouseInvoiceItems()
        {
            return PickedItems.GroupBy(i => new
                    {
                        i.ProductCode,
                        i.ProductName,
                        i.CustomerProductCode,
                        i.PackagingName,
                        i.TreatmentNameShort,
                        i.TotalPrice,

                        i.Contract,
                        i.QuantityOrdered,
                        i.PriceBase,
                        i.PriceFreight,
                        i.PriceTreatment,
                        i.PriceWarehouse,
                    })
                .Select(g => new SalesOrderInvoicePickedItem
                    {
                        ProductCode = g.Key.ProductCode,
                        ProductName = g.Key.ProductName,
                        CustomerProductCode = g.Key.CustomerProductCode,
                        PackagingName = g.Key.PackagingName,
                        TreatmentNameShort = g.Key.TreatmentNameShort,
                        TotalPrice = g.Key.TotalPrice,

                        Contract = g.Key.Contract,
                        QuantityOrdered = g.Key.QuantityOrdered,
                        PriceBase = g.Key.PriceBase,
                        PriceFreight = g.Key.PriceFreight,
                        PriceTreatment = g.Key.PriceTreatment,
                        PriceWarehouse = g.Key.PriceWarehouse,

                        QuantityShipped = g.Sum(i => i.QuantityShipped),
                        NetWeight = g.Sum(i => i.NetWeight),
                    });
        }

        private IEnumerable<SalesOrderInvoicePickedItem> GetCustomerInvoiceItems()
        {
            return PickedItems.GroupBy(i => new
                    {
                        i.ProductCode,
                        i.ProductName,
                        i.CustomerProductCode,
                        i.PackagingName,
                        i.TreatmentNameShort,
                        i.QuantityOrdered,
                        i.TotalPrice
                    })
                .Select(g => new SalesOrderInvoicePickedItem
                    {
                        ProductCode = g.Key.ProductCode,
                        ProductName = g.Key.ProductName,
                        CustomerProductCode = g.Key.CustomerProductCode,
                        PackagingName = g.Key.PackagingName,
                        TreatmentNameShort = g.Key.TreatmentNameShort,
                        QuantityOrdered = g.Key.QuantityOrdered,
                        TotalPrice = g.Key.TotalPrice,

                        QuantityShipped = g.Sum(i => i.QuantityShipped),
                        NetWeight = g.Sum(i => i.NetWeight)
                    });
        }
    }

    public class SalesOrderInvoicePickedItem
    {
        public string SalesOrderItemKey { get; internal set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }

        public string CustomerProductCode { get; set; }
        public string PackagingName { get; set; }
        public string TreatmentNameShort { get; set; }
        
        public int QuantityShipped { get; set; }
        public double NetWeight { get; set; }

        public string Contract { get { return _contract ?? OrderItemSelect(i => i.Contract); } set { _contract = value; } }
        public int QuantityOrdered { get { return _quantityOrdered ?? OrderItemSelect(i => i.QuantityOrdered); } set { _quantityOrdered = value; } }
        public double PriceBase { get { return _priceBase ?? OrderItemSelect(i => i.PriceBase); } set { _priceBase = value; } }
        public double PriceFreight { get { return _priceFreight ?? OrderItemSelect(i => i.PriceFreight); } set { _priceFreight = value; } }
        public double PriceTreatment { get { return _priceTreatment ?? OrderItemSelect(i => i.PriceTreatment); } set { _priceTreatment = value; } }
        public double PriceWarehouse { get { return _priceWarehouse ?? OrderItemSelect(i => i.PriceWarehouse); } set { _priceWarehouse = value; } }
        public double PriceRebate { get { return _priceRebate ?? OrderItemSelect(i => i.PriceRebate); } set { _priceRebate = value; } }

        public double BaseValue { get { return NetWeight * PriceBase; } }
        public double FreightValue { get { return NetWeight * PriceFreight; } }
        public double TreatmentValue { get { return NetWeight * PriceTreatment; } }
        public double WarehouseValue { get { return NetWeight * PriceWarehouse; } }

        public double TotalPrice
        {
            get { return _totalPrice ?? (_totalPrice = PriceBase + PriceFreight + PriceTreatment + PriceWarehouse - PriceRebate).Value; }
            set { _totalPrice = value; }
        }
        public double TotalValue { get { return NetWeight * TotalPrice; } }

        internal SalesOrderInvoiceOrderItem OrderItem { get; set; }

        private double? _totalPrice;
        private string _contract;
        private int? _quantityOrdered;
        private double? _priceBase;
        private double? _priceFreight;
        private double? _priceTreatment;
        private double? _priceWarehouse;
        private double? _priceRebate;

        private T OrderItemSelect<T>(Func<SalesOrderInvoiceOrderItem, T> select)
        {
            return OrderItem == null ? default(T) : select(OrderItem);
        }
    }

    public class SalesOrderInvoiceOrderItem
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PackagingName { get; set; }
        public double NetWeight { get; set; }

        public string SalesOrderItemKey { get; internal set; }
        public string Contract { get; internal set; }

        public int QuantityOrdered { get; internal set; }
        public double PriceBase { get; internal set; }
        public double PriceFreight { get; internal set; }
        public double PriceTreatment { get; internal set; }
        public double PriceWarehouse { get; internal set; }
        public double PriceRebate { get; internal set; }

        public double TotalPrice { get { return PriceBase + PriceFreight + PriceTreatment + PriceWarehouse - PriceRebate; } }
        public double TotalValue { get { return TotalPrice * NetWeight; } }
    }
}