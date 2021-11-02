namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Models
{
    public class KnockoutDataBindingObject
    {
        public KnockoutDataBindingObject(DataBindingAttributeDictionary dataBindingAttributes, object additionalViewData)
        {
            DataBindingAttributes = dataBindingAttributes;
            AdditionalViewData = additionalViewData;
        }

        public DataBindingAttributeDictionary DataBindingAttributes { get; set; }

        public object AdditionalViewData { get; set; }
    }
}