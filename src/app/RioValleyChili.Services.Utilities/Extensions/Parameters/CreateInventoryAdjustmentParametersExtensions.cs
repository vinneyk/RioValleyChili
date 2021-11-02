using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Services.Interfaces.Parameters.InventoryAdjustmentsService;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.UtilityModels;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.Parameters
{
    internal static class CreateInventoryAdjustmentParametersExtensions
    {
        internal static IResult<CreateInventoryAdjustmentConductorParameters> ToParsedParameters(this ICreateInventoryAdjustmentParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            if(parameters.InventoryAdjustments == null || !parameters.InventoryAdjustments.Any())
            {
                return new InvalidResult<CreateInventoryAdjustmentConductorParameters>(null, UserMessages.InventoryAdjustmentItemsRequired);
            }

            var parsedItems = new List<CreateInventoryAdjustmentItemCommandParameters>();
            foreach(var adjustment in parameters.InventoryAdjustments)
            {
                var parsedItemResult = adjustment.ToParsedParameters();
                if(!parsedItemResult.Success)
                {
                    return parsedItemResult.ConvertTo((CreateInventoryAdjustmentConductorParameters) null);
                }
                parsedItems.Add(parsedItemResult.ResultingObject);
            }

            return new SuccessResult<CreateInventoryAdjustmentConductorParameters>(new CreateInventoryAdjustmentConductorParameters
                {
                    CreateInventoryAdjustmentParameters = parameters,
                    Items = parsedItems
                });
        }
    }
}