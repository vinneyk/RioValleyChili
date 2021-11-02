using System;
using System.Globalization;

namespace LotAttributeFix
{
    class Program
    {
        static void Main(string[] args)
        {
            var dateParsed = false;
            while(!dateParsed)
            {
                Console.WriteLine("Enter start date (yyyy-M-d):");
                var inputString = Console.ReadLine();

                DateTime startDate;
                dateParsed = DateTime.TryParseExact(inputString, "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                if(dateParsed)
                {
                    ProcessLots.Process(startDate);
                }
            }

            Console.WriteLine("Press any key to exit.");
            var keyInfo = Console.ReadKey();
            Console.WriteLine("Goodbye, {0}", keyInfo.Key);
        }
    }
}