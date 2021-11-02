using System;
using System.Collections.Generic;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class CreateChileMaterialsReceivedConductor : ChileMaterialsReceivedConductorBase
    {
        internal CreateChileMaterialsReceivedConductor(IMaterialsReceivedUnitOfWork materialsReceivedUnitOfWork) : base(materialsReceivedUnitOfWork) { }

        internal IResult<ChileMaterialsReceived> Execute(CreateChileMaterialsReceivedParameters parameters, DateTime timeStamp)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(MaterialsReceivedUnitOfWork).GetEmployee(parameters.Params);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ChileMaterialsReceived>();
            }

            var chileProduct = MaterialsReceivedUnitOfWork.ChileProductRepository.FindByKey(parameters.ChileProductKey);
            if(chileProduct == null)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.ChileProductNotFound, parameters.ChileProductKey.KeyValue));
            }

            if(parameters.Params.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Dehydrated && chileProduct.ChileState != ChileStateEnum.Dehydrated)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.ChileProductInvalidState, parameters.ChileProductKey.KeyValue, chileProduct.ChileState, ChileStateEnum.Dehydrated));
            }

            var createChileLotResult = new CreateNewChileLotCommand(MaterialsReceivedUnitOfWork).Execute(new CreateNewChileLotCommandParameters
                {
                    EmployeeKey = employeeResult.ResultingObject,
                    TimeStamp = timeStamp,
                    LotDate = parameters.Params.DateReceived.Date,
                    LotType = LotTypeEnum.DeHydrated,
                    ChileProductKey = chileProduct,
                    SetLotProductionStatus = LotProductionStatus.Produced,
                    SetLotQualityStatus = LotQualityStatus.Pending
                });
            if(!createChileLotResult.Success)
            {
                return createChileLotResult.ConvertTo<ChileMaterialsReceived>();
            }
            var chileLot = createChileLotResult.ResultingObject;

            var materialsReceived = MaterialsReceivedUnitOfWork.ChileMaterialsReceivedRepository.Add(new ChileMaterialsReceived
                {
                    ChileLot = chileLot,
                    LotDateCreated = chileLot.LotDateCreated,
                    LotDateSequence = chileLot.LotDateSequence,
                    LotTypeId = chileLot.LotTypeId,
                    
                    Items = new List<ChileMaterialsReceivedItem>()
                });

            return Set(materialsReceived, chileProduct, parameters, employeeResult.ResultingObject, timeStamp);
        }
    }
}
