using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryCycleCountReturn : IInventoryCycleCountReportReturn
    {
        public string FacilityName { get; set; }
        public string GroupName { get; set; }
        public DateTime ReportDateTime { get; private set; }

        public IEnumerable<IInventoryCycleCountLocation> Locations { get; private set; }
        public IEnumerable<InventoryCycleCountLocationSelect> LocationsSelect { get; set; }

        public void Initialize()
        {
            ReportDateTime = DateTime.Now;
            Locations = LocationsSelect
                .Where(l => l.IsGroup(GroupName))
                .Select(l =>
                    {
                        string street;
                        int row;
                        LocationDescriptionHelper.GetStreetRow(l.Location, out street, out row);
                        return new
                            {
                                Street = street,
                                Row = row,
                                Select = l
                            };
                    })
                .OrderBy(l => l.Street).ThenBy(l => l.Row)
                .Select(l => new InventoryCycleCountLocationReturn
                    {
                        Location = LocationDescriptionHelper.FormatLocationDescription(l.Select.Location),
                        Inventory = l.Select.InventorySelect.GroupBy(i => new
                            {
                                lotKey = i.LotKeyReturn.ToLotKey(),
                                productKey = i.ProductSelect.ToProductKey(),
                                packagingKey = i.PackagingSelect.ProductKeyReturn.ToProductKey(),
                                treatmentKey = i.TreatmentSelect.InventoryTreatmentKeyReturn.ToInventoryTreatmentKey()
                            }).Select(g => new InventoryCycleCountInventoryReturn
                            {
                                LotKey = g.Key.lotKey,
                                ProductionDate = g.First().ProductionDate,
                                ProductCode = g.First().ProductSelect.ProductCode,
                                ProductName = g.First().ProductSelect.ProductName,
                                Packaging = g.First().PackagingSelect.ProductName,
                                Treatment = g.First().TreatmentSelect.TreatmentNameShort,
                                Quantity = g.Sum(i => i.Quantity),
                                Weight = g.Sum(i => i.Weight)
                            })
                            .OrderBy(i => i.LotKey)
                            .ToList()
                    })
                .ToList();
        }
    }

    internal class InventoryCycleCountLocationSelect
    {
        public string Location { get; set; }
        public IEnumerable<InventoryCycleCountInventorySelect> InventorySelect { get; set; }

        public bool IsGroup(string groupName)
        {
            string street;
            int row;
            if(LocationDescriptionHelper.GetStreetRow(Location, out street, out row))
            {
                return street == groupName;
            }
            return Location == groupName;
        }
    }

    internal class InventoryCycleCountInventorySelect
    {
        public LotKeyReturn LotKeyReturn { get; set; }
        public DateTime ProductionDate { get; set; }
        public InventoryProductReturn ProductSelect { get; set; }
        public PackagingProductReturn PackagingSelect { get; set; }
        public InventoryTreatmentReturn TreatmentSelect { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
    }

    internal class InventoryCycleCountLocationReturn : IInventoryCycleCountLocation
    {
        public string Location { get; set; }
        public IEnumerable<IInventoryCycleCount> Inventory { get; set; }
    }

    internal class InventoryCycleCountInventoryReturn : IInventoryCycleCount
    {
        public string LotKey { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Packaging { get; set; }
        public string Treatment { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
    }
}