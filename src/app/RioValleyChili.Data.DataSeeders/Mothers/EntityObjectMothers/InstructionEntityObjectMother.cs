using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class InstructionEntityObjectMother : EntityMotherLogBase<InstructionEntityObjectMother.InstructionResult, InstructionEntityObjectMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public InstructionEntityObjectMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) {  throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        public enum EntityTypes
        {
            Note,
            Instruction
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<InstructionResult> BirthRecords()
        {
            _loadCount.Reset();
            var instructions = new HashSet<string>();

            foreach(var oldInstructionsByLot in SelectBatchInstructionsToLoad(OldContext).GroupBy(i => i.Lot))
            {
                var lastSequence = 0;
                foreach(var oldInstruction in oldInstructionsByLot.OrderBy(i => i.Step))
                {
                    _loadCount.AddRead(EntityTypes.Note);

                    if(string.IsNullOrWhiteSpace(oldInstruction.Action))
                    {
                        Log(new CallbackParameters(CallbackReason.EmptyAction)
                            {
                                Instruction = oldInstruction
                            });
                        continue;
                    }

                    var lotKey = LotNumberParser.ParseLotNumber(oldInstruction.Lot);
                    if(lotKey == null)
                    {
                        Log(new CallbackParameters(CallbackReason.InvalidLotKey)
                            {
                                Instruction = oldInstruction
                            });
                        continue;
                    }

                    var notebookKey = _newContextHelper.GetBatchInstructionNotebookKeyByLotKey(lotKey);
                    if(notebookKey == null)
                    {
                        Log(new CallbackParameters(CallbackReason.NotebookNotLoaded)
                            {
                                Instruction = oldInstruction
                            });
                        continue;
                    }

                    var instruction = new string(oldInstruction.Action.Take(Constants.StringLengths.InstructionText).ToArray());
                    Instruction newInstruction = null;
                    if(!instructions.Contains(instruction))
                    {
                        _loadCount.AddRead(EntityTypes.Instruction);
                        newInstruction = new Instruction
                            {
                                InstructionText = instruction,
                                InstructionType = InstructionType.ProductionBatchInstruction
                            };
                        instructions.Add(instruction);
                        _loadCount.AddLoaded(EntityTypes.Instruction);
                    }

                    var sequence = oldInstruction.Step.Value;
                    if(sequence <= lastSequence)
                    {
                        sequence = lastSequence + 1;
                    }
                    lastSequence = sequence;
                    
                    var newNote = new Note
                        {
                            EmployeeId = _newContextHelper.DefaultEmployee.EmployeeId,
                            TimeStamp = oldInstruction.EntryDate.ConvertLocalToUTC(),
                            NotebookDate = notebookKey.NotebookKey_Date,
                            NotebookSequence = notebookKey.NotebookKey_Sequence,
                            Sequence = sequence,
                            Text = oldInstruction.Action
                        };
                    SerializableBatchInstruction.DeserializeIntoNote(newNote, oldInstruction.Serialized);

                    _loadCount.AddLoaded(EntityTypes.Note);
                    yield return new InstructionResult
                        {
                            Note = newNote,
                            Instruction = newInstruction
                        };
                }
            }

            _loadCount.LogResults(s => Log(new CallbackParameters(s)));
        }

        private static List<TblBatchInstructionDTO> SelectBatchInstructionsToLoad(ObjectContext objectContext)
        {
            return objectContext.CreateObjectSet<tblBatchInstr>().AsNoTracking().Select(i => new TblBatchInstructionDTO
                {
                    EntryDate = i.EntryDate,
                    Lot = i.Lot,
                    Step = i.Step,
                    Action = i.Action,
                    Serialized = i.Serialized
                }).ToList();
        }

        public class TblBatchInstructionDTO
        {
            public DateTime EntryDate { get; set; }
            public int Lot { get; set; }
            public int? Step { get; set; }
            public string Action { get; set; }
            public string Serialized { get; set; }
        }

        public class InstructionResult
        {
            public Note Note;
            public Instruction Instruction;
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            EmptyAction,
            InvalidLotKey,
            NotebookNotLoaded,
            StringTruncated
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            protected override CallbackReason ExceptionReason { get { return InstructionEntityObjectMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return InstructionEntityObjectMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return InstructionEntityObjectMother.CallbackReason.StringTruncated; } }

            public CallbackParameters() { }
            public CallbackParameters(DataStringPropertyHelper.Result result) : base(result) { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }

            public TblBatchInstructionDTO Instruction { get; set; }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case InstructionEntityObjectMother.CallbackReason.Exception:
                        return ReasonCategory.Error;
                    
                    case InstructionEntityObjectMother.CallbackReason.EmptyAction:
                    case InstructionEntityObjectMother.CallbackReason.InvalidLotKey:
                    case InstructionEntityObjectMother.CallbackReason.NotebookNotLoaded:
                        return ReasonCategory.RecordSkipped;
                }

                return base.DerivedGetReasonCategory(reason);
            }
        }
    }
}