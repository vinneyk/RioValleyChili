using System;
using System.Collections.Generic;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.MaterialsReceivedService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ChileMaterialsReceivedReturn : IChileMaterialsReceivedDetailReturn, IChileMaterialsReceivedSummaryReturn
    {
        public string LotKey { get { return LotKeyReturn.LotKey; } }
        public ChileMaterialsReceivedType ChileMaterialsReceivedType { get; internal set; }

        public DateTime DateReceived { get; internal set; }
        public string LoadNumber { get; internal set; }
        public string PurchaseOrder { get; internal set; }
        public string ShipperNumber { get; internal set; }
        public int TotalLoad { get; internal set; }

        public IChileProductReturn ChileProduct { get; internal set; }
        public ICompanySummaryReturn Supplier { get; internal set; }
        public IInventoryTreatmentReturn Treatment { get; internal set; }
        public IEnumerable<IChileMaterialsReceivedItemReturn> Items { get; internal set; }

        #region Internal Parts

        internal LotKeyReturn LotKeyReturn { get; set; }

        #endregion
    }
}