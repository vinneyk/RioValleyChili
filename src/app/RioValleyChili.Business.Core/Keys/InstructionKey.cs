using System;
using System.Globalization;
using System.Linq.Expressions;
using Solutionhead.EntityKey;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Business.Core.Keys
{
    public class InstructionKey : EntityKeyBase.Of<IInstructionKey>, IKey<Instruction>, IInstructionKey
    {
        #region fields and constructors

        private readonly int _instructionId;

        public InstructionKey() { }

        public InstructionKey(IInstructionKey instructionKey)
            : this(instructionKey.InstructionKey_InstructionId) { }

        private InstructionKey(int instructionId)
        {
            _instructionId = instructionId;
        }

        #endregion

        #region Overrides of NewEntityKeyBase.Of<IInstructionKey>

        protected override IInstructionKey ParseImplementation(string keyValue)
        {
            return new InstructionKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return UserMessages.InvalidInstructionKey;
        }

        public override IInstructionKey Default
        {
            get { return Null; }
        }

        protected override IInstructionKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(IInstructionKey key)
        {
            return key.InstructionKey_InstructionId.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<Instruction>

        public Expression<Func<Instruction, bool>> FindByPredicate
        {
            get { return (i => i.Id == _instructionId); }
        }

        #endregion

        #region Implementation of IInstructionKey

        public int InstructionKey_InstructionId
        {
            get { return _instructionId; }
        }

        #endregion

        #region Equality overrides
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(obj, this)) return true;
            return obj as IInstructionKey != null && Equals(obj as IInstructionKey);
        }

        protected bool Equals(IInstructionKey other)
        {
            return _instructionId == other.InstructionKey_InstructionId;
        }

        public override int GetHashCode()
        {
            return _instructionId;
        }

        #endregion

        public static IInstructionKey Null = new NullInstructionKey();
        
        private class NullInstructionKey : IInstructionKey
        {

            #region Implementation of IInstructionKey

            public int InstructionKey_InstructionId { get { return 0; } }

            #endregion
        }
    }
}