using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Client.Reporting.Helpers;

namespace RioValleyChili.Client.Reporting.Models
{
    public class ProductionRecapReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public WeightItemGroup<WeightItem> BatchWeights { get; set; }
        public TestItemGroup BatchTests { get; set; }
        public TimeGroup BatchTimes { get; set; }

        public WeightItemGroup<WeightItem> LineWeights { get; set; }
        public TestItemGroup LineTests { get; set; }
        public TimeGroup LineTimes { get; set; }

        public WeightItemGroup<WeightItemGroup<WeightItemGroup<WeightItem>>> LineProductWeights { get; set; }
        public ProductDetailsSection ProductDetails { get; set; }
        public ProductDetailsSection LineDetails { get; set; }

        public void Initialize()
        {
            BatchWeights.Initialize();
            BatchTests.Initialize();
            BatchTimes.Initialize();

            LineWeights.Initialize();
            LineTests.Initialize();
            LineTimes.Initialize();

            LineProductWeights.Initialize();
            ProductDetails.Initialize();
            LineDetails.Initialize();
        }
    }

    public class WeightItem
    {
        public string Name { get; set; }
        public virtual int Target { get; set; }
        public virtual int Produced { get; set; }
        public int Delta { get { return Produced - Target; } }
        public float DeltaPercent { get { return ((float) Delta) / Target; } }

        public WeightItem Parent { get; set; }

        public virtual void Initialize() { }
    }

    public class WeightItemGroup<TItem> : WeightItem
        where TItem : WeightItem
    {
        public sealed override int Target { get { return Items.AddThemUp(b => b.Target) ?? 0; } set { } }
        public sealed override int Produced { get { return Items.AddThemUp(b => b.Produced) ?? 0; } set { } }

        public IEnumerable<TItem> Items { get; set; }

        public sealed override void Initialize()
        {
            Items.ForEach(i =>
                {
                    i.Parent = this;
                    i.Initialize();
                });
        }
    }

    public interface ITestItemBase
    {
        int Passed { get; }
        int Failed { get; }
        int NonCntrl { get; }
        int InProc { get; }
        float PassPercent { get; }
    }

    public abstract class TestItemBase
    {
        protected ITestItemBase This;

        public string PassedDisplay { get { return This.Passed.ToReportString("N0", false); } }
        public string FailedDisplay { get { return This.Failed.ToReportString("N0", false); } }
        public string NonCntrlDisplay { get { return This.NonCntrl.ToReportString("N0", false); } }
        public string InProcDisplay { get { return This.InProc.ToReportString("N0", false); } }
        public string PassPercentDisplay { get { return This.PassPercent.ToReportString("P2"); } }
    }

    public class TestItem : TestItemBase, ITestItemBase
    {
        public string Name { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int NonCntrl { get; set; }
        public int InProc { get; set; }
        public float PassPercent { get { return ((float) Passed) / (Passed + Failed + NonCntrl + InProc); } }
        public TestItemGroup Parent { get; set; }

        public TestItem()
        {
            This = this;
        }
    }

    public class TestItemGroup : TestItemBase, ITestItemBase
    {
        public int Passed { get { return Items.AddThemUp(i => i.Passed) ?? 0; } }
        public int Failed { get { return Items.AddThemUp(i => i.Failed) ?? 0; } }
        public int NonCntrl { get { return Items.AddThemUp(i => i.NonCntrl) ?? 0; } }
        public int InProc { get { return Items.AddThemUp(i => i.InProc) ?? 0; } }
        public float PassPercent { get { return ((float)Passed) / (Passed + Failed + NonCntrl + InProc); } }
        public IEnumerable<TestItem> Items { get; set; }

        public TestItemGroup()
        {
            This = this;
        }

        public void Initialize()
        {
            Items.ForEach(i => i.Parent = this);
        }
    }

    public interface ITimeItemBase
    {
        float BudgetHrs { get; }
        float Actual { get; }
    }

    public abstract class TimeItemBase
    {
        protected ITimeItemBase This;

        public string BudgetHrsDisplay { get { return This.BudgetHrs.ToReportString("N2", false); } }
        public string ActualDisplay { get { return This.Actual.ToReportString("N2", false); } }
        public string DeltaDisplay { get { return (This.Actual - This.BudgetHrs).ToReportString("N2", false); } }
        public string DeltaPercentDisplay { get { return ((This.Actual - This.BudgetHrs) / This.BudgetHrs).ToReportString("P2", false); } }
    }

    public class TimeItem : TimeItemBase, ITimeItemBase
    {
        public string Name { get; set; }
        public float BudgetHrs { get; set; }
        public float Actual { get; set; }
        public TimeGroup Parent { get; set; }

        public TimeItem()
        {
            This = this;
        }
    }

    public class TimeGroup : TimeItemBase, ITimeItemBase
    {
        public float BudgetHrs { get { return Items.AddThemUp(i => i.BudgetHrs) ?? 0.0f; } }
        public float Actual { get { return Items.AddThemUp(i => i.Actual) ?? 0.0f; } }
        public IEnumerable<TimeItem> Items { get; set; }

        public TimeGroup()
        {
            This = this;
        }

        public void Initialize()
        {
            Items.ForEach(i => i.Parent = this);
        }
    }

    public class ProductDetailLot : IProductDetail
    {
        public string Name { get; set; }
        public string Lot { get; set; }
        public string Line { get; set; }
        public string Shift { get; set; }
        public string PSNum { get; set; }
        public string BatchType { get; set; }
        public string Mode { get; set; }
        public string LotStat { get; set; }
        public int TargetWeight { get; set; }
        public int ProducedWeight { get; set; }
        public float BdgtTime { get; set; }
        public float ProductionTime { get; set; }

        public ProductDetailsGroup Parent { get; set; }
    }

    public interface IProductDetail
    {
        int TargetWeight { get; }
        int ProducedWeight { get; }
        float BdgtTime { get; }
        float ProductionTime { get; }
    }

    public abstract class ProductDetailsGroupBase<TItem> : IProductDetail
        where TItem : IProductDetail
    {
        public int TargetWeight { get { return Items.AddThemUp(b => b.TargetWeight) ?? 0; } }
        public int ProducedWeight { get { return Items.AddThemUp(b => b.ProducedWeight) ?? 0; } }
        public float BdgtTime { get { return Items.AddThemUp(b => b.BdgtTime) ?? 0.0f; } }
        public float ProductionTime { get { return Items.AddThemUp(b => b.ProductionTime) ?? 0.0f; } }
        public string BdgtPoundsPerHour { get { return (TargetWeight / BdgtTime).ToReportString("N2"); } }
        public string ProdPoundsPerHour { get { return (ProducedWeight / ProductionTime).ToReportString("N2"); } }

        public IEnumerable<TItem> Items { get; set; }

        public abstract void Initialize();
    }

    public class ProductDetailsGroup : ProductDetailsGroupBase<ProductDetailLot>
    {
        public string Name { get; set; }
        public ProductDetailsSection Parent { get; set; }

        public override void Initialize()
        {
            Items.ForEach(i => i.Parent = this);
        }
    }

    public class ProductDetailsSection : ProductDetailsGroupBase<ProductDetailsGroup>
    {
        public override void Initialize()
        {
            Items.ForEach(i =>
                {
                    i.Parent = this;
                    i.Initialize();
                });
        }
    }

    public static class IEnumerableExtensions
    {
        public static int? AddThemUp<T>(this IEnumerable<T> items, Func<T, int> select)
        {
            return items == null || !items.Any() ? (int?)null : items.Sum(select);
        }

        public static float? AddThemUp<T>(this IEnumerable<T> items, Func<T, float> select)
        {
            return items == null || !items.Any() ? (float?)null : items.Sum(select);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach(var item in items)
            {
                action(item);
            }
        }
    }
}