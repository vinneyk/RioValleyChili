using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class SampleOrderJournalEntryKey : EntityKey<ISampleOrderJournalEntryKey>.With<int, int, int>, ISampleOrderJournalEntryKey, IKey<SampleOrderJournalEntry>
    {
        public int SampleOrderKey_Year { get { return Field0; } }
        public int SampleOrderKey_Sequence { get { return Field1; } }
        public int SampleOrderJournalEntryKey_Sequence { get { return Field2; } }

        public SampleOrderJournalEntryKey() { }
        public SampleOrderJournalEntryKey(ISampleOrderJournalEntryKey sampleOrderJournalEntry) : base(sampleOrderJournalEntry.SampleOrderKey_Year, sampleOrderJournalEntry.SampleOrderKey_Sequence, sampleOrderJournalEntry.SampleOrderJournalEntryKey_Sequence) { }

        protected override ISampleOrderJournalEntryKey ConstructKey(int field0, int field1, int field2)
        {
            return new SampleOrderJournalEntryKey
                {
                    Field0 = field0,
                    Field1 = field1,
                    Field2 = field2
                };
        }

        protected override With<int, int, int> DeconstructKey(ISampleOrderJournalEntryKey key)
        {
            return new SampleOrderJournalEntryKey
                {
                    Field0 = key.SampleOrderKey_Year,
                    Field1 = key.SampleOrderKey_Sequence,
                    Field2 = key.SampleOrderJournalEntryKey_Sequence
                };
        }

        public Expression<Func<SampleOrderJournalEntry, bool>> FindByPredicate { get { return j => j.SampleOrderYear == Field0 && j.SampleOrderSequence == Field1 && j.EntrySequence == Field2; } }
    }

    public static class ISampleOrderJournalEntryKeyExtensions
    {
        public static SampleOrderJournalEntryKey ToSampleOrderJournalEntryKey(this ISampleOrderJournalEntryKey k)
        {
            return new SampleOrderJournalEntryKey(k);
        }
    }
}