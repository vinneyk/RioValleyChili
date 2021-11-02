using System;

namespace RioValleyChili.Data.Models.Interfaces
{
    public interface IOwnerIdentifiable
    {
        string User { get; set; }

        DateTime TimeStamp { get; set; }
    }
}
