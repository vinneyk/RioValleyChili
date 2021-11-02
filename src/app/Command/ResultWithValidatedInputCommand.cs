using System;
using ObjectValidator;
using Solutionhead.Services;

namespace Command
{
    public abstract class ResultWithValidatedInputCommand<TInput> : IResultWithInputCommand<TInput>
    {
        private readonly IValidator<TInput> _inputValidator;

        public abstract IResult ExecuteImplementation(TInput input); 

        protected ResultWithValidatedInputCommand(IValidator<TInput> inputValidator)
        {
            if(inputValidator == null) { throw new ArgumentNullException("inputValidator"); }
            _inputValidator = inputValidator;
        }

        public IResult Execute(TInput input)
        {
            var validateResult = _inputValidator.Validate(input);
            if(!validateResult.Success)
            {
                return new InvalidResult(string.Format("The input validation failed with the following message: '{0}'", validateResult.Message));
            }

            return ExecuteImplementation(input);
        }
    }

    public abstract class ResultWithValidatedInputCommand<TOutput, TInput> : IResultWithInputCommand<TOutput, TInput>
    {
        private readonly IValidator<TInput> _inputValidator;

        public abstract IResult<TOutput> ExecuteImplementation(TInput input); 

        protected ResultWithValidatedInputCommand(IValidator<TInput> inputValidator)
        {
            if(inputValidator == null) { throw new ArgumentNullException("inputValidator"); }
            _inputValidator = inputValidator;
        }

        public IResult<TOutput> Execute(TInput input)
        {
            var validateResult = _inputValidator.Validate(input);
            if(!validateResult.Success)
            {
                return new InvalidResult<TOutput>(default(TOutput), string.Format("The input validation failed with the following message: '{0}'", validateResult.Message));
            }

            return ExecuteImplementation(input);
        }
    }
}