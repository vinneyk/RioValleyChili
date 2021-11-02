using System;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Abstract.OrderShippingServiceComponent;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using RioValleyChili.Services.Utilities.Helpers;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.Shipment
{
    internal class CreateShipmentInformationCommand
    {
        private readonly IShipmentUnitOfWork _shipmentUnitOfWork;

        internal CreateShipmentInformationCommand(IShipmentUnitOfWork inventoryUnitOfWork)
        {
            if (inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _shipmentUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<ShipmentInformation> Execute(DateTime dateCreated)
        {
            if(dateCreated == null) { throw new ArgumentNullException("dateCreated"); }

            dateCreated = dateCreated.Date;
            var sequence = new EFUnitOfWorkHelper(_shipmentUnitOfWork).GetNextSequence<ShipmentInformation>(s => s.DateCreated == dateCreated, s => s.Sequence);
            var shipmentInfo = _shipmentUnitOfWork.ShipmentInformationRepository.Add(new ShipmentInformation
                {
                    DateCreated = dateCreated,
                    Sequence = sequence
                });
            return new SuccessResult<ShipmentInformation>(shipmentInfo);
        }

        internal IResult<ShipmentInformation> Execute(DateTime dateCreated, IShipmentDetailReturn shipment)
        {
            if(dateCreated == null) { throw new ArgumentNullException("dateCreated"); }

            var createResult = Execute(dateCreated);
            if(!createResult.Success)
            {
                return createResult;
            }
            createResult.ResultingObject.SetShipmentInformation(shipment);

            return createResult;
        }

        internal IResult<ShipmentInformation> Execute(DateTime dateCreated, ISetShipmentInformation shipment)
        {
            if(dateCreated == null) { throw new ArgumentNullException("dateCreated"); }

            var createResult = Execute(dateCreated);
            if(!createResult.Success)
            {
                return createResult;
            }
            createResult.ResultingObject.SetShipmentInformation(shipment);

            return createResult;
        }
    }
}