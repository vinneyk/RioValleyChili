using System;
using Moq;
using NUnit.Framework;
using PostSharp.Aspects;
using RioValleyChili.Core;
using RioValleyChili.Services.OldContextSynchronization.Interfaces;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Services;

namespace RioValleyChili.Services.OldContextSynchronization.Tests.Base
{
    public abstract class SynchronizeOldContextUnitTestsBase<TMethodReturn, TCommandInput>
        where TMethodReturn : class, IResult
    {
        protected abstract NewContextMethod NewContextMethod { get;  }

        protected Mock<IKillSwitch> MockKillSwitch { get; private set; }
        protected Mock<ISynchronizationCommandFactory> MockCommandFactory { get; private set; }
        protected Mock<ISynchronizationCommand<TCommandInput>> MockCommand { get; private set; }
        protected Mock<TMethodReturn> MockMethodReturn { get; private set; }

        protected MethodExecutionArgs MethodExecutionArgs { get; private set; }
        protected SynchronizeOldContext SynchronizeOldContext { get; private set; }

        [SetUp]
        public void SetUp()
        {
            KillSwitch.Instance = (MockKillSwitch = new Mock<IKillSwitch>()).Object;

            MockMethodReturn = new Mock<TMethodReturn>();
            MethodExecutionArgs = new MethodExecutionArgs(null, null)
                {
                    ReturnValue = MockMethodReturn.Object
                };
            
            MockCommand = new Mock<ISynchronizationCommand<TCommandInput>>();
            MockCommandFactory = new Mock<ISynchronizationCommandFactory>();
            MockCommandFactory.Setup(m => m.GetCommand<TCommandInput>(It.Is<NewContextMethod>(v => v == NewContextMethod), It.IsAny<MethodExecutionArgs>())).Returns(MockCommand.Object);
            SynchronizationCommandFactory.Factory = MockCommandFactory.Object;

            SynchronizeOldContext = new SynchronizeOldContext(NewContextMethod);
        }

        [Test]
        public void KillSwitch_will_not_have_been_engaged_if_service_method_was_not_successful()
        {
            //Arrange
            MockMethodReturn.Setup(m => m.Success).Returns(false);

            //Act
            SynchronizeOldContext.OnExit(MethodExecutionArgs);

            //Assert
            MockKillSwitch.Verify(k => k.Engage(), Times.Never());
        }

        [Test]
        [ExpectedException(typeof(SynchronizeOldContextException))]
        public void KillSwitch_will_have_been_engaged_if_service_method_was_successful_but_synchronization_was_not()
        {
            //Arrange
            MockMethodReturn.Setup(m => m.Success).Returns(true);
            MockCommand.Setup(m => m.Synchronize(It.IsAny<Func<TCommandInput>>())).Throws(new Exception());

            //Act
            SynchronizeOldContext.OnExit(MethodExecutionArgs);

            //Assert
            MockKillSwitch.Verify(k => k.Engage(), Times.Once());
        }
    }

    public abstract class SynchronizeOldContextUnitTestsBase<TMethodReturn> : SynchronizeOldContextUnitTestsBase<IResult<TMethodReturn>, TMethodReturn> { }
}