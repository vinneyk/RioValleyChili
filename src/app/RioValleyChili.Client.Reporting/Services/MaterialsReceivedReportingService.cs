using System;
using System.ComponentModel;
using RioValleyChili.Client.Core.Extensions;
using RioValleyChili.Client.Reporting.Models;
using RioValleyChili.Services.Interfaces;

namespace RioValleyChili.Client.Reporting.Services
{
    [DataObject]
    public class MaterialsReceivedReportingService : ReportingService
    {
        public MaterialsReceivedReportingService() : this(ResolveService<IMaterialsReceivedService>()) { }
        
        public MaterialsReceivedReportingService(IMaterialsReceivedService materialsReceivedService)
        {
            if(materialsReceivedService == null) throw new ArgumentNullException("materialsReceivedService");
            _materialsReceivedService = materialsReceivedService;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ChileMaterialsReceivedRecapReportModel GetChileRecapReport(string lotKey)
        {
            var result = _materialsReceivedService.GetChileRecapReport(lotKey);
            if(result.Success)
            {
                return result.ResultingObject.Map().To<ChileMaterialsReceivedRecapReportModel>();
            }

            return null;
        }

        private readonly IMaterialsReceivedService _materialsReceivedService;
    }
}