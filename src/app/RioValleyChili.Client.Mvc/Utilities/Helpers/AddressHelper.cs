using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    internal static class AddressHelper
    {
        internal static bool IsEmpty(this Address address)
        {
            return string.IsNullOrWhiteSpace(address.AddressLine1)
                   && string.IsNullOrWhiteSpace(address.AddressLine2)
                   && string.IsNullOrWhiteSpace(address.AddressLine3)
                   && string.IsNullOrWhiteSpace(address.City)
                   && string.IsNullOrWhiteSpace(address.PostalCode)
                   && string.IsNullOrWhiteSpace(address.State);
        }
    }
}