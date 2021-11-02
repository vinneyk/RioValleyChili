using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateChileMaterialsReceivedParameters : ISetChileMaterialsReceivedParameters
    {
        internal ICreateChileMaterialsReceivedParameters Params { get; set; }
        
        public ChileProductKey ChileProductKey { get; set; }
        public InventoryTreatmentKey TreatmentKey { get; set; }
        public CompanyKey SupplierKey { get; set; }

        public IEnumerable<SetChileMaterialsReceivedItemParameters> Items { get; set; }

        ChileMaterialsReceivedType ISetChileMaterialsReceivedParameters.ChileMaterialsReceivedType { get { return Params.ChileMaterialsReceivedType; } }
        DateTime ISetChileMaterialsReceivedParameters.DateReceived { get { return Params.DateReceived; } }
        string ISetChileMaterialsReceivedParameters.LoadNumber { get { return Params.LoadNumber; } }
        string ISetChileMaterialsReceivedParameters.PurchaseOrder { get { return Params.PurchaseOrder; } }
        string ISetChileMaterialsReceivedParameters.ShipperNumber { get { return Params.ShipperNumber; } }
    }

    internal class UpdateChileMaterialsReceivedParameters : ISetChileMaterialsReceivedParameters
    {
        internal IUpdateChileMaterialsReceivedParameters Params { get; set; }

        internal LotKey LotKey { get; set; }
        public ChileProductKey ChileProductKey { get; set; }
        public InventoryTreatmentKey TreatmentKey { get; set; }
        public CompanyKey SupplierKey { get; set; }

        public IEnumerable<SetChileMaterialsReceivedItemParameters> Items { get; set; }

        ChileMaterialsReceivedType ISetChileMaterialsReceivedParameters.ChileMaterialsReceivedType { get { return Params.ChileMaterialsReceivedType; } }
        DateTime ISetChileMaterialsReceivedParameters.DateReceived { get { return Params.DateReceived; } }
        string ISetChileMaterialsReceivedParameters.LoadNumber { get { return Params.LoadNumber; } }
        string ISetChileMaterialsReceivedParameters.PurchaseOrder { get { return Params.PurchaseOrder; } }
        string ISetChileMaterialsReceivedParameters.ShipperNumber { get { return Params.ShipperNumber; } }
    }

    internal interface ISetChileMaterialsReceivedParameters
    {
        ChileMaterialsReceivedType ChileMaterialsReceivedType { get; }
        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }

        ChileProductKey ChileProductKey { get; }
        InventoryTreatmentKey TreatmentKey { get; }
        CompanyKey SupplierKey { get; }

        IEnumerable<SetChileMaterialsReceivedItemParameters> Items { get; }
    }
}