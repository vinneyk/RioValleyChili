using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal abstract class CreateNewLotCommandBase
    {
        protected readonly ILotUnitOfWork LotUnitOfWork;

        protected CreateNewLotCommandBase(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            LotUnitOfWork = lotUnitOfWork;
        }

        protected IResult<Lot> CreateLot(CreateNewLotCommandParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            parameters.LotDate = parameters.LotDate.ToLocalTime().Date; // ensure LotDate is always created in local time

            int newLotSequence;
            if(parameters.LotSequence != null)
            {
                var lot = LotUnitOfWork.LotRepository.Filter(l => l.LotDateCreated == parameters.LotDate && l.LotDateSequence == parameters.LotSequence && l.LotTypeId == (int)parameters.LotType).FirstOrDefault();
                if(lot != null)
                {
                    return new InvalidResult<Lot>(null, string.Format(UserMessages.LotExistsWithKey, lot.ToLotKey()));
                }

                newLotSequence = (int) parameters.LotSequence;
            }
            else
            {
                newLotSequence = new EFUnitOfWorkHelper(LotUnitOfWork).GetNextSequence(LotPredicates.FilterByLotDateAndTypeId(parameters.LotDate, parameters.LotType), l => l.LotDateSequence);
            }

            if(newLotSequence < 1)
            {
                return new InvalidResult<Lot>(null, string.Format(UserMessages.LotDateSequenceLessThanOne));
            }

            var packagingKey = parameters.PackagingReceivedKey;
            if(packagingKey == null)
            {
                packagingKey = LotUnitOfWork.PackagingProductRepository.Filter(p => p.Weight <= 0.0, p => p.Product).ToList().FirstOrDefault(p => p.Product.Name.Replace(" ", "").ToUpper() == "NOPACKAGING");
                if(packagingKey == null)
                {
                    return new InvalidResult<Lot>(null, UserMessages.NoPackagingNotFound);
                }
            }

            var newLot = LotUnitOfWork.LotRepository.Add(new Lot
                {
                    EmployeeId = parameters.EmployeeKey.EmployeeKey_Id,
                    TimeStamp = parameters.TimeStamp,

                    LotDateCreated = parameters.LotDate.Date,
                    LotDateSequence = newLotSequence,
                    LotTypeEnum = parameters.LotType,

                    ReceivedPackagingProductId = packagingKey.PackagingProductKey_ProductId,

                    QualityStatus = parameters.LotQualityStatus,
                    ProductionStatus = parameters.LotProductionStatus,
                    ProductSpecComplete = parameters.ProductSpecComplete,
                    ProductSpecOutOfRange = parameters.ProductSpecOutOfRange,

                    VendorId = parameters.VendorKey == null ? (int?) null : parameters.VendorKey.CompanyKey_Id,
                    PurchaseOrderNumber = parameters.PurchaseOrderNumber,
                    ShipperNumber = parameters.ShipperNumber
                });

            return new SuccessResult<Lot>(newLot);
        }
    }
}