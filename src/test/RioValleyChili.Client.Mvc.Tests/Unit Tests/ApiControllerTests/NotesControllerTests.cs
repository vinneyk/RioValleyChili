using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.NotebookService;
using RioValleyChili.Services.Interfaces.Returns.NotebookService;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class NotesControllerTests
    {
        private Mock<INotebookService> mockNotebookService;
        private Mock<IUserIdentityProvider> mockUserIdentityProvider;
        private NotesController systemUnderTest;
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();

        [SetUp]
        public void SetUp()
        {
            mockNotebookService = new Mock<INotebookService>();
            mockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            mockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()));
            systemUnderTest = new NotesController(mockNotebookService.Object, mockUserIdentityProvider.Object);
        }

        [Test, ExpectedException(typeof(ArgumentNullException), UserMessage = "Expected ArgumentNullException with null service parameter value.")]
        public void ThrowsIfNotebookServiceParameterIsNull()
        {
            new NotesController(null, mockUserIdentityProvider.Object);
        }
        
        [Test, ExpectedException(typeof(ArgumentNullException), UserMessage = "Expected ArgumentNullException with null service parameter value.")]
        public void ThrowsIfUserIdentityProviderParameterIsNull()
        {
            new NotesController(mockNotebookService.Object, null);
        }

        #region GET method tests

        [Test, ExpectedException(typeof (HttpResponseException))]
        public void Get_ThrowsHttpResponseException_IfServiceReturnsInvalid()
        {
            // Arrange
            mockNotebookService.Setup(m => m.GetNotebook(It.IsAny<string>()))
                               .Returns(new InvalidResult<INotebookReturn>(null));

            // Act
            systemUnderTest.Get("blah");
        }

        [Test, ExpectedException(typeof (HttpResponseException))]
        public void Get_ThrowsHttpResponseException_IfServiceReturnsError()
        {
            // Arrange
            mockNotebookService.Setup(m => m.GetNotebook(It.IsAny<string>()))
                               .Returns(new FailureResult<INotebookReturn>(null, "Test"));

            // Act
            systemUnderTest.Get("blah");
        }

        [Test]
        public void Get_ReturnsNotesAsExpectedOnSuccess()
        {
            // Arrange
            var fixture = _fixture;
            var expectedResults = fixture.Create<INotebookReturn>();
            const string key = "12345";
            mockNotebookService.Setup(m => m.GetNotebook(key))
                               .Returns(new SuccessResult<INotebookReturn>(expectedResults));

            // Act
            var actualResults = systemUnderTest.Get(key);

            // Assert
            Assert.AreEqual(expectedResults.Notes, actualResults);
        }

        #endregion

        #region GET(id) method tests

        [Test]
        public void GetByKey_ReturnsNoteAsExpected_OnSuccess()
        {
            // Arrange
            const string notebookKey = "key";
            var expectedResult = _fixture.Create<INotebookReturn>();
            var expectedNote = expectedResult.Notes.Last();
            mockNotebookService.Setup(m => m.GetNotebook(notebookKey))
                               .Returns(new SuccessResult<INotebookReturn>(expectedResult));

            // Act
            var @return = systemUnderTest.Get(notebookKey, expectedNote.NoteKey);

            // Assert
            Assert.AreEqual(expectedNote, @return);
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetByKey_ThrowsHttpResponseException_IfNotebookKeyIsInvalid()
        {
            // Arrange
            mockNotebookService.Setup(m => m.GetNotebook(It.IsAny<string>()))
                               .Returns(new InvalidResult<INotebookReturn>());

            // Act
            systemUnderTest.Get("bad monkey");
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void GetByKey_ThrowsNotFoundHttpResponseException_WhenNotebookIsFoundButNoteKeyIsInvalid()
        {
            // Arrange
            var expectedResults = _fixture.Create<INotebookReturn>();
            mockNotebookService.Setup(m => m.GetNotebook(It.IsAny<string>()))
                               .Returns(new SuccessResult<INotebookReturn>(expectedResults));

            // Act
            try
            {
                systemUnderTest.Get("goodNotebookKey.", "badNoteKey!!");
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                throw;
            }

            Assert.Fail("Expected HttpResponseException but did not catch.");
        }

        #endregion

        #region POST method tests

        [Test]
        public void Post_ReturnsHttpResponseMessageAsExpected_UponSuccess()
        {
            // Arrange
            const string notebookKey = "key";
            var noteParam = _fixture.Create<NoteDto>();
            var expectedReturnObject = new Note
                                           {
                                               //CreatedByUser = noteParam.UserToken,
                                               NoteDate = DateTime.UtcNow,
                                               NoteKey = "1",
                                               Text = noteParam.Text
                                           };
            mockNotebookService.Setup(m => m.AddNote(notebookKey, It.IsAny<ICreateNoteParameters>()))
                               .Returns(new SuccessResult<INoteReturn>(expectedReturnObject));

            // Act
            var httpResponseMessage = systemUnderTest.Post(notebookKey, noteParam);

            // Assert
            Assert.IsTrue(httpResponseMessage.StatusCode == HttpStatusCode.Created);
            var content = httpResponseMessage.Content as ObjectContent<INoteReturn>;
            Assert.IsNotNull(content);
            Assert.AreEqual(expectedReturnObject, content.Value);
        }

        [Test]
        public void Post_UtilizedUserIdentityProvider()
        {
            // arrange
            var note = _fixture.Create<NoteDto>();
            mockNotebookService.Setup(m => m.AddNote(It.IsAny<string>(), note))
                .Returns(new SuccessResult<INoteReturn>());

            // act
            systemUnderTest.Post("123", note);

            // assert
            mockUserIdentityProvider.Verify(m => m.SetUserIdentity(note), Times.Once());
        }

        [Test]
        public void Post_ReturnedObjectContainsResultingObjectInContent_UponSuccess()
        {
            // arrange
            var note = _fixture.Create<NoteDto>();
            var expectedNote = _fixture.Create<INoteReturn>();
            mockNotebookService.Setup(m => m.AddNote(It.IsAny<string>(), note))
                .Returns(new SuccessResult<INoteReturn>(expectedNote));

            // act
            var message = systemUnderTest.Post("123", note);
                        // assert
            var content = message.Content as ObjectContent<INoteReturn>;
            Assert.IsNotNull(content);
            Assert.AreEqual(expectedNote, content.Value);
        }

        [Test]
        public void Post_Returns400_WhenServiceResultsIsInvalid()
        {
            // Arrange
            var value = _fixture.Create<NoteDto>();
            mockNotebookService.Setup(m => m.AddNote(It.IsAny<string>(), It.IsAny<ICreateNoteParameters>()))
                               .Returns(new InvalidResult<INoteReturn>());

            // Act
            var response = systemUnderTest.Post("", value);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Post_Returns500_WhenServiceResultsIsFailure()
        {
            // Arrange
            var value = _fixture.Create<NoteDto>();
            mockNotebookService.Setup(m => m.AddNote(It.IsAny<string>(), It.IsAny<ICreateNoteParameters>()))
                               .Returns(new FailureResult<INoteReturn>());

            // Act
            var response = systemUnderTest.Post("", value);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public void Post_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            mockNotebookService.Setup(m => m.AddNote(It.IsAny<string>(), It.IsAny<ICreateNoteParameters>()))
                               .Returns(new SuccessResult<INoteReturn>());
            systemUnderTest.ModelState.AddModelError("TestError", "Test");
            var param = new NoteDto();

            // Act
            var message = systemUnderTest.Post("bad", param);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, message.StatusCode);
        }

        #endregion

        #region PUT method tests

        [Test]
        public void Put_Returns200_WhenServiceReturnsSuccess()
        {
            // Arrange
            const string noteKey = "1";
            var param = _fixture.Create<NoteDto>();
            mockNotebookService.Setup(m => m.UpdateNote(noteKey, It.IsAny<IUpdateNoteParameters>()))
                               .Returns(new SuccessResult());

            // Act
            var response = systemUnderTest.Put(noteKey, param);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Put_UtilizesIdentityProvider()
        {
            // Arrange
            var dto = _fixture.Create<NoteDto>();
            const string noteKey = "123";
            mockNotebookService.Setup(m => m.UpdateNote(noteKey, dto))
                .Returns(new SuccessResult());

            // Act
            systemUnderTest.Put(noteKey, dto);

            // Assert
            mockUserIdentityProvider.Verify(m => m.SetUserIdentity(dto), Times.Once());
        }

        [Test]
        public void Put_Returns400_WhenServiceReturnsInvalid()
        {
            // Arrange
            const string errorMessage = "ERROR MESSAGE FROM TEST";
            mockNotebookService.Setup(m => m.UpdateNote(It.IsAny<string>(), It.IsAny<IUpdateNoteParameters>()))
                               .Returns(new InvalidResult(errorMessage));
            var param = _fixture.Create<NoteDto>();

            // Act
            var response = systemUnderTest.Put("2134", param);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(errorMessage, response.ReasonPhrase);
        }

        [Test]
        public void Put_Returns500_WhenServiceReturnsFailure()
        {
            // Arrange
            const string errorMessage = "THIS IS MY ERROR!";
            mockNotebookService.Setup(m => m.UpdateNote(It.IsAny<string>(), It.IsAny<IUpdateNoteParameters>()))
                               .Returns(new FailureResult(errorMessage));

            // Act
            var response = systemUnderTest.Put("1", new NoteDto());

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(errorMessage, response.ReasonPhrase);
        }

        [Test, ExpectedException(typeof(HttpResponseException))]
        public void Put_Returns400_WhenModelStateIsInvalid()
        {
            // Arrange
            mockNotebookService.Setup(m => m.UpdateNote(It.IsAny<string>(), It.IsAny<IUpdateNoteParameters>()))
                               .Returns(new SuccessResult());
            systemUnderTest.ModelState.AddModelError("Test Error", "blah");

            // Act
            try
            {
                systemUnderTest.Put("1", new NoteDto());
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, ex.Response.StatusCode);
                throw;
            }
        }

        #endregion

        #region DELETE method tests

        [Test]
        public void Delete_Returns200_WhenServiceReturnsSuccess()
        {
            // Arrange
            mockNotebookService.Setup(m => m.DeleteNote(It.IsAny<string>()))
                               .Returns(new SuccessResult());

            // Act
            var response = systemUnderTest.Delete("1");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Delete_Returns404_WhenServiceReturnsInvalid()
        {
            // Arrange
            const string errorMessage = "THIS ERROR";
            mockNotebookService.Setup(m => m.DeleteNote(It.IsAny<string>()))
                               .Returns(new InvalidResult(errorMessage));

            // Act
            var response = systemUnderTest.Delete("xx");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(errorMessage, response.ReasonPhrase);
        }

        #endregion
    }
}