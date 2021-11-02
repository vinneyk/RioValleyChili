using Solutionhead.Services;

namespace ObjectValidator
{
    public interface IValidator<in TObject>
    {
        IResult Validate(TObject objectToValidate);
    }
}