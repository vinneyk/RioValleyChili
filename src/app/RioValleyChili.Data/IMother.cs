using System;
using System.Collections.Generic;

namespace RioValleyChili.Data
{
    public interface IMother<TChild> where TChild : class
    {
        IEnumerable<TChild> BirthAll(Action consoleCallback = null);
    }

    public interface IProcessedMother<TChild> where TChild : class
    {
        void ProcessedBirthAll(Action<TChild> processResult, Action consoleCallback = null);
    }
}