using RioValleyChili.Services.Interfaces.Parameters.Common;

namespace RioValleyChili.Services.Interfaces.Parameters.CompanyService
{
    public interface ISetCustomerNoteParameters : IUserIdentifiable
    {
        string Type { get; }
        string Text { get; }
        bool Bold { get; }
    }

    public interface ICreateCustomerNoteParameters : ISetCustomerNoteParameters
    {
        string CustomerKey { get; }
    }

    public interface IUpdateCustomerNoteParameters : ISetCustomerNoteParameters
    {
        string CustomerNoteKey { get; }
    }
}