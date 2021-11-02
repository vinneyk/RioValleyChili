using System;
using System.Collections.Generic;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class ContactKey
    {
        public string Name { get; private set; }

        public string PhoneNumber { get; private set; }

        public string EMailAddress { get; private set; }

        public ContactKey(Contact oldContact)
        {
            Name = oldContact.Contact_IA;
            PhoneNumber = oldContact.Phone_IA;
            EMailAddress = oldContact.EmailAddress_IA;
        }

        public static readonly IEqualityComparer<ContactKey> ContactKeyEqualityComparer = new ContactKeyEqualityComparerObject();

        private class ContactKeyEqualityComparerObject : IEqualityComparer<ContactKey>
        {
            public bool Equals(ContactKey x, ContactKey y)
            {
                if(x == y) { return true; }
                return String.Equals(x.Name, y.Name) &&
                       String.Equals(x.PhoneNumber, y.PhoneNumber) &&
                       String.Equals(x.EMailAddress, y.EMailAddress);
            }

            public int GetHashCode(ContactKey obj)
            {
                unchecked
                {
                    var hashCode = (obj.Name != null ? obj.Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.PhoneNumber != null ? obj.PhoneNumber.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.EMailAddress != null ? obj.EMailAddress.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}