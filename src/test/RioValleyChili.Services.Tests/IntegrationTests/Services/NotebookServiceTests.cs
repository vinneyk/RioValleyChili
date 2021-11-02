using System.Linq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;
using RioValleyChili.Services.Tests.IntegrationTests.Services.TestBases;

namespace RioValleyChili.Services.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class NotebookServiceTests : ServiceIntegrationTestBase<NotebookService>
    {
        [TestFixture]
        public class GetNotebookTests : NotebookServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Notebook_could_not_be_found()
            {
                //Act
                var result = Service.GetNotebook(new NotebookKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.NotebookNotFound);
            }

            [Test]
            public void Returns_Notebook_as_expect_on_success()
            {
                //Arrange
                var notebook = TestHelper.CreateObjectGraphAndInsertIntoDatabase<Notebook>();

                //Act
                var result = Service.GetNotebook(new NotebookKey(notebook).KeyValue);

                //Assert
                result.AssertSuccess();
                notebook.AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class AddNoteTests : NotebookServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Notebook_could_not_be_found()
            {
                //Act
                var result = Service.AddNote(new NotebookKey().KeyValue, new CreateNoteParameters
                    {
                        Text = "ARGARGHRAGH!!!!",
                        UserToken = TestUser.UserName
                    });

                //Assert
                result.AssertNotSuccess(UserMessages.NotebookNotFound);
            }

            [Test]
            public void Creates_new_Note_record_as_expected_on_success()
            {
                //Arrange
                var notebookKey = new NotebookKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Notebook>());

                //Act
                var result = Service.AddNote(notebookKey.KeyValue, new CreateNoteParameters
                    {
                        Text = "ARGARGHRAGH!!!!",
                        UserToken = TestUser.UserName
                    });

                //Assert
                result.AssertSuccess();
                RVCUnitOfWork.NoteRepository.Filter(n => true, n => n.Employee).Single().AssertEqual(result.ResultingObject);
            }
        }

        [TestFixture]
        public class UpdateNoteTests : NotebookServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Note_could_not_be_found()
            {
                //Act
                var result = Service.UpdateNote(new NoteKey().KeyValue, new UpdateNoteParameters());

                //Assert
                result.AssertNotSuccess(UserMessages.NoteNotFound);
            }

            [Test]
            public void Updates_Note_record_as_expected_on_success()
            {
                //Arrange
                var noteKey = new NoteKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Note>());
                var parameters = new UpdateNoteParameters
                    {
                        UserToken = TestUser.UserName,
                        Text = "Something something something else as well."
                    };

                //Act
                var result = Service.UpdateNote(noteKey.KeyValue, parameters);

                //Assert
                result.AssertSuccess();
                var note = RVCUnitOfWork.NoteRepository.FindByKey(noteKey, n => n.Employee);
                Assert.AreEqual(parameters.UserToken, note.Employee.UserName);
                Assert.AreEqual(parameters.Text, note.Text);
            }
        }

        [TestFixture]
        public class DeleteNoteTests : NotebookServiceTests
        {
            [Test]
            public void Returns_non_successful_result_if_Note_could_not_be_found()
            {
                //Act
                var result = Service.DeleteNote(new NoteKey().KeyValue);

                //Assert
                result.AssertNotSuccess(UserMessages.NoteNotFound);
            }

            [Test]
            public void Removes_Note_record_from_database_if_successful()
            {
                //Arrange
                var noteKey = new NoteKey(TestHelper.CreateObjectGraphAndInsertIntoDatabase<Note>());

                //Act
                var result = Service.DeleteNote(noteKey.KeyValue);

                //Assert
                result.AssertSuccess();
                Assert.IsNull(RVCUnitOfWork.NoteRepository.FindByKey(noteKey));
            }
        }
    }
}