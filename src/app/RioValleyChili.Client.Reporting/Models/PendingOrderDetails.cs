using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AutoMapper;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;

namespace RioValleyChili.Client.Reporting.Models
{
    public class PendingOrderDetails
    {
        public DateTime Now { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }

        public int ScheduledAmount { get; set; }
        public int ShippedAmount { get; set; }
        public int RemainingAmount { get; set; }
        public int NewAmount { get; set; }

        public string BetweenDatesText { get { return BetweenText(StartDate, EndDate); } }
        public string ScheduledText { get { return string.Format("Orders Scheduled to Ship {0}", BetweenText(StartDate, EndDate)); } }
        public string ShippedText { get { return string.Format("Orders Shipped {0}", BetweenText(StartDate, EndDate)); } }
        public string RemainingText { get { return string.Format("Remaining Orders to Ship {0}", BetweenText(StartDate, EndDate)); } }
        public string NewText { get { return string.Format("New Orders to Ship {0}", BetweenText(NewStartDate, NewEndDate)); } }

        public IEnumerable<PendingCustomerOrder> PendingCustomerOrders { get; set; }
        public IEnumerable<PendingWarehouseOrder> PendingWarehouseOrders { get; set; }

        public bool HasPendingCustomerOrders { get { return PendingShipmentItems.Any(); } }
        public bool HasPendingWarheouseOrders { get { return PendingWarehouseItems.Any(); } }

        public PendingCustomerOrders PendingCustomerOrderSections { get; private set; }
        public PendingWarehouseOrders PendingWarehouseOrderSections { get; private set; }

        [IgnoreMap]
        public IEnumerable<PendingShipmentItem> PendingShipmentItems { get; set; }
        [IgnoreMap]
        public IEnumerable<PendingWarehouseItem> PendingWarehouseItems { get; set; }

        public void Initialize()
        {
            PendingCustomerOrderSections = new PendingCustomerOrders
                {
                    Sections = PendingCustomerOrders.GroupBy(o => o.GetStatus(Now))
                        .OrderBy(g => g.Key)
                        .Select(g => PendingSection<PendingCustomerOrder>.Create(g.Key,
                            g.OrderBy(o => o.ShipmentDate == null)
                            .ThenBy(o => o.ShipmentDate)
                            .ThenBy(o => o.Name)))
                        .ToList()
                };

            PendingWarehouseOrderSections = new PendingWarehouseOrders
                {
                    Sections = PendingWarehouseOrders.GroupBy(o => o.GetStatus(Now))
                        .OrderBy(g => g.Key)
                        .Select(g => PendingSection<PendingWarehouseOrder>.Create(g.Key,
                            g.OrderBy(o => o.ShipmentDate == null)
                            .ThenBy(o => o.ShipmentDate)
                            .ThenBy(o => o.OrderNum)))
                        .ToList()
                };

            var sectionKey = 0;
            var orderKey = 0;
            PendingShipmentItems = PendingCustomerOrderSections.Sections
                .SelectMany(section =>
                    {
                        section.Key = sectionKey++;
                        return section.Items.SelectMany(order =>
                            {
                                order.Key = orderKey++;
                                return order.Items.Select(item => new PendingShipmentItem
                                    {
                                        Section = section,
                                        Order = order,
                                        Item = item
                                    });
                            });
                    }).ToList();

            sectionKey = 0;
            orderKey = 0;
            PendingWarehouseItems = PendingWarehouseOrderSections.Sections
                .SelectMany(section =>
                    {
                        section.Key = sectionKey++;
                        return section.Items.SelectMany(order =>
                            {
                                order.Key = orderKey++;
                                return order.Items.Select(item => new PendingWarehouseItem
                                    {
                                        Section = section,
                                        Order = order,
                                        Item = item
                                    });
                            });
                    }).ToList();
        }

        private static string BetweenText(DateTime start, DateTime end)
        {
            return string.Format("Between {0:MM/dd/yyyy} and {1:MM/dd/yyyy}", start, end);
        }
    }

    public class PendingShipmentItem
    {
        public PendingSection<PendingCustomerOrder> Section { get; set; }
        public PendingCustomerOrder Order { get; set; }
        public PendingOrderItem Item { get; set; }
    }

    public class PendingWarehouseItem
    {
        public PendingSection<PendingWarehouseOrder> Section { get; set; }
        public PendingWarehouseOrder Order { get; set; }
        public PendingOrderItem Item { get; set; }
    }

