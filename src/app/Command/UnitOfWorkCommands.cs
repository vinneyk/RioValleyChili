using System;
using ObjectValidator;
using Solutionhead.Data;
using Solutionhead.Services;

namespace Command
{
    public abstract class UnitOfWorkCommand<TResult, TUnitOfWork> :
        IResultCommand<TResult> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        
        public abstract IResult<TResult> ExecuteImplementation();

        protected UnitOfWorkCommand(TUnitOfWork unitOfWork)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            UnitOfWork = unitOfWork;
        }

        public IResult<TResult> Execute()
        {
            try
            {
                return ExecuteImplementation();
            }
            catch(Exception exception)
            {
                //todo: create ExceptionResult and replace FailureResult with ExceptionResult to enable logging
                return new FailureResult<TResult>(default(TResult), exception.Message); //"An error occurred while attempting to execute the request.");
            }
        }
    }

    public abstract class UnitOfWorkCommand<TResult, TInput, TUnitOfWork> :
        IResultWithInputCommand<TResult, TInput> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;

        public abstract IResult<TResult> ExecuteImplementation(TInput input);

        protected UnitOfWorkCommand(TUnitOfWork unitOfWork)
        {
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            UnitOfWork = unitOfWork;
        }

        public IResult<TResult> Execute(TInput input)
        {
            try
            {
                return ExecuteImplementation(input);
            }
            catch (Exception ex)
            {
                //todo replace FailureResult with ExceptionResult to enable logging
                return new FailureResult<TResult>(default(TResult), ex.Message);
            }
        }
    }

    public abstract class UnitOfWorkCommand<TResult, TParseIn, TParseOut, TUnitOfWork> :
        ResultCommandWithParsing<TResult, TParseIn, TParseOut> where TUnitOfWork : class, IUnitOfWork
    {
        protected readonly TUnitOfWork UnitOfWork;
        
        protected UnitOfWorkCommand(IParser<TParseIn, TParseOut> parser, TUnitOfWork unitOfWork)
            : base(parser)
        {
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            UnitOfWork = unitOfWork; 
        }
    }

    public abstract class CommitUnitOfWorkCommand<TInput, TUnitOfWork> :
        IResultWithInputCommand<TInput> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IValidator<TInput> _validator;
        protected readonly TUnitOfWork UnitOfWork;

        public abstract IResult ExecuteImplementation(TInput input);

        protected CommitUnitOfWorkCommand(IValidator<TInput> validator,TUnitOfWork unitOfWork)
        {
            if(validator == null) { throw new ArgumentNullException("validator"); }
            if(unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            _validator = validator;
            UnitOfWork = unitOfWork;
        }

        public IResult Execute(TInput input)
        {
            var validateResult = _validator.Validate(input);
            if(!validateResult.Success)
            {
                return new InvalidResult("The object received could not be validated. As a result, the command could not be executed.");
            }

            var executeResult = ExecuteImplementation(input);

            try
            {
                UnitOfWork.Commit();
            }
            catch(Exception exception)
            {
                //todo: create ExceptionResult and replace FailureResult with ExceptionResult to enable logging
                return new FailureResult(exception.Message);
            }

            return executeResult;
        }
    }

    public abstract class CommitUnitOfWorkCommand<TResult, TInput, TUnitOfWork> :
        IResultWithInputCommand<TResult, TInput> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IValidator<TInput> _validator;
        protected readonly TUnitOfWork UnitOfWork;

        public abstract IResult<TResult> ExecuteImplementation(TInput input);

        protected CommitUnitOfWorkCommand(IValidator<TInput> validator,TUnitOfWork unitOfWork)
        {
            if (validator == null) { throw new ArgumentNullException("validator"); }
            if (unitOfWork == null) { throw new ArgumentNullException("unitOfWork"); }

            _validator = validator;
            UnitOfWork = unitOfWork;
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
                //todo: create ExceptionResult and replace FailureResult with ExceptionResult to enable logging
                return new FailureResult<TResult>(default(TResult), exception.Message);
            }

            return executeResult;
        }
    }

    public abstract class CommitUnitOfWorkWithParsingCommand<TParseIn, TParseOut, TUnitOfWork> :
        CommitUnitOfWorkCommand<TParseOut, TUnitOfWork> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        protected CommitUnitOfWorkWithParsingCommand(IParser<TParseIn, TParseOut> parser, IValidator<TParseOut> validator, TUnitOfWork unitOfWork)
            : base(validator, unitOfWork)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }
            _parser = parser;
        }

        public IResult Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            if(!parseResult.Success)
            {
                return new InvalidResult(string.Format("Invalid object received. The system could not parse an object of type '{0}' to a '{1}'.", typeof(TParseIn).Name, typeof(TParseOut).Name));
            }

            return base.Execute(parseResult.ResultingObject);
        }
    }

    public abstract class CommitUnitOfWorkWithParsingCommand<TResult, TParseIn, TParseOut, TUnitOfWork> :
        CommitUnitOfWorkCommand<TResult, TParseOut, TUnitOfWork> where TUnitOfWork : class, IUnitOfWork
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        protected CommitUnitOfWorkWithParsingCommand(IParser<TParseIn, TParseOut> parser, IValidator<TParseOut> validator, TUnitOfWork unitOfWork)
            : base(validator, unitOfWork)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }
            _parser = parser;
        }

        public IResult<TResult> Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            if(!parseResult.Success)
            {
                return new InvalidResult<TResult>(default(TResult), string.Format("Invalid object received. The system could not parse an object of type '{0}' to a '{1}'.", typeof(TParseIn).Name, typeof(TParseOut).Name));
            }

            return base.Execute(parseResult.ResultingObject);
        }
    }
}