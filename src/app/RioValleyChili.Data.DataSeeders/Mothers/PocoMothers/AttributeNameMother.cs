using System;
using System.Collections.Generic;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class AttributeNameMother : IMother<AttributeName>
    {
        private readonly Action<string> _logSummaryEntry;

        public AttributeNameMother(Action<string> logSummaryEntry)
        {
            _logSummaryEntry = logSummaryEntry;
        }

        private enum EntityTypes
        {
            AttributeName
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        public IEnumerable<AttributeName> BirthAll(Action consoleCallback = null)
        {
            _loadCount.Reset();

            foreach(var attributeName in StaticAttributeNames.AttributeNames)
            {
                _loadCount.AddRead(EntityTypes.AttributeName);
                _loadCount.AddLoaded(EntityTypes.AttributeName);
                yield return attributeName;
            }

            _loadCount.LogResults(_logSummaryEntry);
        }
    }
}