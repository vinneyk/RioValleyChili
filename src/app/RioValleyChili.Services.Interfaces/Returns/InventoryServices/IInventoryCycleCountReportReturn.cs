using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.InventoryServices
{
    public interface IInventoryCycleCountReportReturn
    {
        string FacilityName { get; }
        string GroupName { get; }
        DateTime ReportDateTime { get; }

        IEnumerable<IInventoryCycleCountLocation> Locations { get; }
    }

    public interface IInventoryCycleCountLocation
    {
        string Location { get; }
        IEnumerable<IInventoryCycleCount> Inventory { get; }
    }

    public interface IInventoryCycleCount
    {
        string LotKey { get; }
        DateTime ProductionDate { get; }
        string ProductCode { get; }
        string ProductName { get; }
        string Packaging { get; }
        string Treatment { get; }
        int Quantity { get; }
        double Weight { get; }
    }
}