// ReSharper disable RedundantExtendsListEntry

using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductionResultsService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class ProductionResultReturn : ProductionResultBaseReturn, IProductionResultDetailReturn, IProductionResultSummaryReturn
    {
        public string ProductionResultKey { get { return LotKeyReturn.LotKey; } }

        public string ProductionBatchKey { get { return LotKeyReturn.LotKey; } }

        public string OutputLotNumber { get { return LotKeyReturn.LotKey; } }

        public string ProductionLineKey { get { return ProductionLocationKeyReturn.LocationKey; } }

        public string ChileProductKey { get { return ChileProductKeyReturn.ProductKey; } }

        public string User { get; set; }

        public DateTime DateTimeEntered { get; set; }

        public DateTime ProductionStartDate { get; set; }

        public string ChileProductName { get; set; }

        public double TargetBatchWeight { get; set; }

        public string WorkType { get; set; }

        public string BatchStatus { get; set; } // enum?

        public IEnumerable<IProductionResultItemReturn> ResultItems { get; set; }

        #region Internal Parts

        internal LotKeyReturn LotKeyReturn { get; set; }

        internal LocationKeyReturn ProductionLocationKeyReturn { get; set; }

        internal ProductKeyReturn ChileProductKeyReturn { get; set; }

        #endregion
    }
}

// ReSharper restore RedundantExtendsListEntry