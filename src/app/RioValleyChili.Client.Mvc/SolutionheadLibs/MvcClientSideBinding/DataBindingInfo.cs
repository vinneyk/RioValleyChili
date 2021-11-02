using System.Collections.Generic;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public class DataBindingInfo
    {
        private DataBindingInfo() { }

        private DataBindingMode bindingMode;
        private string bindingArg = string.Empty;

        public DataBindingMode BindingMode
        {
            get { return bindingMode; }
            private set 
            {
                bindingMode = value;
                bindingArg = KnockoutDataBindingModeHelper.ClientDataBindingModeString(BindingMode);
            }
        }

        public string BindingContext { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", bindingArg, BindingContext); 
        }

        public KeyValuePair<string, object> AsKeyValuePair()
        {
            return new KeyValuePair<string, object>(bindingArg, BindingContext);
        }

        public static DataBindingInfo Create(DataBindingMode mode, string bindingContext)
        {
            return new DataBindingInfo
                       {
                           BindingMode = mode,
                           BindingContext =  bindingContext,
                       };
        }
    }
}