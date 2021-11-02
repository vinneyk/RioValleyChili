using System;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService
{
    public interface IChileMaterialsReceivedSummaryReturn
    {
        string LotKey { get; }

        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }
        int TotalLoad { get; }

        IChileProductReturn ChileProduct { get; }
        ICompanySummaryReturn Supplier { get; }
        IInventoryTreatmentReturn Treatment { get; }
    }
}