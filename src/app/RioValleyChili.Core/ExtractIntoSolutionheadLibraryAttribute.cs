using System;

namespace RioValleyChili.Core
{
    [ExtractIntoSolutionheadLibrary]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ExtractIntoSolutionheadLibraryAttribute : Attribute
    {
        public ExtractIntoSolutionheadLibraryAttribute(string notes = null) {}
    }
}
