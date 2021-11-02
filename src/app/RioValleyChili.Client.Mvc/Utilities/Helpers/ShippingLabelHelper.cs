using RioValleyChili.Core.Models;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    internal static class ShippingLabelHelper
    {
        internal static bool IsEmpty(this ShippingLabel label)
        {
            return (label != null && (label.Address == null || label.Address.IsEmpty()))
                   && string.IsNullOrWhiteSpace(label.Name)
                   && string.IsNullOrWhiteSpace(label.Phone)
                   && string.IsNullOrWhiteSpace(label.EMail)
                   && string.IsNullOrWhiteSpace(label.Fax);
        }
    }
}