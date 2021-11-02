using RioValleyChili.Core;
using RioValleyChili.Core.Models;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateFacilityParameters : ICreateFacilityParameters
    {
        public FacilityType FacilityType { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string ShippingLabelName { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public Address Address { get; set; }

        string IUserIdentifiable.UserToken { get; set; }

        internal string UserToken
        {
            get { return ((IUserIdentifiable)this).UserToken; }
            set { ((IUserIdentifiable) this).UserToken = value; }
        }
    }
}