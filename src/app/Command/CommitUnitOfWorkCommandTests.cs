using System;
using ObjectValidator;
using Solutionhead.Data;
using Solutionhead.Services;

namespace Command
{
    [Obsolete("Use variant of CommitUnitOfWorkCommand or CommitUnitOfWorkWithParsingCommand")]
    public abstract class ParseValidateCommitCommand<TOutput, TInput, TUnitOfWork> :
        IResultWithInputCommand<TOutput, TInput>
        where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IParser<TInput, TOutput> _parser;
        private readonly IValidator<TOutput> _validator;
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected internal abstract IResult<TOutput> ExecuteImplementation(TOutput output);

        protected ParseValidateCommitCommand(IParser<TInput, TOutput> parser, IValidator<TOutput> validator,
                                             TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            _parser = parser;
            _validator = validator;
            UnitOfWork = unitOfWork;
            _exceptionLogger = exceptionLogger;
        }

        public IResult<TOutput> Execute(TInput input)
        {
            var parseResult = _parser.Parse(input);
            if (!parseResult.Success)
            {
                return parseResult;
            }

            var validateResult = _validator.Validate(parseResult.ResultingObject);
            if (!validateResult.Success)
            {
                return new InvalidResult<TOutput>(default(TOutput),
                                                  "The object received could not be validated. As a result, the command could not be executed.");
            }

            var executeResult = ExecuteImplementation(parseResult.ResultingObject);

            try
            {
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                if (_exceptionLogger != null)
                {
                    _exceptionLogger.LogException(exception);
                }
                return new FailureResult<TOutput>();
            }

            return executeResult;
        }
    }

    
    public abstract class UnitOfWorkCommand<TResult, TUnitOfWork> :
        IResultCommand<TResult> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected abstract IResult<TResult> ExecuteImplementation();

        protected UnitOfWorkCommand(TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
        {
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            UnitOfWork = unitOfWork;
            _exceptionLogger = exceptionLogger;
        }

        public IResult<TResult> Execute()
        {
            try
            {
                return ExecuteImplementation();
            }
            catch (Exception exception)
            {
                if (_exceptionLogger != null)
                {
                    _exceptionLogger.LogException(exception);
                }
                return new FailureResult<TResult>(default(TResult), "An error occurred while attempting to execute the request.");
            }
        }
    }

    public abstract class UnitOfWorkCommand<TResult, TInput, TUnitOfWork> :
        IResultWithInputCommand<TResult, TInput> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected UnitOfWorkCommand(TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
        {
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }
            UnitOfWork = unitOfWork; 
            _exceptionLogger = exceptionLogger;
        }

        protected abstract IResult<TResult> ExecuteImplementation(TInput input);

        public IResult<TResult> Execute(TInput input)
        {
            try
            {
                return ExecuteImplementation(input);
            }
            catch (Exception exception)
            {
                if (_exceptionLogger != null)
                {
                    _exceptionLogger.LogException(exception);
                }
                return new FailureResult<TResult>();
            }
        }
    }


    public abstract class UnitOfWorkCommand<TResult, TParseIn, TParseOut, TUnitOfWork> :
        ResultCommandWithParsing<TResult, TParseIn, TParseOut> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected UnitOfWorkCommand(IParser<TParseIn, TParseOut> parser, TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
            : base(parser)
        {
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }
            UnitOfWork = unitOfWork; 
            _exceptionLogger = exceptionLogger;
        }
    }


    public abstract class CommitUnitOfWorkCommand<TInput, TUnitOfWork> :
        IResultWithInputCommand<TInput> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IValidator<TInput> _validator;
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected internal abstract IResult ExecuteImplementation(TInput input);

        protected CommitUnitOfWorkCommand(IValidator<TInput> validator,TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
        {
            if (validator == null) { throw new ArgumentNullException("validator"); }
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            _validator = validator;
            UnitOfWork = unitOfWork;
            _exceptionLogger = exceptionLogger;
        }

        public IResult Execute(TInput input)
        {
            var validateResult = _validator.Validate(input);
            if (!validateResult.Success)
            {
                return new InvalidResult("The object received could not be validated. As a result, the command could not be executed.");
            }

            var executeResult = ExecuteImplementation(input);

            try
            {
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                if (_exceptionLogger != null)
                {
                    _exceptionLogger.LogException(exception);
                }
                return new FailureResult();
            }

            return executeResult;
        }
    }

    public abstract class CommitUnitOfWorkCommand<TResult, TInput, TUnitOfWork> :
        IResultWithInputCommand<TResult, TInput> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IValidator<TInput> _validator;
        protected readonly TUnitOfWork UnitOfWork;
        private readonly IExceptionLogger _exceptionLogger;

        protected internal abstract IResult<TResult> ExecuteImplementation(TInput input);

        protected CommitUnitOfWorkCommand(IValidator<TInput> validator,TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
        {
            if (validator == null) { throw new ArgumentNullException("validator"); }
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            _validator = validator;
            UnitOfWork = unitOfWork;
            _exceptionLogger = exceptionLogger;
        }

        public IResult<TResult> Execute(TInput input)
        {
            var validateResult = _validator.Validate(input);
            if (!validateResult.Success)
            {
                return new InvalidResult<TResult>(default(TResult), "The object received could not be validated. As a result, the command could not be executed.");
            }

            var executeResult = ExecuteImplementation(input);

            try
            {
                UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                if (_exceptionLogger != null)
                {
                    _exceptionLogger.LogException(exception);
                }
                return new FailureResult<TResult>();
            }

            return executeResult;
        }
    }


    public abstract class CommitUnitOfWorkWithParsingCommand<TParseIn, TParseOut, TUnitOfWork> :
        CommitUnitOfWorkCommand<TParseOut, TUnitOfWork> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        protected CommitUnitOfWorkWithParsingCommand(IParser<TParseIn, TParseOut> parser, IValidator<TParseOut> validator, TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
            : base(validator, unitOfWork, exceptionLogger)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }
            _parser = parser;
        }

        public IResult Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            if (!parseResult.Success)
            {
                return new InvalidResult(string.Format("Invalid object recieved. The system could not parse an object of type '{0}' to a '{1}'.", typeof(TParseIn).Name, typeof(TParseOut).Name));
            }

            return base.Execute(parseResult.ResultingObject);
        }
    }

    public abstract class CommitUnitOfWorkWithParsingCommand<TResult, TParseIn, TParseOut, TUnitOfWork> :
        CommitUnitOfWorkCommand<TResult, TParseOut, TUnitOfWork> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        protected CommitUnitOfWorkWithParsingCommand(IParser<TParseIn, TParseOut> parser, IValidator<TParseOut> validator, TUnitOfWork unitOfWork, IExceptionLogger exceptionLogger)
            : base(validator, unitOfWork, exceptionLogger)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }
            _parser = parser;
        }

        public IResult<TResult> Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            if (!parseResult.Success)
            {
                return new InvalidResult<TResult>(default(TResult), string.Format("Invalid object recieved. The system could not parse an object of type '{0}' to a '{1}'.", typeof(TParseIn).Name, typeof(TParseOut).Name));
            }

            return base.Execute(parseResult.ResultingObject);
        }
    }


}