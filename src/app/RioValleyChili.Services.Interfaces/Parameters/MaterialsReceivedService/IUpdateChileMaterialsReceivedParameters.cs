using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService
{
    public interface IUpdateChileMaterialsReceivedParameters : IUserIdentifiable
    {
        string LotKey { get; }
        ChileMaterialsReceivedType ChileMaterialsReceivedType { get; }
        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }

        string ChileProductKey { get; }
        string TreatmentKey { get; }
        string SupplierKey { get; }

        IEnumerable<IUpdateChileMaterialsReceivedItemParameters> Items { get; }
    }
}