using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSampleMatchCommand
    {
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;

        internal SetSampleMatchCommand(ISampleOrderUnitOfWork sampleOrderUnitOfWork)
        {
            if(sampleOrderUnitOfWork == null) { throw new ArgumentNullException("sampleOrderUnitOfWork"); }
            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
        }

        internal IResult<SampleOrderItemMatch> Execute(SetSampleMatchCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            var sampleOrderItem = _sampleOrderUnitOfWork.SampleOrderItemRepository.FindByKey(parameters.SampleOrderItemKey, i => i.Match);
            if(sampleOrderItem == null)
            {
                return new InvalidResult<SampleOrderItemMatch>(null, string.Format(UserMessages.SampleOrderItemNotFound, parameters.SampleOrderItemKey));
            }

            var match = sampleOrderItem.Match;
            if(match == null)
            {
                match = _sampleOrderUnitOfWork.SampleOrderItemMatchRepository.Add(new SampleOrderItemMatch
                    {
                        SampleOrderYear = parameters.SampleOrderItemKey.SampleOrderKey_Year,
                        SampleOrderSequence = parameters.SampleOrderItemKey.SampleOrderKey_Sequence,
                        ItemSequence = parameters.SampleOrderItemKey.SampleOrderItemKey_Sequence,
                    });
            }

            match.Notes = parameters.Parameters.Notes;
            match.Gran = parameters.Parameters.Gran;
            match.AvgAsta = parameters.Parameters.AvgAsta;
            match.AoverB = parameters.Parameters.AoverB;
            match.AvgScov = parameters.Parameters.AvgScov;
            match.H2O = parameters.Parameters.H2O;
            match.Scan = parameters.Parameters.Scan;
            match.Yeast = parameters.Parameters.Yeast;
            match.Mold = parameters.Parameters.Mold;
            match.Coli = parameters.Parameters.Coli;
            match.TPC = parameters.Parameters.TPC;
            match.EColi = parameters.Parameters.EColi;
            match.Sal = parameters.Parameters.Sal;
            match.InsPrts = parameters.Parameters.InsPrts;
            match.RodHrs = parameters.Parameters.RodHrs;

            return new SuccessResult<SampleOrderItemMatch>(match);
        }
    }
}