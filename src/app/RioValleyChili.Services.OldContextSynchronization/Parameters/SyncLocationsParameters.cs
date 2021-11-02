using System.Collections.Generic;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.OldContextSynchronization.Parameters
{
    public class SyncLocationsParameters
    {
        public IEmployeeKey EmployeeKey;
        public List<ILocationKey> Locations;
    }
}