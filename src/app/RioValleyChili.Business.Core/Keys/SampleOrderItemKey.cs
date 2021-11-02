using System;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class SampleOrderItemKey : EntityKey<ISampleOrderItemKey>.With<int, int, int>, ISampleOrderItemKey, IKey<SampleOrderItem>
    {
        public int SampleOrderKey_Year
        {
            get { return Field0; }
            set { Field0 = value; }
        }
        public int SampleOrderKey_Sequence {
            get { return Field1; }
            set { Field1 = value; }
        }
        public int SampleOrderItemKey_Sequence {
            get { return Field2; }
            set { Field2 = value; }
        }

        public SampleOrderItemKey() { }
        public SampleOrderItemKey(ISampleOrderItemKey sampleOrderItem) : base(sampleOrderItem.SampleOrderKey_Year, sampleOrderItem.SampleOrderKey_Sequence, sampleOrderItem.SampleOrderItemKey_Sequence) { }

        protected override ISampleOrderItemKey ConstructKey(int field0, int field1, int field2)
        {
            return new SampleOrderItemKey
                {
                    Field0 = field0,
                    Field1 = field1,
                    Field2 = field2
                };
        }

        protected override With<int, int, int> DeconstructKey(ISampleOrderItemKey key)
        {
            return new SampleOrderItemKey
                {
                    Field0 = key.SampleOrderKey_Year,
                    Field1 = key.SampleOrderKey_Sequence,
                    Field2 = key.SampleOrderItemKey_Sequence
                };
        }

        public Expression<Func<SampleOrderItem, bool>> FindByPredicate { get { return i => i.SampleOrderYear == Field0 && i.SampleOrderSequence == Field1 && i.ItemSequence == Field2; } }
    }

    public static class ISampleOrderItemKeyExtensions
    {
        public static SampleOrderItemKey ToSampleOrderItemKey(this ISampleOrderItemKey k)
        {
            return new SampleOrderItemKey(k);
        }
    }
}