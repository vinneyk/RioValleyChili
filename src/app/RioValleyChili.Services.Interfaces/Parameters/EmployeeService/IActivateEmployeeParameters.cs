namespace RioValleyChili.Services.Interfaces.Parameters.EmployeeService
{
    public interface IActivateEmployeeParameters
    {
        string EmployeeKey { get; }

        string EmailAddress { get; }
    }
}