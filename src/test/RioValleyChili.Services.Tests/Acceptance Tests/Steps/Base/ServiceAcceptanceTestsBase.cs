using RioValleyChili.Data.Models;
using RioValleyChili.Services.Tests.Helpers;

namespace RioValleyChili.Services.Tests
{
    public abstract class ServiceAcceptanceTestsBase<TService>
        where TService : class
    {
        public SharedContext SharedContext { get; private set; }
        public TService Service { get { return _service ?? (_service = SharedContext.GetService<TService>()); } }
        private TService _service;
        public RVCIntegrationTestHelper TestHelper { get { return SharedContext.TestHelper; } }
        public Employee TestUser { get { return SharedContext.TestUser; } }
        public Facility RinconFacility { get { return SharedContext.RinconFacility; } }

        protected ServiceAcceptanceTestsBase(SharedContext sharedContext)
        {
            SharedContext = sharedContext;
        }
    }
}