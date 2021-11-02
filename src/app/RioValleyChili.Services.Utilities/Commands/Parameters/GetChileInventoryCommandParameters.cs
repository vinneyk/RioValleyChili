using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class GetChileInventoryCommandParameters 
    {
        public ChileStateEnum? ChileState { get; set; }

        public IFacilityKey FacilityKey { get; set; }

        public IChileProductKey ChileProductKey { get; set; }

        public ILotKey ChileLotKey { get; set; }

        public bool IncludeUnavailable { get; set; }
    }
}