using System.Collections.Generic;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class AddressHelper
    {
        public static Address ToAddress(this Contact contact)
        {
            return new Address
                {
                    AddressLine1 = contact.Address1_IA,
                    AddressLine2 = contact.Address2_IA,
                    AddressLine3 = contact.Address3_IA,
                    City = contact.City_IA,
                    State = contact.State_IA,
                    PostalCode = contact.Zip_IA,
                    Country = contact.Country_IA
                };
        }

        public static readonly IEqualityComparer<Address> AddressEqualityComparer = new AddressEqualityComparerObject();

        private class AddressEqualityComparerObject : IEqualityComparer<Address>
        {
            public bool Equals(Address x, Address y)
            {
                if(x == y) { return true; }
                return string.Equals(x.AddressLine1, y.AddressLine1) &&
                    string.Equals(x.AddressLine2, y.AddressLine2) &&
                    string.Equals(x.AddressLine3, y.AddressLine3) &&
                    string.Equals(x.City, y.City) &&
                    string.Equals(x.State, y.State) &&
                    string.Equals(x.PostalCode, y.PostalCode) &&
                    string.Equals(x.Country, y.Country);
            }

            public int GetHashCode(Address obj)
            {
                unchecked
                {
                    var hashCode = (obj.AddressLine1 != null ? obj.AddressLine1.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.AddressLine2 != null ? obj.AddressLine2.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.AddressLine3 != null ? obj.AddressLine3.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.City != null ? obj.City.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.State != null ? obj.State.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.PostalCode != null ? obj.PostalCode.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Country != null ? obj.Country.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}