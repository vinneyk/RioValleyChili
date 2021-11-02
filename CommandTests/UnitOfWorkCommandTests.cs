using System;
using Command;
using Moq;
using NUnit.Framework;
using ObjectValidator;
using Solutionhead.Data;
using Solutionhead.Services;

namespace CommandTests
{
    public class UnitOfWorkCommand_ResultTests
    {
        internal UnitOfWorkCommand<object, IUnitOfWork> Command;

        internal Mock<UnitOfWorkCommand<object, IUnitOfWork>> MoqCommand;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            MoqUnitOfWork = new Mock<IUnitOfWork>();
            
            MoqCommand = new Mock<UnitOfWorkCommand<object, IUnitOfWork>>(MoqUnitOfWork.Object) { CallBase = true };
            Command = MoqCommand.Object;
        }

        [TestFixture]
        public sealed class Constructing : UnitOfWorkCommand_ResultTests
        {
            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<UnitOfWorkCommand<object, IUnitOfWork>>(null).Object;
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
        public sealed class Execute : UnitOfWorkCommand_ResultTests
        {
            [Test]
            public void Returns_failure_result_if_ExecuteImplementation_throws_exception()
            {
                //Arrange
                MoqCommand.Setup(m => m.ExecuteImplementation()).Throws(new Exception());

                //Act
                var result = Command.Execute();

                //Assert
                MoqCommand.Verify(m => m.ExecuteImplementation(), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
            {
                //Arrange
                var expectedResult = new SuccessResult<object>();
                MoqCommand.Setup(m => m.ExecuteImplementation()).Returns(expectedResult);

                //Act
                var result = Command.Execute();

                //Assert
                Assert.AreSame(expectedResult, result);
            }
        }
    }

    public class UnitOfWorkCommandTests
    {
        internal UnitOfWorkCommand<object, object, IUnitOfWork> Command;

        internal Mock<UnitOfWorkCommand<object, object, IUnitOfWork>> MoqCommand;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            MoqUnitOfWork = new Mock<IUnitOfWork>();

            MoqCommand = new Mock<UnitOfWorkCommand<object, object, IUnitOfWork>>(MoqUnitOfWork.Object) { CallBase = true };
            Command = MoqCommand.Object;
        }

        [TestFixture]
        public sealed class Constructing : UnitOfWorkCommandTests
        {
            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<UnitOfWorkCommand<object, object, IUnitOfWork>>(null).Object;
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
        public sealed class Execute : UnitOfWorkCommandTests
        {

