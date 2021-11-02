using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Services.OldContextSynchronization.Tests.Base;
using RioValleyChili.Services.Tests;
using RioValleyChili.Services.Tests.IntegrationTests.Parameters;

namespace RioValleyChili.Services.OldContextSynchronization.Tests
{
    [TestFixture]
    public class NotebookServiceTests
    {
        [TestFixture]
        public class AddNoteIntegrationTests : SynchronizeOldContextIntegrationTestsBase<NotebookService>
        {
            [Test]
            public void Adds_new_tblBatchInstr_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                const string testnote = "TestNote!";
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b => b.InstructionNotebook.Notes.All(n => n.Text != testnote), b => b.InstructionNotebook).FirstOrDefault();
                if(productionBatch == null)
                {
                    Assert.Inconclusive("No suitable test ProductionBatch found.");
                }

                //Act
                var result = Service.AddNote(new NotebookKey(productionBatch.InstructionNotebook), new CreateNoteParameters
                    {
                        UserToken = TestUser.UserName,
                        Text = testnote
                    });
                var lotKeyString = GetKeyFromConsoleString(ConsoleOutput.UpdatedBatchInstructions);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lot = int.Parse(lotKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var instructions = oldContext.tblBatchInstrs.Where(i => i.Lot == lot);
                    Assert.AreEqual(1, instructions.Count(i => i.Action == testnote));
                }
            }
        }

        [TestFixture]
        public class UpdateNoteIntegrationTests : SynchronizeOldContextIntegrationTestsBase<NotebookService>
        {
            [Test]
            public void Updates_tblBatchInstr_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                const string testnote = "TestNote!";

                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b => b.InstructionNotebook.Notes.Count(n => n.Text != testnote) >= 1, b => b.InstructionNotebook.Notes).FirstOrDefault();
                if(productionBatch == null)
                {
                    Assert.Inconclusive("No suitable test ProductionBatch found.");
                }
                var note = productionBatch.InstructionNotebook.Notes.First();

                //Act
                var result = Service.UpdateNote(new NoteKey(note), new UpdateNoteParameters
                    {
                        UserToken = TestUser.UserName,
                        Text = testnote
                    });
                var lotKeyString = GetKeyFromConsoleString(ConsoleOutput.UpdatedBatchInstructions);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lot = int.Parse(lotKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var instructions = oldContext.tblBatchInstrs.Where(i => i.Lot == lot);
                    Assert.AreEqual(1, instructions.Count(i => i.Step == note.Sequence && i.Action == testnote));
                    Assert.AreEqual(0, instructions.Count(i => i.Action == note.Text));
                }
            }

            [Test]
            public void Serializes_Contract_with_updated_Comments_Note_in_old_context()
            {
                //Arrange
                const string updatedNote = "Updated contract comment!";
                var contract = RVCUnitOfWork.ContractRepository
                    .Filter(c => c.Comments.Notes.Count() == 1 && c.Comments.Notes.All(n => n.Text != updatedNote), c => c.Comments.Notes)
                    .FirstOrDefault();
                if(contract == null)
                {
                    Assert.Inconclusive("No suitable test contract found.");
                }
                var noteKey = new NoteKey(contract.Comments.Notes.Single());

                //Assert
                var result = Service.UpdateNote(noteKey, new UpdateNoteParameters
                    {
                        UserToken = TestUser.UserName,
                        Text = updatedNote
                    });
                var contractKeyString = GetKeyFromConsoleString(ConsoleOutput.SynchronizedContract);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var contractId = int.Parse(contractKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var serialized = oldContext.tblContracts.First(c => c.ContractID == contractId).Serialized;
                    var serializedContract = SerializableContract.Deserialize(serialized);
                    Assert.AreEqual(updatedNote, serializedContract.Notes.Single().Text);
                }
            }
        }

        [TestFixture]
        public class DeleteNoteIntegrationTests : SynchronizeOldContextIntegrationTestsBase<NotebookService>
        {
            [Test]
            public void Removes_tblBatchInstr_record_and_KillSwitch_will_not_have_been_engaged_if_service_method_and_synchronization_were_successful()
            {
                //Arrange
                
                var productionBatch = RVCUnitOfWork.ProductionBatchRepository.Filter(b => b.InstructionNotebook.Notes.Any(), b => b.InstructionNotebook.Notes).FirstOrDefault();
                if(productionBatch == null)
                {
                    Assert.Inconclusive("No suitable test ProductionBatch found.");
                }
                var note = productionBatch.InstructionNotebook.Notes.First();

                //Act
                var result = Service.DeleteNote(new NoteKey(note));
                var lotKeyString = GetKeyFromConsoleString(ConsoleOutput.UpdatedBatchInstructions);

                //Assert
                result.AssertSuccess();
                MockKillSwitch.Verify(k => k.Engage(), Times.Never());

                var lot = int.Parse(lotKeyString);
                using(var oldContext = new RioAccessSQLEntities())
                {
                    var instructions = oldContext.tblBatchInstrs.Where(i => i.Lot == lot);
                    Assert.AreEqual(0, instructions.Count(i => i.Action == note.Text));
                }
            }
        }
    }
}