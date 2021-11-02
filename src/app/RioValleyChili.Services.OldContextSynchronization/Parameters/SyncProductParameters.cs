using System.Collections.Generic;
using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.OldContextSynchronization.Parameters
{
    public class SyncProductParameters
    {
        public ProductKey ProductKey;
        public List<AdditiveTypeKey> DeletedIngredients;
    }
}