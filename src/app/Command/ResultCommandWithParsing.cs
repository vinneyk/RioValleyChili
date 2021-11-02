using System;
using Solutionhead.Services;

namespace Command
{
    public abstract class ResultCommandWithParsing<TParseIn, TParseOut> : IResultWithInputCommand<TParseIn>
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        public abstract IResult ExecuteImplementation(TParseOut input);

        protected ResultCommandWithParsing(IParser<TParseIn, TParseOut> parser)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }

            _parser = parser;
        }

        public IResult Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            return !parseResult.Success ? new FailureResult() : ExecuteImplementation(parseResult.ResultingObject);
        }
    }

    public abstract class ResultCommandWithParsing<TResult, TParseIn, TParseOut> : IResultWithInputCommand<TResult, TParseIn>
    {
        private readonly IParser<TParseIn, TParseOut> _parser;

        public abstract IResult<TResult> ExecuteImplementation(TParseOut input);

        protected ResultCommandWithParsing(IParser<TParseIn, TParseOut> parser)
        {
            if(parser == null) { throw new ArgumentNullException("parser"); }

            _parser = parser;
        }

        public IResult<TResult> Execute(TParseIn input)
        {
            var parseResult = _parser.Parse(input);
            return !parseResult.Success ? new FailureResult<TResult>() : ExecuteImplementation(parseResult.ResultingObject);
        }
    }
}