using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Core.Models
{
    [ComplexType]
    public class ShippingLabel
    {
        public ShippingLabel()
        {
            Address = new Address();
        }

        [StringLength(Constants.StringLengths.ShippingLabelName)]
        public string Name { get; set; }
        [StringLength(Constants.StringLengths.PhoneNumber)]
        public string Phone { get; set; }
        [StringLength(Constants.StringLengths.Email)]
        public string EMail { get; set; }
        [StringLength(Constants.StringLengths.Fax)]
        public string Fax { get; set; }

        public Address Address { get; set; }
    }
}