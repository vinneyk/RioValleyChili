using Command;
using Moq;
using NUnit.Framework;
using ObjectValidator;
using Solutionhead.Services;

namespace CommandTests
{
    public class ResultWithValidatedInputCommand_InputTests
    {
        internal ResultWithValidatedInputCommand<object> Command;

        internal Mock<ResultWithValidatedInputCommand<object>> MoqCommand;

        internal Mock<IValidator<object>> MoqValidator;

        [SetUp]
        public void SetUp()
        {
            MoqValidator = new Mock<IValidator<object>>();
            MoqCommand = new Mock<ResultWithValidatedInputCommand<object>>(MoqValidator.Object) { CallBase = true };

            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult());

            Command = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : ResultWithValidatedInputCommand_InputTests
        {
            [Test]
            public void Will_throw_exception_if_inputValidator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<ResultWithValidatedInputCommand<object>>(null).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }
        }

        [TestFixture]
        public class Execute : ResultWithValidatedInputCommand_InputTests
        {
            [Test]
            public void Will_not_return_successful_resul_if_validation_fails()
            {
                //Arrange
                MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new InvalidResult());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqValidator.Verify(m => m.Validate(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.State);
            }

            [Test]
            public void Returns_ExceuteImplementation_result()
            {
                //Arrange
                var expectedResult = new SuccessResult();
                MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(expectedResult);

                //Act
                var result = Command.Execute(null);

                //Assert
                Assert.AreSame(expectedResult, result);
            }
        }
    }

    public class ResultWithValidatedInputCommand_OutputInputTests
    {
        internal ResultWithValidatedInputCommand<object, object> Command;

        internal Mock<ResultWithValidatedInputCommand<object, object>> MoqCommand;

        internal Mock<IValidator<object>> MoqValidator;

        [SetUp]
        public void SetUp()
        {
            MoqValidator = new Mock<IValidator<object>>();
            MoqCommand = new Mock<ResultWithValidatedInputCommand<object, object>>(MoqValidator.Object) { CallBase = true };

            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult());

            Command = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : ResultWithValidatedInputCommand_OutputInputTests
        {
            [Test]
            public void Will_throw_exception_if_inputValidator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<ResultWithValidatedInputCommand<object, object>>(null).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }
        }

        [TestFixture]
        public class Execute : ResultWithValidatedInputCommand_OutputInputTests
        {
            [Test]
            public void Will_not_return_successful_resul_if_validation_fails()
            {
                //Arrange
                MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new InvalidResult());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqValidator.Verify(m => m.Validate(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.State);
            }

            [Test]
            public void Returns_ExceuteImplementation_result()
            {
                //Arrange
                var expectedResult = new SuccessResult<object>();
                MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(expectedResult);

                //Act
                var result = Command.Execute(null);

                //Assert
                Assert.AreSame(expectedResult, result);
            }
        }
    }
}