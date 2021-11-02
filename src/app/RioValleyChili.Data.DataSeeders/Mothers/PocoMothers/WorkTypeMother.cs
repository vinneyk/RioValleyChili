using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class WorkTypeMother : PocoMother<WorkType>
    {
        public static WorkType NewProduct
        {
            get
            {
                return new WorkType
                {
                    Description = "New",
                    Id = 1,
                    TimeStamp = TimeStampHelper.Current.CurrentTimeStamp,
                    User = "Load"
                };
            }
        }

        public static WorkType Rework
        {
            get
            {
                return new WorkType
                {
                    Description = "Rework",
                    Id = 2,
                    TimeStamp = TimeStampHelper.Current.CurrentTimeStamp,
                    User = "Load"
                };
            }
        }

        public static WorkType Relabel
        {
            get
            {
                return new WorkType
                {
                    Description = "Relabel",
                    Id = 3,
                    TimeStamp = TimeStampHelper.Current.CurrentTimeStamp,
                    User = "Load"
                };
            }
        }

        public static WorkType Repack
        {
            get
            {
                return new WorkType
                {
                    Description = "Repack",
                    Id = 4,
                    TimeStamp = TimeStampHelper.Current.CurrentTimeStamp,
                    User = "Load"
                };
            }
        }
    }
}
