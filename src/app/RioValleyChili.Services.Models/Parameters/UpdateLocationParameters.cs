using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateLocationParameters : IUpdateLocationParameters
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Locked { get; set; }
        public string LocationKey { get; set; }

        internal string UserToken
        {
            get { return ((IUserIdentifiable) this).UserToken; }
            set { ((IUserIdentifiable) this).UserToken = value; }
        }

        string IUserIdentifiable.UserToken { get; set; }
    }
}