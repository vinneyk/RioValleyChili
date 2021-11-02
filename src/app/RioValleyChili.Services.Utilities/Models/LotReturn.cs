// ReSharper disable RedundantExtendsListEntry

using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LotSummaryReturn : LotBaseReturn, ILotSummaryReturn
    {
        public bool? LoBac { get; internal set; }
        public int? AstaCalc { get; internal set; }
        public DateTime AstaCalcDate { get; internal set; }
        public DateTime LotDateCreated { get; internal set; }

        public ICompanyHeaderReturn Customer { get; internal set; }
        public IInventoryProductReturn LotProduct { get; internal set; }
        public IEnumerable<ILotAttributeReturn> Attributes { get; internal set; }
        public IEnumerable<ILotDefectReturn> Defects { get; internal set; }
    }
}

// ReSharper restore RedundantExtendsListEntry