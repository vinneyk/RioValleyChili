using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;

namespace RioValleyChili.Services.Tests.IntegrationTests.Parameters
{
    public class ChileMaterialsReceivedParameters : IUpdateChileMaterialsReceivedParameters, ICreateChileMaterialsReceivedParameters
    {
        public string LotKey { get; set; }
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; set; }
        public string UserToken { get; set; }
        public DateTime DateReceived { get; set; }
        public string LoadNumber { get; set; }
        public string PurchaseOrder { get; set; }
        public string ShipperNumber { get; set; }
        public string SupplierKey { get; set; }
        public string ChileProductKey { get; set; }
        public string TreatmentKey { get; set; }

        public List<ChileMaterialsReceivedItemParameters> Items { get; set; }
        IEnumerable<ICreateChileMaterialsReceivedItemParameters> ICreateChileMaterialsReceivedParameters.Items { get { return Items; } }
        IEnumerable<IUpdateChileMaterialsReceivedItemParameters> IUpdateChileMaterialsReceivedParameters.Items { get { return Items; } }

        public ChileMaterialsReceivedParameters() { }

        public ChileMaterialsReceivedParameters(ChileMaterialsReceived received)
        {
            LotKey = received.ToLotKey();
            ChileMaterialsReceivedType = received.ChileMaterialsReceivedType;
            UserToken = received.Employee.UserName;
            DateReceived = received.DateReceived;
            SupplierKey = received.ToCompanyKey();
            LoadNumber = received.LoadNumber;
            PurchaseOrder = received.ChileLot.Lot.PurchaseOrderNumber;
            ShipperNumber = received.ChileLot.Lot.ShipperNumber;
            ChileProductKey = received.ToChileProductKey();
            TreatmentKey = received.ToInventoryTreatmentKey();
            Items = received.Items.Select(i => new ChileMaterialsReceivedItemParameters(i)).ToList();
        }
    }
}