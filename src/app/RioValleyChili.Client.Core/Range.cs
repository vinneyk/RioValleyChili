namespace RioValleyChili.Client.Core
{
    public class Range<TValue>
    {
        private readonly TValue _minValue;
        private readonly TValue _maxValue;

        public Range(TValue min, TValue max)
        {
            _minValue = min;
            _maxValue = max;
        }

        public TValue MinValue
        {
            get { return _minValue; }
        }

        public TValue MaxValue
        {
            get { return _maxValue; }
        }
    }
}