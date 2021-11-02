using System;

namespace RioValleyChili.Core
{
    public enum TransType
    {
        DeHy = 1,
        MnW = 2,
        Batching = 3,
        Production = 4,
        Move = 5,
        Sale = 6,
        Rework = 7,
        ToTrmt = 8,
        FrmTrmt = 9,
        InvAdj = 10,
        Ingredients = 11,
        Packaging = 12,
        Rincon = 13,
        OnConsignment = 14,
        ConsignmentSale = 15,
        MiscInvoice = 16,
        Other = 17,
        GRP = 18,
        Quote = 19
    }

    public static class TransTypeExtensions
    {
        public static PickedReason ToPickedReason(this TransType transType)
        {
            switch(transType)
            {
                case TransType.MnW: return PickedReason.Production;
                case TransType.Batching: return PickedReason.Production;
                case TransType.Move: return PickedReason.InterWarehouseMovement;
                case TransType.Sale: return PickedReason.SalesOrder;
                case TransType.ToTrmt: return PickedReason.TreatmentOrder;
                case TransType.Rincon: return PickedReason.IntraWarehouseMovement;
                case TransType.OnConsignment: return PickedReason.ConsignmentOrder;
                default: throw new ArgumentOutOfRangeException(string.Format("TransType[{0}] has no associated PickedReason defined."));
            }
        }

        public static TransType? ToTransType(this int? TTypeID)
        {
            try
            {
                return (TransType?) TTypeID.Value;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}