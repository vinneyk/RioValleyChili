using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class SampleOrderKey : EntityKey<ISampleOrderKey>.With<int, int>, ISampleOrderKey, IKey<SampleOrder>
    {
        public int SampleOrderKey_Year { get { return Field0; } }
        public int SampleOrderKey_Sequence { get { return Field1; } }

        public SampleOrderKey() { }
        public SampleOrderKey(ISampleOrderKey sampleOrder) : base(sampleOrder.SampleOrderKey_Year, sampleOrder.SampleOrderKey_Sequence) { }

        protected override ISampleOrderKey ConstructKey(int field0, int field1)
        {
            return new SampleOrderKey
                {
                    Field0 = field0,
                    Field1 = field1
                };
        }

        protected override With<int, int> DeconstructKey(ISampleOrderKey key)
        {
            return new SampleOrderKey
                {
                    Field0 = key.SampleOrderKey_Year,
                    Field1 = key.SampleOrderKey_Sequence
                };
        }

        public Expression<Func<SampleOrder, bool>> FindByPredicate { get { return s => s.Year == Field0 && s.Sequence == Field1; } }
    }

    public static class ISampleOrderKeyExtensions
    {
        public static SampleOrderKey ToSampleOrderKey(this ISampleOrderKey k)
        {
            return new SampleOrderKey(k);
        }
    }
}