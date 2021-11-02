using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Returns.ProductService;

namespace RioValleyChili.Services.Interfaces.Returns.PackScheduleService
{
    public interface IPackSchedulePickSheetReturn : IPackScheduleBaseReturn
    {
        DateTime DateCreated { get;  }
        string SummaryOfWork { get; }

        IProductKeyNameReturn ChileProduct { get; }
        IEnumerable<IBatchPickedInventoryItemReturn> PickedItems { get; }
    }
}