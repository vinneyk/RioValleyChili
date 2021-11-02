using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderItemKeyReturn : ISampleOrderItemKey
    {
        public int SampleOrderKey_Year { get; internal set; }
        public int SampleOrderKey_Sequence { get; internal set; }
        public int SampleOrderItemKey_Sequence { get; internal set; }

        internal string SampleOrderItemKey { get { return new SampleOrderItemKey(this); } }
    }
}