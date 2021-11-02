using System;
using System.Text.RegularExpressions;
using System.Web;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Utilities.Helpers
{
    public static class LotTypeHelper
    {
        public static LotTypeEnum ParseFromKeyValue(string lotKeyValue)
        {
            var reg = new Regex("^0?[0-9]");
            if (!reg.IsMatch(lotKeyValue))
            {
                throw new HttpRequestValidationException("The supplied Lot Number is invalid.");
            }
            var lotTypeVal = reg.Match(lotKeyValue).Value;

            try
            {
                var parsedObject = Enum.Parse(typeof(LotTypeEnum), lotTypeVal);
                return (LotTypeEnum)parsedObject;
            }
            catch (Exception)
            {
                throw new ArgumentException(String.Format("The supplied Lot Number is invalid. The lot type could not be inferred. Value received: '{0}'.", lotKeyValue));
            }
        }

        public static LotTypeEnum FromInventoryType(InventoryType inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Chile:
                    return LotTypeEnum.FinishedGood;
                case InventoryType.Packaging:
                    return LotTypeEnum.Packaging;
                case InventoryType.Additive:
                    return LotTypeEnum.Additive;
                default :
                    throw new NotImplementedException(string.Format("The InventoryType '{0}' is not supported.", inventoryType));
            }
        }

        public static LotTypeEnum FromProductType(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Chile:
                    return LotTypeEnum.FinishedGood;
                case ProductTypeEnum.Packaging:
                    return LotTypeEnum.Packaging;
                case ProductTypeEnum.Additive:
                    return LotTypeEnum.Additive;
                default :
                    throw new NotImplementedException(string.Format("The ProductTypeEnum '{0}' is not supported.", productType));
            }
        }
    }
}