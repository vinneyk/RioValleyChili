using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class MoveInventoryCommandParameters
    {
        public IInventoryKey SourceInventoryKey { get; private set; }

        public ILocationKey DestinationLocationKey { get; private set; }

        public int MoveQuantity { get; private set; }

        public MoveInventoryCommandParameters(IInventoryKey sourceInventoryKey, ILocationKey destinationWarehouseLocationKey, int moveQuantity)
        {
            if(sourceInventoryKey == null) { throw new ArgumentNullException("sourceInventoryKey"); }
            if(destinationWarehouseLocationKey == null) { throw new ArgumentNullException("destinationWarehouseLocationKey"); }

            SourceInventoryKey = new InventoryKey(sourceInventoryKey);
            DestinationLocationKey = new LocationKey(destinationWarehouseLocationKey);
            MoveQuantity = moveQuantity;
        }
    }
}