using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryShipmentOrderService;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class InventoryShipmentOrderCertificateOfAnalysisReturn : IInventoryShipmentOrderCertificateOfAnalysisReturn
    {
        public string DestinationName { get; internal set; }
        public string OrderKey { get { return OrderKeyReturn.InventoryShipmentOrderKey; } }
        public int? MovementNumber { get; internal set; }
        public string PurchaseOrderNumber { get; internal set; }
        public DateTime? ShipmentDate { get; internal set; }
        public InventoryShipmentOrderTypeEnum OrderType { get; internal set; }

        public IEnumerable<IInventoryShipmentOrderItemAnalysisReturn> Items
        {
            get
            {
                return _Items.GroupBy(i => new
                    {
                        i.LotKey,
                        i.TreatmentReturn.TreatmentKey
                    }).Select(g => g.First());
            }
        }
        
        internal IEnumerable<IInventoryShipmentOrderItemAnalysisReturn> _Items { get; set; }
        internal InventoryShipmentOrderKeyReturn OrderKeyReturn { get; set; }
    }
}