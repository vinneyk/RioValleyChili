namespace RioValleyChili.Services.Interfaces.Returns.EmployeeService
{
    public interface IEmployeeSummaryReturn
    {
        string EmployeeKey { get; }

        string DisplayName { get; }

        string UserName { get; }

        string EmailAddress { get; }

        bool IsActive { get; }
    }
}