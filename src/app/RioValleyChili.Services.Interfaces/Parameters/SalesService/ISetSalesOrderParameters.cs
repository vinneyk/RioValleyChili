using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.SalesService
{
    public interface ISetSalesOrderParameters : IUserIdentifiable
    {
        string BrokerKey { get; }
        string FacilitySourceKey { get; }
        bool PreShipmentSampleRequired { get; }
        DateTime? InvoiceDate { get; }
        string InvoiceNotes { get; }
        float FreightCharge { get; }

        ISetOrderHeaderParameters HeaderParameters { get; }
        ISetShipmentInformation SetShipmentInformation { get; }

        IEnumerable<ISalesOrderItem> PickOrderItems { get; }
    }
}