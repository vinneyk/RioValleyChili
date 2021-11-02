using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class CreateSalesOrderConductorParameters : SetSalesOrderParametersBase
    {
        internal sealed override ISetSalesOrderParameters Parameters { get { return CreateParameters; } }
        internal ICreateSalesOrderParameters CreateParameters { get; set; }
        internal CustomerKey CustomerKey { get; set; }
    }

    internal class UpdateSalesOrderCommandParameters : SetSalesOrderParametersBase
    {
        internal sealed override ISetSalesOrderParameters Parameters { get { return UpdateParameters; } }
        internal IUpdateSalesOrderParameters UpdateParameters { get; set; }
        internal SalesOrderKey SalesOrderKey { get; set; }
    }

    internal abstract class SetSalesOrderParametersBase
    {
        internal abstract ISetSalesOrderParameters Parameters { get; }
        internal FacilityKey ShipFromFacilityKey { get; set; }
        internal CompanyKey BrokerKey { get; set; }

        internal List<SetSalesOrderItemParameters> OrderItems { get; set; }
    }
}