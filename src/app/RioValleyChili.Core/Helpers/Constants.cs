using System.Text;

namespace RioValleyChili.Core.Helpers
{
    public static class Constants
    {
        public static class StringLengths
        {
            public const int ShippingLabelName = 50;
            public const int ShippingLabelAdditionalInfo = 100;
            public const int AddressLine = 50;
            public const int AddressCity = 75;
            public const int AddressState = 50;
            public const int AddressPostalCode = 15;
            public const int AddressCountry = 50;
            public const int User = 25;
            public const int AttributeShortName = 10;
            public const int CustomerProductCode = 100;
            public const int CustomerLotCode = 100;
            public const int SampleItem = 50;
            public const int PaymentTerms = 25;
            public const int PurchaseOrderNumber = 50;
            public const int ShipperNumber = 50;
            public const int ToteKeyLength = 15;
            public const int InstructionText = 250;
            public const int PhoneNumber = 50;
            public const int Email = 50;
            public const int Fax = 50;
            public const int ContactName = 60;
            public const int AddressDescription = 25;
            public const int CompanyName = 100;
            public const int GrowerCode = 20;
            public const int FreightBillType = 25;
            public const int ShipmentMethod = 25;
            public const int DriverName = 25;
            public const int CarrierName = 25;
            public const int TrailerLicenseNumber = 25;
            public const int ContainerSeal = 25;
            public const int ShipmentInformationNotes = 600;
            public const int LotHoldDescription = 50;
            public const int PackScheduleSummaryOfWork = 250;
            public const int LotNotes = 255;
            public const int OrderNumber = 50;
            public const int FOB = 30;
            public const int LocationDescription = 20;
            public const int CustomerNoteType = 25;
            public const int FacilityName = 150;
            public const int ChileVariety = 25;
            public const int InvoiceNotes = 600;
            public const int SampleOrderNotes = 300;
            public const int SampleMatchAttribute = 50;
            public const int LoadNumber = 20;

            public static object ToObject()
            {
                return new
                    {
                        ShipmentInformationNotes
                    };
            }

            public static string ToJson()
            {
                //todo: replace with Newtonsoft.Json when versions have been consolidated
                var jsonBuilder = new StringBuilder();
                jsonBuilder.Append("{");
                jsonBuilder.AppendFormat("shipmentInformationNotes:{0}", ShipmentInformationNotes);
                jsonBuilder.Append("}");
                return jsonBuilder.ToString();
            }
        }

        public static class ChileAttributeKeys
        {
            public const string AB = "AB";
            public const string AIA = "AIA";
            public const string Ash = "Ash";
            public const string Asta = "Asta";
            public const string AToxin = "AToxin";
            public const string BI = "BI";
            public const string ColiF = "ColiF";
            public const string EColi = "EColi";
            public const string Ethox = "Ethox";
            public const string Gluten = "Gluten";
            public const string Gran = "Gran";
            public const string H2O = "H2O";
            public const string InsP = "InsP";
            public const string Lead = "Lead";
            public const string Mold = "Mold";
            public const string RodHrs = "Rod Hrs";
            public const string Sal = "Sal";
            public const string Scan = "Scan";
            public const string Scov = "Scov";
            public const string TPC = "TPC";
            public const string Yeast = "Yeast";
        }

        public static class Reporting
        {
            public const double DefaultOrderPalletWeight = 60;
        }

        public static class StaticKeyValues
        {
            public const int RinconFacilityKey = 2;
        }
    }
}
