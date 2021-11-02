using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EF_Projectors;
using EF_Projectors.Extensions;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class TreatmentOrderProjectors
    {
        internal static Expression<Func<TreatmentOrder, TreatmentOrderKeyReturn>> SelectKey()
        {
            return t => new TreatmentOrderKeyReturn
                {
                    InventoryShipmentOrderKey_DateCreated = t.DateCreated,
                    InventoryShipmentOrderKey_Sequence = t.Sequence
                };
        }

        internal static Expression<Func<TreatmentOrder, TreatmentOrderSummaryReturn>> SelectSummary()
        {
            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return InventoryShipmentOrderProjectors.SelectInventoryShipmentOrderSummary().Merge(Projector<TreatmentOrder>.To(t => new TreatmentOrderSummaryReturn
                    {
                        Returned = t.Returned,
                        InventoryTreatment = treatment.Invoke(t.Treatment)
                    }), t => t.InventoryShipmentOrder);

        }

        internal static IEnumerable<Expression<Func<TreatmentOrder, TreatmentOrderDetailReturn>>> SplitSelectDetail(ITreatmentOrderUnitOfWork treatmentOrderUnitOfWork, DateTime currentDate)
        {
            if(treatmentOrderUnitOfWork == null) { throw new ArgumentNullException("treatmentOrderUnitOfWork"); }

            var treatment = InventoryTreatmentProjectors.SelectInventoryTreatment();

            return new Projectors<TreatmentOrder, TreatmentOrderDetailReturn>
                {
                    { InventoryShipmentOrderProjectors.SplitSelectInventoryShipmentOrderDetail(treatmentOrderUnitOfWork, currentDate, InventoryOrderEnum.Treatments), p => p.Translate().To<TreatmentOrder, TreatmentOrderDetailReturn>(t => t.InventoryShipmentOrder) },
                    t => new TreatmentOrderDetailReturn
                        {
                            Returned = t.Returned,
                            InventoryTreatment = treatment.Invoke(t.Treatment)
                        }
                };
        }
    }
}