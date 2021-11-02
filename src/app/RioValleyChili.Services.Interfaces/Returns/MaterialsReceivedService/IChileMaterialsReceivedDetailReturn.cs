using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService
{
    public interface IChileMaterialsReceivedDetailReturn
    {
        string LotKey { get; }
        ChileMaterialsReceivedType ChileMaterialsReceivedType { get; }

        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }

        IChileProductReturn ChileProduct { get; }
        ICompanySummaryReturn Supplier { get; }
        IInventoryTreatmentReturn Treatment { get; }
        
        IEnumerable<IChileMaterialsReceivedItemReturn> Items { get; }
    }
}