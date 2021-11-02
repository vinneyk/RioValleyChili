using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService
{
    public interface ICreateChileMaterialsReceivedParameters : IUserIdentifiable
    {
        ChileMaterialsReceivedType ChileMaterialsReceivedType { get; }
        DateTime DateReceived { get; }
        string LoadNumber { get; }
        string PurchaseOrder { get; }
        string ShipperNumber { get; }

        string ChileProductKey { get; }
        string TreatmentKey { get; }
        string SupplierKey { get; }

        IEnumerable<ICreateChileMaterialsReceivedItemParameters> Items { get; }
    }
}