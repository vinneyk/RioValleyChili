using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class ChileTypeMother : PocoMother<ChileType>
    {
        public static ChileType ChiliPepper 
        { 
            get
            {
                return new ChileType
                           {
                               Id = 1,
                               Description = "Chili Pepper",
                           };
            } 
        }

        public static ChileType ChiliPowder
        { 
            get
            {
                return new ChileType
                           {
                               Id = 2,
                               Description = "Chili Powder",
                           };
            } 
        }

        public static ChileType Paprika
        { 
            get
            {
                return new ChileType
                           {
                               Id = 3,
                               Description = "Paprika"
                           };
            } 
        }

        public static ChileType RedPepper
        { 
            get
            {
                return new ChileType
                           {
                               Id = 4,
                               Description = "Red Pepper"
                           };
            } 
        }

        public static ChileType GroundRedPepper
        { 
            get
            {
                return new ChileType
                           {
                               Id = 12,
                               Description = "GRP",
                           };
            } 
        }

        public static ChileType Other
        { 
            get
            {
                return new ChileType
                           {
                               Id = 7,
                               Description = "Other"
                           };
            } 
        }
    }
}
