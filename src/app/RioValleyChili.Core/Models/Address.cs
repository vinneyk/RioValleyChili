using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Core.Models
{
    [ComplexType]
    public class Address
    {
        [StringLength(Constants.StringLengths.AddressLine)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [StringLength(Constants.StringLengths.AddressLine)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [StringLength(Constants.StringLengths.AddressLine)]
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }

        [StringLength(Constants.StringLengths.AddressCity)]
        public string City { get; set; }

        [StringLength(Constants.StringLengths.AddressState)]
        public string State { get; set; }

        [StringLength(Constants.StringLengths.AddressPostalCode)]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(Constants.StringLengths.AddressCountry)]
        public string Country { get; set; }
    }
}