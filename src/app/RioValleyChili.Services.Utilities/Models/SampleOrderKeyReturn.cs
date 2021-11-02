using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class SampleOrderKeyReturn : ISampleOrderKey
    {
        public int SampleOrderKey_Year { get; internal set; }
        public int SampleOrderKey_Sequence { get; internal set; }

        internal string SampleOrderKey { get { return new SampleOrderKey(this); } }
    }
}