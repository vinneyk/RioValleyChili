using System;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.SalesService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ContractItemExtensions
    {
        internal static IResult<SetContractItemParameters> ToParsedParameters(this IContractItem item)
        {
            if(item == null) { throw new ArgumentNullException("item"); }

            if(item.Quantity <= 0)
            {
                return new InvalidResult<SetContractItemParameters>(null, UserMessages.QuantityNotGreaterThanZero);
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(item.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo((SetContractItemParameters) null);
            }

            var packagingProductKeyResult = KeyParserHelper.ParseResult<IPackagingProductKey>(item.PackagingProductKey);
            if(!packagingProductKeyResult.Success)
            {
                return packagingProductKeyResult.ConvertTo((SetContractItemParameters)null);
            }

            var treatmentKeyResult = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(item.TreatmentKey);
            if(!treatmentKeyResult.Success)
            {
                return treatmentKeyResult.ConvertTo((SetContractItemParameters)null);
            }

            return new SuccessResult<SetContractItemParameters>(new SetContractItemParameters
                {
                    ContractItemParameters = item,
                    ChileProductKey = new ChileProductKey(chileProductKeyResult.ResultingObject),
                    PackagingProductKey = new PackagingProductKey(packagingProductKeyResult.ResultingObject),
                    TreatmentKey = new InventoryTreatmentKey(treatmentKeyResult.ResultingObject)
                });
        }
    }
}