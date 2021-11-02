using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class AdditiveTypes
    {
        public static AdditiveType Unknown
        {
            get
            {
                return new AdditiveType
                {
                    Description = "Unknown",
                    Id = 99
                };
            }
        }
    }
}
