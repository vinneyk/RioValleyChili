using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSampleSpecsCommand
    {
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;

        internal SetSampleSpecsCommand(ISampleOrderUnitOfWork sampleOrderUnitOfWork)
        {
            if(sampleOrderUnitOfWork == null) { throw new ArgumentNullException("sampleOrderUnitOfWork"); }
            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
        }

        internal IResult<SampleOrderItemSpec> Execute(SetSampleSpecsCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            var sampleOrderItem = _sampleOrderUnitOfWork.SampleOrderItemRepository.FindByKey(parameters.SampleOrderItemKey, i => i.Spec);
            if(sampleOrderItem == null)
            {
                return new InvalidResult<SampleOrderItemSpec>(null, string.Format(UserMessages.SampleOrderItemNotFound, parameters.SampleOrderItemKey));
            }

            var specs = sampleOrderItem.Spec;
            if(specs == null)
            {
                specs = _sampleOrderUnitOfWork.SampleOrderItemSpecRepository.Add(new SampleOrderItemSpec
                    {
                        SampleOrderYear = parameters.SampleOrderItemKey.SampleOrderKey_Year,
                        SampleOrderSequence = parameters.SampleOrderItemKey.SampleOrderKey_Sequence,
                        ItemSequence = parameters.SampleOrderItemKey.SampleOrderItemKey_Sequence,
                    });
            }

            specs.Notes = parameters.Parameters.Notes;
            specs.AstaMin = parameters.Parameters.AstaMin;
            specs.AstaMax = parameters.Parameters.AstaMax;
            specs.MoistureMin = parameters.Parameters.MoistureMin;
            specs.MoistureMax = parameters.Parameters.MoistureMax;
            specs.WaterActivityMin = parameters.Parameters.WaterActivityMin;
            specs.WaterActivityMax = parameters.Parameters.WaterActivityMax;
            specs.Mesh = parameters.Parameters.Mesh;
            specs.AoverB = parameters.Parameters.AoverB;
            specs.ScovMin = parameters.Parameters.ScovMin;
            specs.ScovMax = parameters.Parameters.ScovMax;
            specs.ScanMin = parameters.Parameters.ScanMin;
            specs.ScanMax = parameters.Parameters.ScanMax;
            specs.TPCMin = parameters.Parameters.TPCMin;
            specs.TPCMax = parameters.Parameters.TPCMax;
            specs.YeastMin = parameters.Parameters.YeastMin;
            specs.YeastMax = parameters.Parameters.YeastMax;
            specs.MoldMin = parameters.Parameters.MoldMin;
            specs.MoldMax = parameters.Parameters.MoldMax;
            specs.ColiformsMin = parameters.Parameters.ColiformsMin;
            specs.ColiformsMax = parameters.Parameters.ColiformsMax;
            specs.EColiMin = parameters.Parameters.EColiMin;
            specs.EColiMax = parameters.Parameters.EColiMax;
            specs.SalMin = parameters.Parameters.SalMin;
            specs.SalMax = parameters.Parameters.SalMax;

            return new SuccessResult<SampleOrderItemSpec>(specs);
        }
    }
}