using System;
using System.Linq.Expressions;
using LinqKit;

namespace RioValleyChili.Business.Core.Helpers
{
    public static class AstaCalculator
    {
        public static int CalculateAsta(double testedValue, DateTime testDate, DateTime productionEndDate, DateTime currentDate)
        {
            return CalculateAstaDelegate(testedValue, testDate, productionEndDate, currentDate);
        }

        public static double CalculateRatio(DateTime startDate, DateTime currentDate)
        {
            return (double) CalculateRatioDelegate(startDate, currentDate);
        }

        public static Expression<Func<double, DateTime, DateTime, DateTime, int>> CalculateAsta()
        {
            var calcRatio = CalculateRatio();

            Expression<Func<double, DateTime, DateTime, DateTime, int>> calcAsta = (testedValue, testDate, productionEndDate, currentDate) => (int)
                (
                    (
                        ( testedValue / calcRatio.Invoke(productionEndDate, testDate) ) * calcRatio.Invoke(productionEndDate, currentDate)
                    )
                    + 0.5
                );
            
            return calcAsta.ExpandAll();
        }

        #region Private Parts

        private const double Decay = 0.9995;

        private static Func<DateTime, DateTime, double?> CalculateRatioDelegate
        {
            get { return _calculateRatioDelegate ?? (_calculateRatioDelegate = CalculateRatio().Compile()); }
        }

        private static Func<DateTime, DateTime, double?> _calculateRatioDelegate;

        private static Func<double, DateTime, DateTime, DateTime, int> CalculateAstaDelegate
        {
            get { return _calculateAstaDelegate ?? (_calculateAstaDelegate = CalculateAsta().Compile()); }
        }

        private static Func<double, DateTime, DateTime, DateTime, int> _calculateAstaDelegate;

        private static Expression<Func<DateTime, DateTime, double?>> CalculateRatio()
        {
            return (dateStart, dateEnd) => DBFunctions.Power(Decay, DBFunctions.DiffDays(dateStart, dateEnd));
        }

        #endregion
    }
}