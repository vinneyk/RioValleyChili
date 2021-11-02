using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class InstructionLogger : EntityLoggerBase<InstructionEntityObjectMother.CallbackParameters, InstructionEntityObjectMother.CallbackReason>
    {
        public InstructionLogger(string logFilePath) : base(logFilePath) { }

        protected override string GetLogMessage(InstructionEntityObjectMother.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case InstructionEntityObjectMother.CallbackReason.Exception:
                    return GetExceptionString(parameters.Exception);

                case InstructionEntityObjectMother.CallbackReason.EmptyAction:
                    return string.Format("{0} Action is null or empty.", ParametersString(parameters));

                case InstructionEntityObjectMother.CallbackReason.InvalidLotKey:
                    return string.Format("{0} Lot number could not be parsed.", ParametersString(parameters));

                case InstructionEntityObjectMother.CallbackReason.NotebookNotLoaded:
                    return string.Format("{0} Notebook record not loaded", ParametersString(parameters));
            }

            return null;
        }

        private static string ParametersString(InstructionEntityObjectMother.CallbackParameters parameters)
        {
            return string.Format("Instruction EntryDate[{0}] Lot[{1}]", parameters.Instruction.EntryDate, parameters.Instruction.Lot);
        }
    }
}