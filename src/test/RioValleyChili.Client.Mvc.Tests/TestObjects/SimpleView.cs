using System.IO;
using System.Web.Mvc;

namespace RioValleyChili.Tests.TestObjects
{
    public class SimpleView : IView
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            writer.Write(viewContext.ViewData.ModelMetadata.ModelType.Name);
        }
    }
}