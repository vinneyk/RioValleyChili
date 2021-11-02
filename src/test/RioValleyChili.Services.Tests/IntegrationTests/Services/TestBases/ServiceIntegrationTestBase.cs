using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;
using Ninject;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.StaticRecords;
using RioValleyChili.Services.Configuration;
using RioValleyChili.Services.Tests.Helpers;
using Solutionhead.Core;
using TechTalk.SpecFlow;
using Company = RioValleyChili.Data.Models.Company;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases
{
    public abstract class ServiceIntegrationTestBase
    {
        public static RVCIntegrationTestHelper TestHelper { get; private set; }

        public EFRVCUnitOfWork RVCUnitOfWork { get { return _rvcUnitOfWork ?? (_rvcUnitOfWork = new EFRVCUnitOfWork(TestHelper.ResetContext())); } }

        public virtual Employee TestUser { get { return GetOrCreateTestUser(); } }

        public Facility RinconFacility
        {
            get
            {
                if(_rincon == null)
                {
                    _rincon = TestHelper.Context.Set<Facility>().FirstOrDefault(w => w.Id == GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id);
                    while(_rincon == null)
                    {
                        var warehouse = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Facility>(w => w.Locations = null);
                        TestHelper.SaveChangesToContext();
                        if(warehouse.Id > GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id)
                        {
                            throw new Exception("Could not create warehouse for Rincon with proper key!");
                        }

                        if(warehouse.Id == GlobalKeyHelpers.RinconFacilityKey.FacilityKey_Id)
                        {
                            _rincon = warehouse;
                        }
                    }
                }
                return _rincon;
            }
        }
        private Facility _rincon;

        protected ServiceIntegrationTestBase()
        {
            ServiceLocatorConfig.Configure(Kernel);
            Kernel.Bind<IExceptionLogger>().ToConstant(new Mock<IExceptionLogger>().Object);
        }

        [SetUp]
        [BeforeScenario]
        public void SetUp()
        {
            _rincon = null;
            _rvcUnitOfWork = null;

            if(TestHelper == null)
            {
                TestHelper = new RVCIntegrationTestHelper(TestHelperDropAndRecreateContext);
            }
            else if(TestHelperDropAndRecreateContext)
            {
                TestHelper.Reset();
            }

            OldContextSynchronizationSwitch.Enabled = OldContextSynchronizationEnabled;

            if(SetupStaticRecords)
            {
                LoadStaticRecords();
            }

            DerivedSetUp();
        }

        protected virtual bool OldContextSynchronizationEnabled { get { return false; } }
        protected virtual bool TestHelperDropAndRecreateContext { get { return true; } }
        protected virtual bool SetupStaticRecords { get { return true; } }

        protected readonly StandardKernel Kernel = new StandardKernel();

        protected virtual void DerivedSetUp() { }

        private static Stopwatch _stopwatch;

        protected static void StartStopwatch()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        protected static void StopWatchAndWriteTime(string label = null)
        {
            _stopwatch.Stop();
            Console.WriteLine("{0}: {1}", label ?? "Time Elapsed", _stopwatch.Elapsed);
        }

        protected static void TimedExecution(Action block, string label = null)
        {
            var stopwatch = Stopwatch.StartNew();
            block();
            stopwatch.Stop();
            Console.WriteLine("{0}: {1}", label ?? "Time Elapsed", stopwatch.Elapsed);
        }

        protected static TReturn TimedExecution<TReturn>(Func<TReturn> block, string label = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = block();
            stopwatch.Stop();
            Console.WriteLine("{0}: {1}", label ?? "Time Elapsed", stopwatch.Elapsed);
            return result;
        }

        protected ITimeStamper TimeStamper
        {
            get { return _timeStamper ?? (_timeStamper = Kernel.Get<ITimeStamper>()); }
        }
        private ITimeStamper _timeStamper;
        private EFRVCUnitOfWork _rvcUnitOfWork;

        private Employee GetOrCreateTestUser()
        {
            const string userName = "TestUser";
            var employees = TestHelper.Context.Employees;
            var testUser = employees.FirstOrDefault(e => e.UserName == userName);
            if(testUser == null)
            {
                var id = employees.Any() ? employees.Max(e => e.EmployeeId) + 1 : 1;
                testUser = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Employee>(e => e.EmployeeId = id, e => e.UserName = userName, e => e.DisplayName = userName);
            }

            if(OldContextSynchronizationEnabled)
            {
                try
                {
                    using(var oldContext = new RioAccessSQLEntities())
                    {
                        if(!oldContext.tblEmployees.Any(e => e.Employee == testUser.UserName))
                        {
                            oldContext.tblEmployees.AddObject(new tblEmployee
                                {
                                    EmployeeID = testUser.EmployeeId,
                                    Employee = testUser.UserName,
                                    EName = testUser.DisplayName
                                });
                            oldContext.SaveChanges();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Could not get or create test user in old context.");
                }
            }

            return testUser;
        }

        protected List<InventoryKey> GetExpectedInventoryKeys(IEnumerable<ILotKey> expectedLots, bool includeUnavailable, IFacilityKey facilityKey = null)
        {
            facilityKey = facilityKey ?? FacilityKey.Null;
            var useWarehouseKey = !facilityKey.Equals(FacilityKey.Null);

            return TestHelper.Context.Inventory
                .Where(i =>
                    (includeUnavailable || i.Quantity > 0) &&
                    (!useWarehouseKey || i.Location.FacilityId == facilityKey.FacilityKey_Id)).ToList()
                .Join(
                    expectedLots.ToList(),
                    i => new { i.LotKey_DateCreated, i.LotKey_DateSequence, i.LotKey_LotTypeId },
                    l => new { l.LotKey_DateCreated, l.LotKey_DateSequence, l.LotKey_LotTypeId },
                    (i, l) => new InventoryKey(i))
                .OrderBy(i => i.KeyValue).ToList();
        }

        protected void LoadStaticRecords()
        {
            TestHelper.CreateObjectGraph<InventoryTreatment>();
            TestHelper.Context.Set<InventoryTreatment>().Add(StaticInventoryTreatments.NoTreatment);

            TestHelper.CreateObjectGraph<Company>();
            var rvcBroker = StaticCompanies.RVCBroker;
            rvcBroker.EmployeeId = TestUser.EmployeeId;
            rvcBroker.Employee = null;
            rvcBroker.TimeStamp = DateTime.UtcNow;
            TestHelper.Context.Set<Company>().Add(rvcBroker);

            StaticAttributeNames.AttributeNames.ForEach(n =>
                {
                    TestHelper.CreateObjectGraph<AttributeName>();
                    TestHelper.Context.Set<AttributeName>().Add(n);
                });

            StaticPackagingProducts.PackagingProducts.ForEach(p =>
                {
                    TestHelper.CreateObjectGraph<PackagingProduct>();
                    TestHelper.Context.Set<PackagingProduct>().Add(p);
                });

            StaticFacilities.Warehouses.ForEach(w =>
                {
                    Facility facility;
                    do
                    {
                        w.Locations = null;
                        TestHelper.CreateObjectGraph<Facility>();
                        facility = TestHelper.Context.Set<Facility>().Add(w);
                    } while(facility.Id < w.Id);
                });

            if(TestHelper.Context.AdditiveTypes.Local.Any())
            {
                Debugger.Break();
            }

            try
            {
                TestHelper.Context.SaveChanges();
            }
            catch(Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        protected void ResetUnitOfWork()
        {
            _rvcUnitOfWork = null;
        }
    }

    public class ServiceIntegrationTestBase<TService> : ServiceIntegrationTestBase
        where TService : class
    {
        public TService Service { get { return _service; } }
        private TService _service;

        protected override void DerivedSetUp()
        {
            base.DerivedSetUp();
            _service = Kernel.Get<TService>();
            Assert.IsNotNull(_service, "Could not construct the service under test from the service locator.");
        }
    }
}