            [Test]
            public void Returns_failure_result_if_ExecuteImplementation_throws_exception()
            {
                //Arrange
                MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Throws(new Exception());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqCommand.Verify(m => m.ExecuteImplementation(It.IsAny<object>()), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
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

    public class UnitOfWorkCommand_ResultInOutTests
    {
        internal UnitOfWorkCommand<object, object, object, IUnitOfWork> Command;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        internal Mock<IParser<object, object>> MoqParser;

        [SetUp]
        public void SetUp()
        {
            MoqUnitOfWork = new Mock<IUnitOfWork>();
            MoqParser = new Mock<IParser<object, object>>();
        }
            
        [TestFixture]
        public class Constructing : UnitOfWorkCommand_ResultInOutTests
        {
            [Test]
            public void Will_throw_exception_if_parser_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<UnitOfWorkCommand<object, object, object, IUnitOfWork>>(null, MoqParser.Object, null).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<UnitOfWorkCommand<object, object, object, IUnitOfWork>>(MoqParser.Object, null).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }
        }
    }

    public class CommitUnitOfWorkCommand_Input
    {
        internal CommitUnitOfWorkCommand<object, IUnitOfWork> Command;

        internal Mock<CommitUnitOfWorkCommand<object, IUnitOfWork>> MoqCommand;

        internal Mock<IValidator<object>> MoqValidator;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            MoqValidator = new Mock<IValidator<object>>();
            MoqUnitOfWork = new Mock<IUnitOfWork>();
            MoqCommand = new Mock<CommitUnitOfWorkCommand<object, IUnitOfWork>>(MoqValidator.Object, MoqUnitOfWork.Object) { CallBase = true };

            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult());
            MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(new SuccessResult());

            Command = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : CommitUnitOfWorkCommand_Input
        {
            [Test]
            public void Will_throw_exception_if_validator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkCommand<object, IUnitOfWork>>(null, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkCommand<object, IUnitOfWork>>(MoqValidator.Object, null).Object;
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
        public class Execute : CommitUnitOfWorkCommand_Input
        {
            [Test]
            public void Will_not_return_success_result_if_validating_is_not_successful()
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
            public void Returns_failure_result_if_calling_Commit_on_the_UnitOfWork_throws_exception()
            {
                //Arrange
                MoqUnitOfWork.Setup(m => m.Commit()).Throws(new Exception());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqUnitOfWork.Verify(m => m.Commit(), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
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

    public class CommitUnitOfWorkCommand_ResultInputTests
    {
        internal CommitUnitOfWorkCommand<object, object, IUnitOfWork> Command;

        internal Mock<CommitUnitOfWorkCommand<object, object, IUnitOfWork>> MoqCommand;

        internal Mock<IValidator<object>> MoqValidator;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            MoqValidator = new Mock<IValidator<object>>();
            MoqUnitOfWork = new Mock<IUnitOfWork>();
            MoqCommand = new Mock<CommitUnitOfWorkCommand<object, object, IUnitOfWork>>(MoqValidator.Object, MoqUnitOfWork.Object) { CallBase = true };

            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult());
            MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));

            Command = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : CommitUnitOfWorkCommand_ResultInputTests
        {
            [Test]
            public void Will_throw_exception_if_validator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkCommand<object, object, IUnitOfWork>>(null, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkCommand<object, object, IUnitOfWork>>(MoqValidator.Object, null).Object;
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
        public class Execute : CommitUnitOfWorkCommand_ResultInputTests
        {
            [Test]
            public void Will_not_return_success_result_if_validating_is_not_successful()
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
            public void Returns_failure_result_if_calling_Commit_on_the_UnitOfWork_throws_exception()
            {
                //Arrange
                MoqUnitOfWork.Setup(m => m.Commit()).Throws(new Exception());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqUnitOfWork.Verify(m => m.Commit(), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
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

    public class CommitUnitOfWorkWithParsingCommand_InputOutputTests
    {
        internal CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork> Command;

        internal Mock<CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork>> MoqCommand;

        internal Mock<IParser<object, object>> MoqParser;

        internal Mock<IValidator<object>> MoqValidator;

        internal Mock<IUnitOfWork> MoqUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            MoqParser = new Mock<IParser<object, object>>();
            MoqValidator = new Mock<IValidator<object>>();
            MoqUnitOfWork = new Mock<IUnitOfWork>();

            MoqCommand = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork>>(MoqParser.Object, MoqValidator.Object, MoqUnitOfWork.Object)
                { CallBase = true };
            Command = MoqCommand.Object;

            MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));
            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));
        }

        [TestFixture]
        public sealed class Constructing : CommitUnitOfWorkWithParsingCommand_InputOutputTests
        {
            [Test]
            public void Will_throw_exception_if_parser_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork>>(null, MoqValidator.Object, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_validator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork>>(MoqParser.Object, null, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, IUnitOfWork>>(MoqParser.Object, MoqValidator.Object, null).Object;
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
        public sealed class Execute : CommitUnitOfWorkWithParsingCommand_InputOutputTests
        {
            [Test]
            public void Will_not_return_success_result_if_parsing_result_is_not_successful()
            {
                //Arrange
                MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new InvalidResult<object>(null));

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqParser.Verify(m => m.Parse(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.Success);
            }

            [Test]
            public void Will_not_return_success_result_if_validation_is_no_successful()
            {
                //Arrange
                MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new InvalidResult<object>(null));

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqValidator.Verify(m => m.Validate(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.Success);
            }

            [Test]
            public void Returns_failure_result_if_calling_Commit_on_the_UnitOfWork_throws_exception()
            { 
                //Arrange
                MoqUnitOfWork.Setup(m => m.Commit()).Throws(new Exception());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqUnitOfWork.Verify(m => m.Commit(), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
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

    public class CommitUnitOfWorkWithParsingCommand_ResultInputOutputTests
    {
        internal CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork> Command;

        internal Mock<CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork>> MoqCommand;

        internal Mock<IParser<object, object>> MoqParser;

        internal Mock<IValidator<object>> MoqValidator;

        internal Mock<IUnitOfWork> MoqUnitOfWork;
        
        [SetUp]
        public void SetUp()
        {
            MoqParser = new Mock<IParser<object, object>>();
            MoqValidator = new Mock<IValidator<object>>();
            MoqUnitOfWork = new Mock<IUnitOfWork>();

            MoqCommand = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork>>(MoqParser.Object, MoqValidator.Object, MoqUnitOfWork.Object) { CallBase = true };
            Command = MoqCommand.Object;

            MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));
            MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));
        }

        [TestFixture]
        public sealed class Constructing : CommitUnitOfWorkWithParsingCommand_ResultInputOutputTests
        {
            [Test]
            public void Will_throw_exception_if_parser_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork>>(null, MoqValidator.Object, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_validator_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork>>(MoqParser.Object, null, MoqUnitOfWork.Object).Object;
                }
                catch
                {
                    caughtException = true;
                }

                //Assert
                Assert.IsTrue(caughtException);
            }

            [Test]
            public void Will_throw_exception_if_unitOfWork_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    Command = new Mock<CommitUnitOfWorkWithParsingCommand<object, object, object, IUnitOfWork>>(MoqParser.Object, MoqValidator.Object, null).Object;
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
        public sealed class Execute : CommitUnitOfWorkWithParsingCommand_ResultInputOutputTests
        {
            [Test]
            public void Will_not_return_success_result_if_parsing_result_is_not_successful()
            {
                //Arrange
                MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new InvalidResult<object>(null));

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqParser.Verify(m => m.Parse(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.Success);
            }

            [Test]
            public void Will_not_return_success_result_if_validation_is_no_successful()
            {
                //Arrange
                MoqValidator.Setup(m => m.Validate(It.IsAny<object>())).Returns(new InvalidResult<object>(null));

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqValidator.Verify(m => m.Validate(It.IsAny<object>()), Times.Once());
                Assert.AreNotEqual(ResultState.Success, result.Success);
            }

            [Test]
            public void Returns_failure_result_if_calling_Commit_on_the_UnitOfWork_throws_exception()
            {
                //Arrange
                MoqUnitOfWork.Setup(m => m.Commit()).Throws(new Exception());

                //Act
                var result = Command.Execute(null);

                //Assert
                MoqUnitOfWork.Verify(m => m.Commit(), Times.Once());
                Assert.AreEqual(ResultState.Failure, result.State);
            }

            [Test]
            public void Returns_ExecuteImplementation_result()
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