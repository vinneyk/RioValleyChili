using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class InterWarehouseOrdersMother : MovementOrdersMotherBase<InventoryShipmentOrder>
    {
        protected override int[] TransTypeIDs { get { return new[] { (int) TransType.Move, (int) TransType.OnConsignment }; } }

        public InterWarehouseOrdersMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback = null) : base(oldContext, newContext, loggingCallback) { }

        protected override IEnumerable<InventoryShipmentOrder> BirthRecords()
        {
            LoadCount.Reset();

            foreach(var order in SelectMovementOrdersToLoad())
            {
                var entityType = GetEntityType(order.TTypeID);
                LoadCount.AddRead(entityType);

                if(order.EntryDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.NullEntryDate)
                        {
                            MovementOrder = order
                        });
                    continue;
                }
                var dateCreated = order.EntryDate.Value.ConvertLocalToUTC().Date;
                var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);

                var sourceWarehouse = NewContextHelper.GetFacility(order.FromWHID);
                if(sourceWarehouse == null)
                {
                    Log(new CallbackParameters(CallbackReason.SourceWarehouseNotLoaded)
                        {
                            MovementOrder = order
                        });
                    continue;
                }

                var destinationWarehouse = NewContextHelper.GetFacility(order.ToWHID);
                if(destinationWarehouse == null)
                {
                    Log(new CallbackParameters(CallbackReason.DestinationWarehouseNotLoaded)
                        {
                            MovementOrder = order
                        });
                    continue;
                }

                var shipmentInformationSequence = NewContextHelper.ShipmentInformationKeys.GetNextSequence(dateCreated);

                var shipmentOrder = SetOrderProperties(new InventoryShipmentOrder
                    {
                        DateCreated = dateCreated,
                        Sequence = sequence,
                        ShipmentInfoDateCreated = dateCreated,
                        ShipmentInfoSequence = shipmentInformationSequence,
                        DestinationFacilityId = destinationWarehouse.Id,
                        SourceFacilityId = sourceWarehouse.Id
                    }, order);
                if(shipmentOrder == null)
                {
                    continue;
                }
                    
                shipmentOrder.ShipmentInformation = CreateShipmentInformation(shipmentOrder, order);
                shipmentOrder.InventoryPickOrder = CreateInventoryPickOrder(shipmentOrder, order);
                shipmentOrder.PickedInventory = CreatePickedInventory(shipmentOrder, order, shipmentOrder.ShipmentInformation.Status);
                if(shipmentOrder.PickedInventory == null)
                {
                    continue;
                }

                var deserialized = SerializableMove.Deserialize(order.Serialized);
                if(deserialized != null)
                {
                    deserialized.SetOrder(shipmentOrder);
                }

                LoadCount.AddLoaded(entityType);
                LoadCount.AddLoaded(EntityTypes.InventoryShipmentOrder);
                LoadCount.AddLoaded(EntityTypes.PickOrder);
                LoadCount.AddLoaded(EntityTypes.PickOrderItem, (uint)shipmentOrder.InventoryPickOrder.Items.Count);
                LoadCount.AddLoaded(EntityTypes.PickedInventory);
                LoadCount.AddLoaded(EntityTypes.PickedInventoryItem, (uint)shipmentOrder.PickedInventory.Items.Count);
                LoadCount.AddLoaded(EntityTypes.ShipmentInformation);

                yield return shipmentOrder;
            }

            LoadCount.LogResults(l => Log(new CallbackParameters(l)));
        }
    }
}