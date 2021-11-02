using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Core;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableCompany
    {
        public static string Serialize(Models.Company company)
        {
            return JsonConvert.SerializeObject(new SerializableCompany(company), Formatting.None);
        }

        public static SerializableCompany Deserialize(string serializedCompany)
        {
            if(string.IsNullOrWhiteSpace(serializedCompany))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<SerializableCompany>(serializedCompany);
        }

        public SerializableCustomer Customer;
        public IEnumerable<CompanyType> Types;

        public class SerializableCustomer
        {
            public string Broker;
        }

        private SerializableCompany(Models.Company company)
        {
            Types = company.CompanyTypes.Select(t => t.CompanyTypeEnum).Distinct().ToList();
            if(company.Customer != null)
            {
                Customer = new SerializableCustomer
                    {
                        Broker = company.Customer.Broker.Name
                    };
            }
            else
            {
                Customer = null;
            }
        }
    }
}