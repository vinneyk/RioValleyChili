using System;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class UpdateChileMaterialsReceivedConductor : ChileMaterialsReceivedConductorBase
    {
        internal UpdateChileMaterialsReceivedConductor(IMaterialsReceivedUnitOfWork materialsReceivedUnitOfWork) : base(materialsReceivedUnitOfWork) { }

        internal IResult<ChileMaterialsReceived> Execute(UpdateChileMaterialsReceivedParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(MaterialsReceivedUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileMaterialsReceived>();
            }

            var materialsReceived = MaterialsReceivedUnitOfWork.ChileMaterialsReceivedRepository.FindByKey(parameters.LotKey,
                m => m.ChileLot.Lot,
                m => m.Items);
            if(materialsReceived == null)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.ChileMaterialsReceivedNotFound, parameters.LotKey));
            }
            
            var chileProduct = MaterialsReceivedUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey);
            if(chileProduct == null)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey));
            }

            if(parameters.Params.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Dehydrated && chileProduct.ChileState != ChileStateEnum.Dehydrated)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.ChileProductInvalidState, parameters.ChileProductKey.KeyValue, chileProduct.ChileState, ChileStateEnum.Dehydrated));
            }

            return Set(materialsReceived, chileProduct, parameters, employeeResult.ResultingObject, timeStamp);
        }
    }
}