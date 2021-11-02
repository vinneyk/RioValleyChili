using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PostSharp.Aspects;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Services.OldContextSynchronization.Helpers;
using RioValleyChili.Services.OldContextSynchronization.Interfaces;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.OldContextSynchronization
{
    public class SynchronizationCommandFactory : ISynchronizationCommandFactory
    {
        #region Static

        public static SyncCommandInfo GetCommandInfo(NewContextMethod m)
        {
            SyncCommandInfo command;
            return Commands.TryGetValue(m, out command) ? command : null;
        }

        public static ISynchronizationCommandFactory Factory
        {
            get { return _factory ?? (_factory = new SynchronizationCommandFactory()); }
            set { _factory = value; }
        }

        private static ISynchronizationCommandFactory _factory;

        private static readonly IReadOnlyDictionary<NewContextMethod, SyncCommandInfo> Commands;

        static SynchronizationCommandFactory()
        {
            var syncBaseType = typeof(SyncCommandBase<,>);
            var results = new Dictionary<NewContextMethod, SyncCommandInfo>();

            foreach(var type in syncBaseType.Assembly.GetTypes())
            {
                Type[] arguments;
                if(type.Implements(syncBaseType, out arguments))
                {
                    var commandInfo = new SyncCommandInfo(type, arguments);
                    foreach(var method in type.GetCustomAttributes(true).OfType<SyncAttribute>().Select(a => a.NewContextMethod).Distinct().ToList())
                    {
                        results.Add(method, commandInfo);
                    }
                }
            }

            Commands = results;
        }

        // ReSharper disable UnusedMember.Local
        private static readonly MethodInfo GetUnitOfWorkBaseMethodInfo = typeof(SynchronizationCommandFactory).GetMethod("GetUnitOfWork", BindingFlags.Static | BindingFlags.NonPublic);
        private static TUnitOfWork GetUnitOfWork<TUnitOfWork>(MethodExecutionArgs args) where TUnitOfWork : IUnitOfWork
        {
            var unitOfWorkContainer = args.Instance as IUnitOfWorkContainer<TUnitOfWork>;
            if(unitOfWorkContainer == null)
            {
                throw new ArgumentException(string.Format("{0} must implement IUnitOfWorkContainer<{1}>", args.Instance.GetType().Name, typeof(TUnitOfWork).Name));
            }
            return unitOfWorkContainer.UnitOfWork;
        }

        // ReSharper restore UnusedMember.Local

        #endregion

        private SynchronizationCommandFactory() { }

        public ISynchronizationCommand<TInput> GetCommand<TInput>(NewContextMethod method, MethodExecutionArgs args)
        {
            SyncCommandInfo constructCommand;
            if(!Commands.TryGetValue(method, out constructCommand))
            {
                throw new Exception(string.Format("Could not find command to synchronize method[{0}]", method));
            }

            return (ISynchronizationCommand<TInput>)constructCommand.CreateCommand(args);
        }

        public class SyncCommandInfo
        {
            public readonly Type CommandType;
            public readonly Type UnitOfWorkType;
            public readonly Type ParameterType;
            public readonly MethodInfo GetUnitOfWorkMethodInfo;

            public SyncCommandInfo(Type commandType, Type[] genericArguments)
            {
                CommandType = commandType;
                UnitOfWorkType = genericArguments[0];
                ParameterType = genericArguments[1];
                GetUnitOfWorkMethodInfo = GetUnitOfWorkBaseMethodInfo.MakeGenericMethod(UnitOfWorkType);
            }

            public object CreateCommand(MethodExecutionArgs args)
            {
                return Activator.CreateInstance(CommandType, GetUnitOfWorkMethodInfo.Invoke(null, new object[] { args }));
            }
        }
    }
}