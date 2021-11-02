using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class TagBuilderHelper
    {
        public static void ApplyHtmlAttributes(this TagBuilder htmlContainer, object htmlAttributes)
        {
            if (htmlAttributes == null) return;

            var attributes = htmlAttributes.ToHtmlAttributeDictionary();
            if (attributes.ContainsKey("class"))
            {
                htmlContainer.AddCssClass(attributes["class"] as string);
                attributes.Remove("class");
            }

            htmlContainer.MergeAttributes(attributes);
        }
    }
}