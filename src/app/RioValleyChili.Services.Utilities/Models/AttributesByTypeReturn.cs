using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class AttributesByTypeReturn : IAttributesByProductType
    {
        public IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> AttributeNamesByProductType
        {
            get
            {
                return AttributeNamesAndTypes == null ? null :
                    _attributeNamesByProductType ?? (_attributeNamesByProductType = new Dictionary<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>
                    {
                        { ProductTypeEnum.Chile, AttributeNamesAndTypes.Where(a => a.ValidForChile).Select(a => new KeyValuePair<string, string>(a.Key, a.Name)) },
                        { ProductTypeEnum.Additive, AttributeNamesAndTypes.Where(a => a.ValidForAdditive).Select(a => new KeyValuePair<string, string>(a.Key, a.Name)) },
                        { ProductTypeEnum.Packaging, AttributeNamesAndTypes.Where(a => a.ValidForPackaging).Select(a => new KeyValuePair<string, string>(a.Key, a.Name)) },
                    });
            }
        }

        internal IEnumerable<AttributeNameAndType> AttributeNamesAndTypes { get; set; }

        private IEnumerable<KeyValuePair<ProductTypeEnum, IEnumerable<KeyValuePair<string, string>>>> _attributeNamesByProductType;
    }
}