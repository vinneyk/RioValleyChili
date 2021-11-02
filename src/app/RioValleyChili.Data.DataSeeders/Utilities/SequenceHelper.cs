using System.Collections.Generic;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class SequenceHelper<TKey>
    {
        protected Dictionary<TKey, int> Sequences
        {
            get { return _sequences ?? (_sequences = new Dictionary<TKey, int>()); }
            set { _sequences = value; }
        }
        private Dictionary<TKey, int> _sequences;

        public int GetNextSequence(TKey key, int sequenceStart = 1)
        {
            int sequence;
            if(Sequences.TryGetValue(key, out sequence))
            {
                return Sequences[key] = ++sequence;
            }

            Sequences.Add(key, sequenceStart);
            return sequenceStart;
        }
    }
}