    public class CompanyTest
    {
        public string CompanyName { get; set; }
        public string CompanyString { get; set; }
    }

    public class PendingSection<T>
        where T : PendingOrderBase
    {
        [IgnoreMap]
        public int Key { get; set; }
        public string SectionLabel { get; set; }
        public string SectionDescription { get; set; }
        public Color SectionColor { get; set; }

        public IEnumerable<T> Items { get; set; }

        private PendingSection() { }

        public static PendingSection<T> Create<T>(PendingOrderStatus status, IEnumerable<T> orders)
            where T : PendingOrderBase
        {
            switch(status)
            {
                case PendingOrderStatus.UnpickedAboutToShip:
                    return new PendingSection<T>
                        {
                            SectionLabel = "Warning",
                            SectionDescription = "Orders are scheduled to ship within 2 days and have not been picked",
                            SectionColor = Color.FromArgb(255, 255, 192, 192),
                            Items = orders
                        };

                case PendingOrderStatus.Picked:
                    return new PendingSection<T>
                        {
                            SectionLabel = "On Time",
                            SectionDescription = "Orders have been picked",
                            SectionColor = Color.FromArgb(255, 192, 192, 192),
                            Items = orders
                        };

                case PendingOrderStatus.UnpickedNotAboutToShip:
                    return new PendingSection<T>
                    {
                        SectionLabel = "Unpicked",
                        SectionDescription = "Orders have not been picked but are not scheduled to ship within 2 days",
                        SectionColor = Color.FromArgb(255, 255, 220, 220),
                        Items = orders
                    };

                default: throw new ArgumentOutOfRangeException("status");
            }
        }
    }

    public enum PendingOrderStatus
    {
        UnpickedAboutToShip,
        Picked,
        UnpickedNotAboutToShip
    }

    public abstract class PendingOrderBase
    {
        public DateTime? DateRecd { get; set; } 
        public string OrderNum { get; set; }
        public OrderStatus Status { get; set; }
        public int SubTotal { get { return Items.Select(i => i.LbsToShip).DefaultIfEmpty(0).Sum(); } }
        protected abstract DateTime? DateToShip { get; }

        public IEnumerable<PendingOrderItem> Items { get; set; }

        public PendingOrderStatus GetStatus(DateTime now)
        {
            if(Items.All(i => i.QuantityPicked >= i.QuantityOrdered))
            {
                return PendingOrderStatus.Picked;
            }

            if(DateToShip == null)
            {
                return PendingOrderStatus.UnpickedNotAboutToShip;
            }

            var dayThreshold = 3;
            switch(now.DayOfWeek)
            {
                case DayOfWeek.Thursday: dayThreshold += 2; break;
                case DayOfWeek.Friday: dayThreshold += 1; break;
            }

            return (DateToShip.Value.ToSimpleDate() - now).Days < dayThreshold ? PendingOrderStatus.UnpickedAboutToShip : PendingOrderStatus.UnpickedNotAboutToShip;
        }
    }

    public class PendingOrderItem
    {
        public int QuantityPicked { get; set; }
        public int QuantityOrdered { get; set; }
        public string Packaging { get; set; }
        public string Product { get; set; }
        public string Treatment { get; set; }
        public int LbsToShip { get; set; }
    }

    #region PendingShipments

    public class PendingCustomerOrders
    {
        public int Total { get { return Sections.SelectMany(s => s.Items.Select(c => c.SubTotal)).DefaultIfEmpty(0).Sum(); } }
        public IEnumerable<PendingSection<PendingCustomerOrder>> Sections { get; set; }
    }

    public class PendingCustomerOrder : PendingOrderBase
    {
        [IgnoreMap]
        public int Key { get; set; }
        public string Name { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string Origin { get; set; }
        public bool Sample { get; set; }

        protected override DateTime? DateToShip { get { return ShipmentDate; } }
    }

    #endregion

    #region PendingWarehouseOrders

    public class PendingWarehouseOrders
    {
        public int Total { get { return Sections.SelectMany(s => s.Items.Select(o => o.SubTotal)).DefaultIfEmpty(0).Sum(); } }
        public IEnumerable<PendingSection<PendingWarehouseOrder>> Sections { get; set; }
    }

    public class PendingWarehouseOrder : PendingOrderBase
    {
        [IgnoreMap]
        public int Key { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? ShipmentDate { get; set; }

        protected override DateTime? DateToShip { get { return ShipmentDate; } }
    }

    #endregion
}