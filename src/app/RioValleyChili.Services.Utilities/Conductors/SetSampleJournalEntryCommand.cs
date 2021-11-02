using System;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetSampleJournalEntryCommand
    {
        private readonly ISampleOrderUnitOfWork _sampleOrderUnitOfWork;

        internal SetSampleJournalEntryCommand(ISampleOrderUnitOfWork sampleOrderUnitOfWork)
        {
            if(sampleOrderUnitOfWork == null) { throw new ArgumentNullException("sampleOrderUnitOfWork"); }
            _sampleOrderUnitOfWork = sampleOrderUnitOfWork;
        }

        internal IResult<SampleOrderJournalEntry> Execute(SetSampleJournalEntryCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_sampleOrderUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<SampleOrderJournalEntry>();
            }

            var sampleOrder = _sampleOrderUnitOfWork.SampleOrderRepository.FindByKey(parameters.SampleOrderKey, o => o.JournalEntries);
            if(sampleOrder == null)
            {
                return new InvalidResult<SampleOrderJournalEntry>(null, string.Format(UserMessages.SampleOrderNotFound, parameters.SampleOrderKey));
            }

            SampleOrderJournalEntry journalEntry;
            if(parameters.JournalEntryKey != null)
            {
                journalEntry = sampleOrder.JournalEntries.FirstOrDefault(parameters.JournalEntryKey.FindByPredicate.Compile());
                if(journalEntry == null)
                {
                    return new InvalidResult<SampleOrderJournalEntry>(null, string.Format(UserMessages.SampleOrderJournalEntryNotFound, parameters.JournalEntryKey));
                }
            }
            else
            {
                journalEntry = _sampleOrderUnitOfWork.SampleOrderJournalEntryRepository.Add(new SampleOrderJournalEntry
                    {
                        SampleOrderYear = sampleOrder.Year,
                        SampleOrderSequence = sampleOrder.Sequence,
                        EntrySequence = sampleOrder.JournalEntries.Select(j => j.EntrySequence).DefaultIfEmpty(0).Max() + 1
                    });
            }

            journalEntry.EmployeeId = employeeResult.ResultingObject.EmployeeId;
            journalEntry.Date = parameters.Parameters.Date;
            journalEntry.Text = parameters.Parameters.Text;

            return new SuccessResult<SampleOrderJournalEntry>(journalEntry);
        }
    }
}