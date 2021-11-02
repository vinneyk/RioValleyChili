using System.Collections.Generic;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticNotebooks
    {
        static StaticNotebooks()
        {
            Notebooks = new List<Notebook>
                {
                    (FeedbackNotebook = new Notebook
                        {
                            Date = DataConstants.SqlMinDate,
                            Sequence = 0
                        })
                };
        }

        public static List<Notebook> Notebooks { get; private set; }

        public static Notebook FeedbackNotebook { get; private set; }
    }
}