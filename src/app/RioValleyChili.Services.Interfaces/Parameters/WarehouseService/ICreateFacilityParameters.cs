using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.WarehouseService
{
    public interface ICreateFacilityParameters : IUserIdentifiable
    {
        FacilityType FacilityType { get; }
        string Name { get; }
        bool Active { get; }
        string ShippingLabelName { get; }
        string PhoneNumber { get; }
        string EMailAddress { get; }
        Address Address { get; }
    }
}