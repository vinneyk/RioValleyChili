using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class TreatmentOrdersMother : MovementOrdersMotherBase<TreatmentOrder>
    {
        protected override int[] TransTypeIDs { get { return new[] { (int) TransType.ToTrmt }; } }

        public TreatmentOrdersMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback = null) : base(oldContext, newContext, loggingCallback) { }

        protected override OrderStatus GetOrderStatus(tblOrderStatus? orderStatus)
        {
            switch(orderStatus)
            {
                case tblOrderStatus.Void: return OrderStatus.Void;
                case tblOrderStatus.Treated: return OrderStatus.Fulfilled;
                default: return OrderStatus.Scheduled;
            }
        }

        protected override IEnumerable<TreatmentOrder> BirthRecords()
        {
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

                var sourceFacility = NewContextHelper.GetFacility(order.FromWHID);
                if(sourceFacility == null)
                {
                    Log(new CallbackParameters(CallbackReason.SourceWarehouseNotLoaded)
                        {
                            MovementOrder = order
                        });
                    continue;
                }

                var destinationFacility = NewContextHelper.GetFacility(order.ToWHID);
                if(destinationFacility == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentFaciltyNotLoaded)
                        {
                            MovementOrder = order
                        });
                    continue;
                }
                destinationFacility.FacilityType = FacilityType.Treatment;

                var deserialized = SerializableMove.Deserialize(order.Serialized);

                var treatment = DeterminedOrderTreatment(order, deserialized);
                if(treatment == null)
                {
                    Log(new CallbackParameters(CallbackReason.TreatmentNotDetermined)
                        {
                            MovementOrder = order
                        });
                    continue;
                }

                var shipmentDate = order.Date.GetDate() ?? order.EntryDate.GetDate();
                if(shipmentDate == null)
                {
                    Log(new CallbackParameters(CallbackReason.UndeterminedShipmentDate)
                        {
                            MovementOrder = order
                        });
                    continue;
                }

                var sequence = PickedInventoryKeyHelper.Singleton.GetNextSequence(dateCreated);
                var shipmentOrder = SetOrderProperties(new InventoryShipmentOrder
                    {
                        DateCreated = dateCreated,
                        Sequence = sequence,
                        ShipmentInfoDateCreated = dateCreated,
                        ShipmentInfoSequence = NewContextHelper.ShipmentInformationKeys.GetNextSequence(dateCreated),
                        DestinationFacilityId = destinationFacility.Id,
                        SourceFacilityId = sourceFacility.Id
                    }, order);
                if(shipmentOrder == null)
                {
                    continue;
                }

                var treatmentOrder = new TreatmentOrder
                    {
                        DateCreated = dateCreated,
                        Sequence = sequence,

                        InventoryTreatmentId = treatment.InventoryTreatmentKey_Id,
                        InventoryShipmentOrder = shipmentOrder,
                        Returned = order.Returned.ConvertLocalToUTC()
                    };
                treatmentOrder.InventoryShipmentOrder.ShipmentInformation = CreateShipmentInformation(treatmentOrder.InventoryShipmentOrder, order);
                treatmentOrder.InventoryShipmentOrder.InventoryPickOrder = CreateInventoryPickOrder(treatmentOrder.InventoryShipmentOrder, order);
                treatmentOrder.InventoryShipmentOrder.PickedInventory = CreatePickedInventory(treatmentOrder.InventoryShipmentOrder, order, treatmentOrder.InventoryShipmentOrder.ShipmentInformation.Status);
                if(treatmentOrder.InventoryShipmentOrder.PickedInventory == null)
                {
                    continue;
                }
                treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.ForEach(i => i.TreatmentId = NewContextHelper.NoTreatment.Id);

                if(deserialized != null)
                {
                    deserialized.SetOrder(treatmentOrder);
                }

                LoadCount.AddLoaded(entityType);
                LoadCount.AddLoaded(EntityTypes.InventoryShipmentOrder);
                LoadCount.AddLoaded(EntityTypes.PickOrder);
                LoadCount.AddLoaded(EntityTypes.PickOrderItem, (uint) treatmentOrder.InventoryShipmentOrder.InventoryPickOrder.Items.Count);
                LoadCount.AddLoaded(EntityTypes.PickedInventory);
                LoadCount.AddLoaded(EntityTypes.PickedInventoryItem, (uint) treatmentOrder.InventoryShipmentOrder.PickedInventory.Items.Count);
                LoadCount.AddLoaded(EntityTypes.ShipmentInformation);

                yield return treatmentOrder;
            }

            LoadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private IInventoryTreatmentKey DeterminedOrderTreatment(MovementOrderDTO order, SerializableMove deserialized)
        {
            if(deserialized != null && deserialized.TreatmentOrder != null && deserialized.TreatmentOrder.TreatmentKey != null)
            {
                return deserialized.TreatmentOrder.TreatmentKey;
            }

            int treatmentId;
            if(order.ToWHID == null || !_treatmentIdsByFacility.TryGetValue(order.ToWHID.Value, out treatmentId))
            {
                treatmentId = order.tblOutgoings.Where(o => o.TTypeID == (int) TransType.FrmTrmt)
                                       .Select(o => o.TrtmtID)
                                       .Distinct().FirstOrDefault(i => i != 0);
                if(treatmentId == 0)
                {
                    return null;
                }
            }

            return NewContextHelper.GetInventoryTreatment(treatmentId);
        }

        private readonly Dictionary<int, int> _treatmentIdsByFacility = new Dictionary<int, int>
            {
                { 28, 1 }, //Cosmed, ET
                { 9, 1 }, //IBA, ET
                { 4, 3 } //Isomedix, GT
            };
    }
}