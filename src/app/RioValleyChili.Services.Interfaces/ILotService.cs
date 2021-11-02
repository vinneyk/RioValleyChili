using System;
using System.Collections.Generic;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface ILotService
    {
        IResult<ILotQualitySummariesReturn> GetLotSummaries(FilterLotParameters parameters = null);
        IResult<ILotQualitySingleSummaryReturn> GetLotSummary(string lotKey);

        /// <summary>
        /// Creates or updates lot attributes, resolves attributes defects when appropriate and updates the lot status and contaminated fields.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IResult<ILotStatInfoReturn> SetLotAttributes(ISetLotAttributeParameters parameters);

        IResult AddLotAttributes(IAddLotAttributesParameters parameters);
        IResult<ICreateLotDefectReturn> CreateLotDefect(ICreateLotDefectParameters parameters);
        IResult<IResolutionsByDefectTypeReturn> GetDefectResolutions();
        IResult<ILotStatInfoReturn> SetLotHoldStatus(ISetLotHoldStatusParameters parameters);
        IResult<ILotStatInfoReturn> SetLotQualityStatus(ISetLotStatusParameters parameters);
        IResult<ILabReportReturn> GetLabReport(string lotKey);
        IResult<ILabReportReturn> GetLabReport(DateTime minTestDate, DateTime maxTestDate);
        IResult SetLotPackagingReceived(ISetLotPackagingReceivedParameters parameters);
        IResult AddLotAllowance(ILotAllowanceParameters parameters);
        IResult RemoveLotAllowance(ILotAllowanceParameters parameters);
        IResult<ILotHistoryReturn> GetLotHistory(string lotKey);
        IResult<IEnumerable<ILotOutputTraceReturn>> GetOutputTrace(string lotKey);
        IResult<IEnumerable<ILotInputTraceReturn>> GetInputTrace(string lotKey);
    }
}