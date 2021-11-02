using System;
using Solutionhead.Services;

namespace Command
{
    public interface IResultCommand
    {
        IResult Execute();
    }

    public interface IResultCommand<out TResult>
    {
        IResult<TResult> Execute();
    }

    [Obsolete("Use IResultWithInputCommand instead.")]
    public interface IResultCommand<out TResult, in TInput>
    {
        IResult<TResult> Execute(TInput input);
    }

    public interface IResultWithInputCommand<in TInput>
    {
        IResult Execute(TInput input);
    }

    public interface IResultWithInputCommand<out TResult, in TInput>
    {
        IResult<TResult> Execute(TInput input);
    }
}