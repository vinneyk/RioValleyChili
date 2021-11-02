using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.PocoMothers
{
    public class InstructionMother
    {
        public static Instruction US20
        {
            get
            {
                return new Instruction
                           {
                               InstructionText =  "US 20",
                               Id = 1,
                               //InstructionType = 
                           };
            }
        }

        public static Instruction Pull1JarAnd1SandwichBag
        {
            get
            {
                return new Instruction
                {
                    InstructionText = "Pull 1 jar and 1 sandwich size bag total 2 samples/Saca 2 muestras 1 frasco y 1 bolsa de sandwich",
                    Id = 2,
                    //InstructionType = 
                };
            }
        }
    }
}
