using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using RioValleyChili.Client.Mvc.Core.Messaging;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Utility for assisting with the many different architecture styles in the application. Specifically, 
        /// the use of shared Knockout script templates which need to be utilized by a webpack composed script. 
        /// This enables the contents of the old script template partial razor views to be extracted into an html 
        /// file and referenced by the newer webpack-centric front-end architecture.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="fileName"></param>
        public static void RenderPartialScriptTemplate(this HtmlHelper htmlHelper, string fileName)
        {
            // Note: in order to prevent the need for setting permissions on the production server, we copy the template files
            // into the App_Data folder at compilation time. However, in order to prevent the need to recompile in order to 
            // see updates to the template files, we use the following compiler directive to determine which folder to read
            // from based on the build profile.
#if DEBUG
            const string folder = "App";
#else 
            const string  folder = "App_Data";
#endif
            var path = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, folder, "templates", fileName);
            htmlHelper.ViewContext.HttpContext.Response.WriteFile(path);
        }

        public static IDictionary<string, object> ToHtmlAttributeDictionary(this object input)
        {
            var dictionary = new RouteValueDictionary(input);
            dictionary.Where(attr => attr.Key.Contains("_")).ToList()
                .ForEach(attr =>
                {
                    dictionary.Add(attr.Key.Replace("_", "-"), attr.Value);
                    dictionary.Remove(attr.Key);
                });

            return dictionary;
        }

        public static MvcHtmlString UserMessage(this HtmlHelper htmlHelper)
        {
            var messengerService = new HtmlHelperMessengerServiceHelper(htmlHelper);

            var tagBuilder = new TagBuilder("div");
            tagBuilder.GenerateId("usermsg");
            tagBuilder.AddCssClass("usermsg");

            var message = messengerService.GetMessage();
            if (message != null)
            {
                tagBuilder.InnerHtml = message.Message;

                switch (message.MessageType)
                {
                    case MessageType.Error:
                        tagBuilder.AddCssClass("error");
                        break;
                    case MessageType.Warning:
                        tagBuilder.AddCssClass("warning");
                        break;
                    case MessageType.YesNo:
                        tagBuilder.AddCssClass("yesno");
                        break;
                    default:
                        tagBuilder.AddCssClass("basic");
                        break;
                }   
            }

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        /// <summary>
        /// Returns a raw JSON encoded string with doubly encoded escape sequences. This is necessary for the JSON parser.
        /// </summary>
        public static IHtmlString RawJson(this HtmlHelper htmlHelper, object value)
        {
            var rawJson = htmlHelper.Raw(Json.Encode(value)).ToHtmlString().Replace("\\", "\\\\");
            return new HtmlString(rawJson);
        }

        public static void SetPageTitles(this HtmlHelper htmlHelper, string htmlTitle, string pageTitle, string pageSubTitle = null, string pageCaption = null)
        {
            htmlHelper.ViewBag.Title = htmlTitle;
            htmlHelper.ViewBag.PageTitle = pageTitle;
            htmlHelper.ViewBag.PageSubTitle = pageSubTitle;
            htmlHelper.ViewBag.PageCaption = pageCaption;
        }

        [Obsolete("Use WebpackLayout or WebpackHtmlHead instead.")]
        public static void UseWebPack(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewBag.useWebPack = true;
        } 

        public static void PushJavaScriptResource(this HtmlHelper htmlHelper, string filePath)
        {
            var jsResources = htmlHelper.ViewContext.TempData["__JSResources"] as List<string> 
                ?? (List<string>)(htmlHelper.ViewContext.TempData["__JSResources"] = new List<string>());

            if (!jsResources.Contains(filePath))
            {
                jsResources.Add(filePath);
            }
        }

        public static MvcHtmlString RenderJavaScriptIncludes(this HtmlHelper htmlHelper)
        {
            var jsResources = htmlHelper.ViewContext.TempData["__JSResources"] as IEnumerable<string>;
            if (jsResources == null) return new MvcHtmlString(string.Empty);

            var stringBuilder = new StringBuilder();

            foreach (var path in jsResources)
            {
                stringBuilder.AppendFormat("<script src=\"{0}\"></script>", path);
            }

            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static void PushJavaScript(this HtmlHelper htmlHelper, MvcHtmlString script)
        {
            PushJavaScript(htmlHelper, script.ToHtmlString());
        }

        public static void PushJavaScript(this HtmlHelper htmlHelper, string script)
        {
            var js = htmlHelper.ViewContext.TempData["__JSEmbedded"] as List<string>
                     ?? (List<string>)(htmlHelper.ViewContext.TempData["__JSEmbedded"] = new List<string>());

            if (!js.Contains(script))
            {
                js.Add(script);
            }
        }

        public static MvcHtmlString RenderEmbeddedJavaScript(this HtmlHelper htmlHelper)
        {
            //embedded 
            var jsEmbedded = htmlHelper.ViewContext.TempData["__JSEmbedded"] as IEnumerable<string>;
            if (jsEmbedded == null) return new MvcHtmlString(string.Empty);

            var scriptTag = new TagBuilder("script");
            var stringBuilder = new StringBuilder();
            foreach (var js in jsEmbedded)
            {
                stringBuilder.Append(js);
            }

            scriptTag.InnerHtml = stringBuilder.ToString();
            return new MvcHtmlString(scriptTag.ToString());
        }
    }
}