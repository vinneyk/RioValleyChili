using System;
using System.Collections.Generic;

namespace RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService
{
    public interface IChileMaterialsReceivedRecapReturn
    {
        string LotKey { get; }
        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string EmployeeName { get; }
        string Supplier { get; }
        string Product { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }

        IEnumerable<IChileMaterialsReceivedRecapItemReturn> Items { get; }
    }

    public interface IChileMaterialsReceivedRecapItemReturn
    {
        string Tote { get; }
        int Quantity { get; }
        string Packaging { get; }
        double Weight { get; }
        string Variety { get; }
        string LocaleGrown { get; }
        string Location { get; }
    }
}