using RioValleyChili.Services.Interfaces.Returns.SampleOrderService;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class UserSummaryReturn : IUserSummaryReturn
    {
        public string EmployeeKey { get { return EmployeeKeyReturn.EmployeeKey; } }
        public string Name { get; internal set; }

        internal EmployeeKeyReturn EmployeeKeyReturn { get; set; }
    }
}