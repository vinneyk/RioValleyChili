using System;
using System.Reflection;
using PostSharp.Aspects;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Services.OldContextSynchronization.Interfaces;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    [Serializable]
    [Issue("It looks like there's some sort of optimization in place where if a boundary aspect does not reference" +
           "MethodExecutionArgs properties in executable code PostSharp decides to not initialize at all." +
           "Since this aspect actually depends on a runtime-compiled method to execute it started breaking the" +
           "old context synch. I've modified the OnExit method to check that the necessary properties are not null" +
           "and that seems to have fixed the issue for now. -RI 2016-11-8",
           References = new[] { "RVCADMIN-1366"})]
    public class SynchronizeOldContext : OnMethodBoundaryAspect
    {
        private readonly NewContextMethod _newContextMethod;

        public SynchronizeOldContext(NewContextMethod newContextMethod)
        {
            _newContextMethod = newContextMethod;
        }

        public sealed override void OnExit(MethodExecutionArgs args)
        {
            if(!OldContextSynchronizationSwitch.Enabled)
            {
                Console.WriteLine("OldContextSynchronization not enabled.");
                return;
            }

            if(args.Exception != null)
            {
                Console.WriteLine("Method exited in error state.");
                return;
            }

            try
            {
                if(args.Instance == null)
                {
                    throw new Exception("SynchronizeOldContext requires method to belong to an object instance.");
                }

                if(args.ReturnValue == null)
                {
                    throw new Exception("SynchronizeOldContext requires method return value.");
                }

                ExecuteByConvention(_newContextMethod, args);
            }
            catch(Exception ex)
            {
                KillSwitch.Engage();
                var synchronizeException = new SynchronizeOldContextException(args, ex);
                Console.WriteLine(synchronizeException.Message);
                throw synchronizeException;
            }
        }

        private static void ExecuteByConvention(NewContextMethod method, MethodExecutionArgs args)
        {
            try
            {
                var commandInfo = SynchronizationCommandFactory.GetCommandInfo(method);
                var executeMethod = ExecuteCommandByConventionMethodInfo.MakeGenericMethod(commandInfo.ParameterType);
                executeMethod.Invoke(null, new object[] { commandInfo, args });
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Could not execute '{0}' by convention.", method), ex);
            }
        }

        // ReSharper disable UnusedMember.Local
        private static readonly MethodInfo ExecuteCommandByConventionMethodInfo = typeof(SynchronizeOldContext).GetMethod("ExecuteCommandByConvention", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        private static void ExecuteCommandByConvention<TParameters>(SynchronizationCommandFactory.SyncCommandInfo info, MethodExecutionArgs args)
        {
            var result = (IResult)args.ReturnValue;
            if(result.Success)
            {
                var parameters = (SyncParameters<TParameters>)args.ReturnValue;
                var command = (ISynchronizationCommand<TParameters>)info.CreateCommand(args);
                command.Synchronize(() => parameters.Parameters);
            }
        }
        // ReSharper restore UnusedMember.Local
    }
}