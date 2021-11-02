using System;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers
{
    public static class KnockoutDataBindingModeHelper
    {
        public static string ClientDataBindingModeString(DataBindingMode mode)
        {
            switch (mode)
            {
                case DataBindingMode.Editable: return "value";
                case DataBindingMode.Readonly: return "text";
                default: throw new NotSupportedException(string.Format("The ClientDataBindingMode '{0}' is not supported.", mode));
            }
        }
    }
}