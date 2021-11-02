using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateChileMaterialsReceivedParameters : IUpdateChileMaterialsReceivedParameters
    {
        public string LotKey { get; set; }
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        public string UserToken { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }
        
        public string ChileProductKey { get; set; }
        public string TreatmentKey { get; set; }
        public string SupplierKey { get; set; }

        public IEnumerable<UpdateChileMaterialsReceivedItemParameters> Items { get; set; }
        IEnumerable<IUpdateChileMaterialsReceivedItemParameters> IUpdateChileMaterialsReceivedParameters.Items { get { return Items; } }
    }
}