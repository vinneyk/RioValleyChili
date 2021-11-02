using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Serializable
{
    [JsonObject(MemberSerialization.Fields)]
    public class SerializableCustomerSpec
    {
        public List<AttributeRange> AttributeRanges;

        public static string Serialize(IEnumerable<CustomerProductAttributeRange> productSpecs)
        {
            return JsonConvert.SerializeObject(new SerializableCustomerSpec(productSpecs));
        }

        public static SerializableCustomerSpec Deserialize(string serializedSpecs)
        {
            return JsonConvert.DeserializeObject<SerializableCustomerSpec>(serializedSpecs);
        }

        private SerializableCustomerSpec(IEnumerable<CustomerProductAttributeRange> productSpecs)
        {
            AttributeRanges = productSpecs.Select(r => new AttributeRange
                {
                    AttributeNameKey = r.ToAttributeNameKey(),
                    EmployeeKey = r.ToEmployeeKey(),
                    TimeStamp = r.TimeStamp,
                    Active = r.Active,
                    RangeMin = r.RangeMin,
                    RangeMax = r.RangeMax
                }).ToList();
        }

        public List<CustomerProductAttributeRange> ToDataModels(IChileProductKey chileProduct, ICompanyKey customer)
        {
            var employeeKeyParser = new EmployeeKey();
            var attributeNameKeyParser = new AttributeNameKey();

            return AttributeRanges.Select(r => new CustomerProductAttributeRange
                {
                    EmployeeId = employeeKeyParser.Parse(r.EmployeeKey).EmployeeKey_Id,
                    TimeStamp = r.TimeStamp,

                    CustomerId = customer.CompanyKey_Id,
                    ChileProductId = chileProduct.ChileProductKey_ProductId,
                    AttributeShortName = attributeNameKeyParser.Parse(r.AttributeNameKey).AttributeNameKey_ShortName,

                    Active = r.Active,
                    RangeMin = r.RangeMin,
                    RangeMax = r.RangeMax
                }).ToList();
        }

        public class AttributeRange
        {
            public string EmployeeKey;
            public DateTime TimeStamp;

            public string AttributeNameKey;
            public double RangeMin, RangeMax;
            public bool Active;
        }
    }
}