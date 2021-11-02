using System;
using System.Collections.Generic;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class NotebookMother : IMother<Notebook>
    {
        private readonly Action<string> _logSummaryEntry;

        public NotebookMother(Action<string> logSummaryEntry)
        {
            _logSummaryEntry = logSummaryEntry;
        }

        private enum EntityTypes
        {
            Notebook
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        public IEnumerable<Notebook> BirthAll(Action consoleCallback = null)
        {
            _loadCount.Reset();

            foreach(var notebook in StaticNotebooks.Notebooks)
            {
                _loadCount.AddRead(EntityTypes.Notebook);

                if(consoleCallback != null)
                {
                    consoleCallback();
                }

                _loadCount.AddLoaded(EntityTypes.Notebook);
                yield return notebook;
            }

            _loadCount.LogResults(_logSummaryEntry);
        }
    }
}