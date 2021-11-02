using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class DeleteSampleOrderConductor
    {
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;

        internal DeleteSampleOrderConductor(ISampleOrderUnitOfWork sampleOrderUnitOfWork)
        {
            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
        }

        public IResult Execute(SampleOrderKey sampleOrderKey, out int? sampleId)
        {
            if(sampleOrderKey == null) { throw new ArgumentNullException("sampleOrderKey"); }
            sampleId = null;

            var sampleOrder = _sampleOrderUnitOfWork.SampleOrderRepository.FindByKey(sampleOrderKey,
                o => o.JournalEntries,
                o => o.Items.Select(i => i.Spec),
                o => o.Items.Select(i => i.Match));
            if(sampleOrder == null)
            {
                return new NoWorkRequiredResult();
            }

            sampleId = sampleOrder.SampleID;

            foreach(var item in sampleOrder.Items.ToList())
            {
                if(item.Spec != null)
                {
                    _sampleOrderUnitOfWork.SampleOrderItemSpecRepository.Remove(item.Spec);
                }
                if(item.Match != null)
                {
                    _sampleOrderUnitOfWork.SampleOrderItemMatchRepository.Remove(item.Match);
                }

                _sampleOrderUnitOfWork.SampleOrderItemRepository.Remove(item);
            }

            foreach(var entry in sampleOrder.JournalEntries.ToList())
            {
                _sampleOrderUnitOfWork.SampleOrderJournalEntryRepository.Remove(entry);
            }

            _sampleOrderUnitOfWork.SampleOrderRepository.Remove(sampleOrder);

            return new SuccessResult();
        }
    }
}