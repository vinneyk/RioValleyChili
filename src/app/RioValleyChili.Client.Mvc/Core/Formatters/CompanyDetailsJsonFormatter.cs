using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core.Utilities;
using RioValleyChili.Services.Interfaces.Returns.CompanyService;

namespace RioValleyChili.Client.Mvc.Core.Formatters
{
    public class CompanyDetailsJsonFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private const string JSON = "application/json";
        private readonly CompanyDetailsBuilder _companyDetailsBuilder = new CompanyDetailsBuilder();

        public CompanyDetailsJsonFormatter(JsonSerializerSettings settings = null)
        {
            _jsonSerializerSettings = settings ?? new JsonSerializerSettings();
            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(JSON));
        }

        public override bool CanReadType(Type type)
        {
            return SupportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            var canWrite = SupportedType(type);
            return canWrite;
        }

        private static bool SupportedType(Type type)
        {
            return type == typeof(ICompanyDetailReturn)
                || type == typeof(IEnumerable<ICompanyDetailReturn>);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            var serializer = JsonSerializer.Create(_jsonSerializerSettings);
            return Task.Factory.StartNew(() =>
            {
                using (var jsonWriter = new JsonTextWriter(new StreamWriter(writeStream)))
                {
                    serializer.Serialize(jsonWriter, _companyDetailsBuilder.BuildCompanyDetailsObject(value as ICompanyDetailReturn));
                    jsonWriter.Flush();
                }
            });
        }
    }
}