using System;
using System.Linq.Expressions;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class ShipmentInformationProjectors
    {
        internal static Expression<Func<ShipmentInformation, ShipmentInformationKeyReturn>> SelectShipmentInfoKey()
        {
            return s => new ShipmentInformationKeyReturn
            {
                ShipmentInfoKey_DateCreated = s.DateCreated,
                ShipmentInfoKey_Sequence = s.Sequence
            };
        }

        internal static Expression<Func<ShipmentInformation, ShipmentReturn>> SelectSummary()
        {
            var shipmentInfoKey = SelectShipmentInfoKey();

            return s => new ShipmentReturn
                {
                    ShipmentInformationKeyReturn = shipmentInfoKey.Invoke(s),
                    Status = s.Status,
                    ShipmentDate = s.ShipmentDate
                };
        }

        internal static Expression<Func<ShipmentInformation, ShipmentReturn>> SelectDetail(InventoryOrderEnum inventoryOrderEnum)
        {
            return SelectShipmentInformation().Merge(s => new ShipmentReturn
                {
                    InventoryOrderEnum = inventoryOrderEnum
                });
        }

        internal static Expression<Func<ShipmentInformation, ShipmentReturn>> SelectShipmentInformation()
        {
            return SelectSummary().Merge(s => new ShipmentReturn
                {
                    PalletWeight = s.PalletWeight,
                    PalletQuantity = s.PalletQuantity,

                    TransitInformation = new TransitInformation
                        {
                            ShipmentMethod = s.ShipmentMethod,
                            FreightType = s.FreightBillType,
                            DriverName = s.DriverName,
                            CarrierName = s.CarrierName,
                            TrailerLicenseNumber = s.TrailerLicenseNumber,
                            ContainerSeal = s.ContainerSeal
                        },
                    ShippingInstructions = new ShippingInstructions
                        {
                            RequiredDeliveryDateTime = s.RequiredDeliveryDate,
                            ShipmentDate = s.ShipmentDate,

                            InternalNotes = s.InternalNotes,
                            ExternalNotes = s.ExternalNotes,
                            SpecialInstructions = s.SpecialInstructions,

                            ShipFromOrSoldToShippingLabel = s.ShipFrom,
                            ShipToShippingLabel = s.ShipTo,
                            FreightBillToShippingLabel = s.FreightBill
                        }
                });
        }
    }
}