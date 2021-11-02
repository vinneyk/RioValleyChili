using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateChileMaterialsReceivedParameters : ICreateChileMaterialsReceivedParameters
    {
        public string UserToken { get; set; }
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }
        public string ChileProductKey { get; set; }
        public string SupplierKey { get; set; }
        public string TreatmentKey { get; set; }

        public IEnumerable<CreateChileMaterialsReceivedItemParameters> Items { get; set; }
        IEnumerable<ICreateChileMaterialsReceivedItemParameters> ICreateChileMaterialsReceivedParameters.Items { get { return Items; } }
    }
}