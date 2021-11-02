using Command;
using Moq;
using NUnit.Framework;
using Solutionhead.Services;

namespace CommandTests
{
    public class ResultCommandWithParsing_InputOutputTests
    {
        internal ResultCommandWithParsing<object, object> CommandWithParsing;

        internal Mock<ResultCommandWithParsing<object, object>> MoqCommand;

        internal Mock<IParser<object, object>> MoqParser;

        [SetUp]
        public void SetUp()
        {
            MoqParser = new Mock<IParser<object, object>>();
            MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));

            MoqCommand = new Mock<ResultCommandWithParsing<object, object>>(MoqParser.Object) { CallBase = true };

            CommandWithParsing = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : ResultCommandWithParsing_InputOutputTests
        {
            [Test]
            public void Will_throw_exception_if_parser_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    CommandWithParsing = new Mock<ResultCommandWithParsing<object, object>>(null).Object;
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
        public class Execute : ResultCommandWithParsing_ResultInputOutputTests
        {
            [Test]
            public void Does_not_return_success_result_if_parsing_is_not_sucessful()
            {
                //Arrange
                MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new FailureResult<object>());

                //Act
                var result = CommandWithParsing.Execute(null);

                //Assert
                Assert.AreNotEqual(ResultState.Success, result.State);
            }

            [Test]
            public void Returns_result_of_ExecuteImplementation()
            {
                //Arrange
                var expectedResult = new SuccessResult<object>(new object());
                MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(expectedResult);

                //Act
                var result = CommandWithParsing.Execute(null);

                //Assert
                Assert.AreSame(expectedResult, result);
            }
        }
    }

    public class ResultCommandWithParsing_ResultInputOutputTests
    {
        internal ResultCommandWithParsing<object, object, object> CommandWithParsing;

        internal Mock<ResultCommandWithParsing<object, object, object>> MoqCommand;

        internal Mock<IParser<object, object>> MoqParser;

        [SetUp]
        public void SetUp()
        {
            MoqParser = new Mock<IParser<object, object>>();
            MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new SuccessResult<object>(new object()));

            MoqCommand = new Mock<ResultCommandWithParsing<object, object, object>>(MoqParser.Object) { CallBase = true };

            CommandWithParsing = MoqCommand.Object;
        }

        [TestFixture]
        public class Constructing : ResultCommandWithParsing_ResultInputOutputTests
        {
            [Test]
            public void Will_throw_exception_if_parser_is_null()
            {
                //Arrange
                var caughtException = false;

                //Act
                try
                {
                    CommandWithParsing = new Mock<ResultCommandWithParsing<object, object, object>>(null).Object;
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
        public class Execute : ResultCommandWithParsing_ResultInputOutputTests
        {
            [Test]
            public void Does_not_return_success_result_if_parsing_is_not_sucessful()
            {
                //Arrange
                MoqParser.Setup(m => m.Parse(It.IsAny<object>())).Returns(new FailureResult<object>());

                //Act
                var result = CommandWithParsing.Execute(null);

                //Assert
                Assert.AreNotEqual(ResultState.Success, result.State);
            }

            [Test]
            public void Returns_result_of_ExecuteImplementation()
            {
                //Arrange
                var expectedResult = new SuccessResult<object>(new object()); 
                MoqCommand.Setup(m => m.ExecuteImplementation(It.IsAny<object>())).Returns(expectedResult);

                //Act
                var result = CommandWithParsing.Execute(null);

                //Assert
                Assert.AreSame(expectedResult, result);
            }
        }
    }
}
