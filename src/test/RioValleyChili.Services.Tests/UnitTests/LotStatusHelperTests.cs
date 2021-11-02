using System;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;

namespace RioValleyChili.Services.Tests.UnitTests
{
    [TestFixture]
    public class LotStatusHelperTests
    {
        [TestFixture]
        public class PreserveQualityStatusTests : LotStatusHelperTests
        {
            [TestFixture]
            public class Current_Undetermined : CurrentStatusTestsBase
            {
                protected override LotQualityStatus CurrentStatus { get { return LotQualityStatus.Pending; } }

                [Test]
                public void Not_Rejected_results_in_determined_status()
                {
                    foreach(var status in Enum.GetValues(typeof(LotQualityStatus)).Cast<LotQualityStatus>())
                    {
                        if(status != LotQualityStatus.Rejected)
                        {
                            AssertResult(status, status);
                        }
                    }
                }
            }

            [TestFixture]
            public class Current_Accepted : CurrentStatusTestsBase
            {
                protected override LotQualityStatus CurrentStatus { get { return LotQualityStatus.Released; } }

                [Test]
                public void Not_Rejected_results_in_determined_status()
                {
                    foreach(var status in Enum.GetValues(typeof(LotQualityStatus)).Cast<LotQualityStatus>())
                    {
                        if(status != LotQualityStatus.Rejected)
                        {
                            AssertResult(status, status);
                        }
                    }
                }
            }

            [TestFixture]
            public class Current_Contaminated : CurrentStatusTestsBase
            {
                protected override LotQualityStatus CurrentStatus { get { return LotQualityStatus.Contaminated; } }

                [Test]
                public void Not_Rejected_results_in_determined_status()
                {
                    foreach(var status in Enum.GetValues(typeof(LotQualityStatus)).Cast<LotQualityStatus>())
                    {
                        if(status != LotQualityStatus.Rejected)
                        {
                            AssertResult(status, status);
                        }
                    }
                }
            }

            [TestFixture]
            public class Current_Rejected : CurrentStatusTestsBase
            {
                protected override LotQualityStatus CurrentStatus { get { return LotQualityStatus.Rejected; } }

                [Test]
                public void Always_results_in_Rejected()
                {
                    foreach(var status in Enum.GetValues(typeof(LotQualityStatus)).Cast<LotQualityStatus>())
                    {
                        AssertResult(LotQualityStatus.Rejected, status);
                    }
                }
            }

            public abstract class CurrentStatusTestsBase : PreserveQualityStatusTests
            {
                protected abstract LotQualityStatus CurrentStatus { get; }

                protected void AssertResult(LotQualityStatus expectedStatus, LotQualityStatus determinedStatus)
                {
                    Assert.AreEqual(expectedStatus, LotStatusHelper.PreserveQualityStatus(CurrentStatus, determinedStatus));
                }
            }
        }
    }
}