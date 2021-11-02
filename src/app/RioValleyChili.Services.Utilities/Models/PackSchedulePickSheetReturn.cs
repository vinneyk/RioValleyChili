using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns;
using RioValleyChili.Services.Interfaces.Returns.PackScheduleService;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class PackSchedulePickSheetReturn : PackScheduleBaseReturn, IPackSchedulePickSheetReturn
    {
        public DateTime DateCreated { get; internal set; }
        public string SummaryOfWork { get; internal set; }
        public IProductKeyNameReturn ChileProduct { get; internal set; }
        public IEnumerable<IBatchPickedInventoryItemReturn> PickedItems { get; internal set; }
    }
}