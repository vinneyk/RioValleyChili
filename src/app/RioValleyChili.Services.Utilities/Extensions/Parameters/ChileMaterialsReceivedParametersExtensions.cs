using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.MaterialsReceivedService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class ChileMaterialsReceivedParametersExtensions
    {
        internal static IResult<CreateChileMaterialsReceivedParameters> ToParsedParameters(this ICreateChileMaterialsReceivedParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.Items == null || !parameters.Items.Any())
            {
                return new InvalidResult<CreateChileMaterialsReceivedParameters>(null, UserMessages.CannotCreateChileReceivedWithNoItems);
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<CreateChileMaterialsReceivedParameters>();
            }

            var treatmentKey = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
            if(!treatmentKey.Success)
            {
                return treatmentKey.ConvertTo<CreateChileMaterialsReceivedParameters>();
            }

            var supplierKey = KeyParserHelper.ParseResult<ICompanyKey>(parameters.SupplierKey);
            if(!supplierKey.Success)
            {
                return supplierKey.ConvertTo<CreateChileMaterialsReceivedParameters>();
            }

            var items = new List<SetChileMaterialsReceivedItemParameters>();
            foreach(var item in parameters.Items)
            {
                var parsedItemResult = item.ToParsedParameters();
                if(!parsedItemResult.Success)
                {
                    return parsedItemResult.ConvertTo<CreateChileMaterialsReceivedParameters>();
                }

                items.Add(parsedItemResult.ResultingObject);
            }

            return new SuccessResult<CreateChileMaterialsReceivedParameters>(new CreateChileMaterialsReceivedParameters
                {
                    Params = parameters,

                    ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey(),
                    SupplierKey = supplierKey.ResultingObject.ToCompanyKey(),
                    TreatmentKey = treatmentKey.ResultingObject.ToInventoryTreatmentKey(),

                    Items = items
                });
        }

        internal static IResult<UpdateChileMaterialsReceivedParameters> ToParsedParameters(this IUpdateChileMaterialsReceivedParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKey = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
            if(!lotKey.Success)
            {
                return lotKey.ConvertTo<UpdateChileMaterialsReceivedParameters>();
            }

            var chileProductKeyResult = KeyParserHelper.ParseResult<IChileProductKey>(parameters.ChileProductKey);
            if(!chileProductKeyResult.Success)
            {
                return chileProductKeyResult.ConvertTo<UpdateChileMaterialsReceivedParameters>();
            }

            var treatmentKey = KeyParserHelper.ParseResult<IInventoryTreatmentKey>(parameters.TreatmentKey);
            if(!treatmentKey.Success)
            {
                return treatmentKey.ConvertTo<UpdateChileMaterialsReceivedParameters>();
            }

            var supplierKey = KeyParserHelper.ParseResult<ICompanyKey>(parameters.SupplierKey);
            if(!supplierKey.Success)
            {
                return supplierKey.ConvertTo<UpdateChileMaterialsReceivedParameters>();
            }
            
            var items = new List<SetChileMaterialsReceivedItemParameters>();
            if(parameters.Items != null)
            {
                foreach(var item in parameters.Items)
                {
                    var parsedItemResult = item.ToParsedParameters();
                    if(!parsedItemResult.Success)
                    {
                        return parsedItemResult.ConvertTo<UpdateChileMaterialsReceivedParameters>();
                    }

                    items.Add(parsedItemResult.ResultingObject);
                }
            }

            return new SuccessResult<UpdateChileMaterialsReceivedParameters>(new UpdateChileMaterialsReceivedParameters
            {
                Params = parameters,
                LotKey = lotKey.ResultingObject.ToLotKey(),

                ChileProductKey = chileProductKeyResult.ResultingObject.ToChileProductKey(),
                SupplierKey = supplierKey.ResultingObject.ToCompanyKey(),
                TreatmentKey = treatmentKey.ResultingObject.ToInventoryTreatmentKey(),

                Items = items
            });
        }
    }
}