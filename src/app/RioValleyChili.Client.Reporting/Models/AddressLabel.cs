namespace RioValleyChili.Client.Reporting.Models
{
    public class FacilityAddressLabel : AddressLabel
    {
        public string FacilityName
        {
            get { return _facilityName; }
            set { _facilityName = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        private string _facilityName;
    }

    public class AddressLabel
    {
        public string AttentionLine
        {
            get { return _attentionLine; }
            set { _attentionLine = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string AddressLine1
        {
            get { return _addressLine1; }
            set { _addressLine1 = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string AddressLine2
        {
            get { return _addressLine2; }
            set { _addressLine2 = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string AddressLine3
        {
            get { return _addressLine3; }
            set { _addressLine3 = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        private string _attentionLine;
        private string _addressLine1;
        private string _addressLine2;
        private string _addressLine3;
        private string _companyName;
        private string _phone;
    }
}