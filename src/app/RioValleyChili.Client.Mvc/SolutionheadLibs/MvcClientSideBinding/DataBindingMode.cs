using System;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    [Flags]
    public enum DataBindingMode
    {
        None = 0x0,
        Readonly = 0x1,
        Editable = 0x2,
    }
}