namespace RioValleyChili.Services.Interfaces.Parameters.EmployeeService
{
    public interface ICreateEmployeeParameters
    {
        string UserName { get; }
        string EmailAddress { get; }
        string DisplayName { get; }
    }
}