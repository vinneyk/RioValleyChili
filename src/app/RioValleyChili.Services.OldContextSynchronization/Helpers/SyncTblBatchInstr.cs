using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class SyncTblBatchInstr
    {
        private readonly RioAccessSQLEntities _oldContext;

        public SyncTblBatchInstr(RioAccessSQLEntities oldContext)
        {
            _oldContext = oldContext;
        }

        public void Synchronize(Notebook instructionNotebook, int lotNumber)
        {
            var removeOldInstructions = _oldContext.tblBatchInstrs.Where(i => i.Lot == lotNumber).ToList();
            var addNewInstructions = new List<tblBatchInstr>();

            DateTime entryDate;
            var entryDates = GetEntryDates(out entryDate);

            foreach(var newNote in instructionNotebook.Notes ?? new List<Note>())
            {
                var oldNote = removeOldInstructions.FirstOrDefault(i => i.Action == newNote.Text && i.Step == newNote.Sequence);
                if(oldNote != null)
                {
                    removeOldInstructions.Remove(oldNote);
                }
                else
                {
                    while(entryDates.Contains(entryDate))
                    {
                        entryDate = entryDate.AddMilliseconds(10).RoundMillisecondsForSQL();
                    }
                    entryDates.Add(entryDate);

                    addNewInstructions.Add(new tblBatchInstr
                        {
                            EntryDate = entryDate,
                            Lot = lotNumber,
                            Step = newNote.Sequence,
                            Action = newNote.Text,
                            Serialized = SerializableBatchInstruction.Serialize(newNote)
                        });
                }
            }

            foreach(var instruction in removeOldInstructions)
            {
                _oldContext.tblBatchInstrs.DeleteObject(instruction);
            }

            foreach(var instruction in addNewInstructions)
            {
                _oldContext.tblBatchInstrs.AddObject(instruction);
            }
        }

        private HashSet<DateTime> GetEntryDates(out DateTime rangeStartOutput)
        {
            rangeStartOutput = DateTime.Now;
            var rangeStart = rangeStartOutput;
            return new HashSet<DateTime>(_oldContext.tblBatchInstrs
                .Select(i => i.EntryDate)
                .Where(d => d >= rangeStart));
        }
    }
}