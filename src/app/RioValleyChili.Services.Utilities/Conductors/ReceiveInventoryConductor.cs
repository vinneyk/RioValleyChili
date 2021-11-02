using System;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class ReceiveInventoryConductor
    {
        private readonly IInventoryUnitOfWork _inventoryUnitOfWork;

        internal ReceiveInventoryConductor(IInventoryUnitOfWork inventoryUnitOfWork)
        {
            if(inventoryUnitOfWork == null) { throw new ArgumentNullException("inventoryUnitOfWork"); }
            _inventoryUnitOfWork = inventoryUnitOfWork;
        }

        internal IResult<Lot> ReceiveInventory(DateTime timeStamp, ReceiveInventoryParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }
            if(timeStamp == null) { throw new ArgumentNullException("timeStamp"); }

            var employeeResult = new GetEmployeeCommand(_inventoryUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<Lot>();
            }

            IResult<Lot> lotResult;
            var productType = parameters.Parameters.LotType.ToProductType();
            switch(productType)
            {
                case ProductTypeEnum.Additive:  lotResult = CreateAdditiveLot (timeStamp, employeeResult.ResultingObject, parameters); break;
                case ProductTypeEnum.Chile:     lotResult = CreateChileLot    (timeStamp, employeeResult.ResultingObject, parameters); break;
                case ProductTypeEnum.Packaging: lotResult = CreatePackagingLot(timeStamp, employeeResult.ResultingObject, parameters); break;
                default: throw new ArgumentOutOfRangeException(productType.ToString());
            }

            if(!lotResult.Success)
            {
                return lotResult;
            }

            var inventoryResult = new ModifyInventoryCommand(_inventoryUnitOfWork).Execute(parameters.Items.Select(i => i.ToModifyInventory(lotResult.ResultingObject)),
                new InventoryTransactionParameters(employeeResult.ResultingObject, timeStamp, InventoryTransactionType.ReceiveInventory, null));
            if(!inventoryResult.Success)
            {
                return inventoryResult.ConvertTo<Lot>();
            }

            return new SuccessResult<Lot>(lotResult.ResultingObject);
        }

        private IResult<Lot> CreateAdditiveLot(DateTime timeStamp, IEmployeeKey employeeKey, ReceiveInventoryParameters parameters)
        {
            var additiveProductKey = AdditiveProductKey.FromProductKey(parameters.ProductKey);
            var additiveProduct = _inventoryUnitOfWork.AdditiveProductRepository.FindByKey(additiveProductKey);
            if(additiveProduct == null)
            {
                return new InvalidResult<Lot>(null, string.Format(UserMessages.AdditiveProductNotFound, additiveProductKey.KeyValue));
            }

            var additiveLotResult = new CreateNewAdditiveLotCommand(_inventoryUnitOfWork).Execute(new CreateNewAdditiveLotCommandParameters
                {
                    EmployeeKey = employeeKey,
                    TimeStamp = timeStamp,

                    AdditiveProductKey = additiveProduct,

                    LotType = parameters.Parameters.LotType,
                    LotDate = parameters.Parameters.LotDate?.Date ?? timeStamp,
                    LotSequence = parameters.Parameters.LotSequence,

                    PackagingReceivedKey = parameters.PackagingReceivedKey,

                    VendorKey = parameters.VendorKey,
                    PurchaseOrderNumber = parameters.Parameters.PurchaseOrderNumber,
                    ShipperNumber = parameters.Parameters.ShipperNumber
                });

            return !additiveLotResult.Success ? additiveLotResult.ConvertTo<Lot>() : new SuccessResult<Lot>(additiveLotResult.ResultingObject.Lot);
        }

        private IResult<Lot> CreateChileLot(DateTime timeStamp, IEmployeeKey employeeKey, ReceiveInventoryParameters parameters)
        {
            var chileProductKey = ChileProductKey.FromProductKey(parameters.ProductKey);
            var chileProduct = _inventoryUnitOfWork.ChileProductRepository.FindByKey(chileProductKey);
            if(chileProduct == null)
            {
                return new InvalidResult<Lot>(null, string.Format(UserMessages.ChileProductNotFound, chileProductKey.KeyValue));
            }

            var chileLotResult = new CreateNewChileLotCommand(_inventoryUnitOfWork).Execute(new CreateNewChileLotCommandParameters
                {
                    EmployeeKey = employeeKey,
                    TimeStamp = timeStamp,

                    ChileProductKey = chileProduct,

                    LotType = parameters.Parameters.LotType,
                    LotDate = parameters.Parameters.LotDate?.Date ?? timeStamp,
                    LotSequence = parameters.Parameters.LotSequence,

                    PackagingReceivedKey = parameters.PackagingReceivedKey,

                    SetLotProductionStatus = LotProductionStatus.Produced,
                    SetLotQualityStatus = LotQualityStatus.Released,

                    VendorKey = parameters.VendorKey,
                    PurchaseOrderNumber = parameters.Parameters.PurchaseOrderNumber,
                    ShipperNumber = parameters.Parameters.ShipperNumber
                });

            return !chileLotResult.Success ? chileLotResult.ConvertTo<Lot>() : new SuccessResult<Lot>(chileLotResult.ResultingObject.Lot);
        }

        private IResult<Lot> CreatePackagingLot(DateTime timeStamp, IEmployeeKey employeeKey, ReceiveInventoryParameters parameters)
        {
            var packagingProductKey = PackagingProductKey.FromProductKey(parameters.ProductKey);
            var packagingProduct = _inventoryUnitOfWork.PackagingProductRepository.FindByKey(packagingProductKey);
            if(packagingProduct == null)
            {
                return new InvalidResult<Lot>(null, string.Format(UserMessages.PackagingProductNotFound, packagingProductKey.KeyValue));
            }

            var packagingLotResult = new CreateNewPackagingLotCommand(_inventoryUnitOfWork).Execute(new CreateNewPackagingLotCommandParameters
                {
                    EmployeeKey = employeeKey,
                    TimeStamp = timeStamp,

                    PackagingProductKey = packagingProduct,
                    PackagingReceivedKey = parameters.PackagingReceivedKey,

                    LotType = parameters.Parameters.LotType,
                    LotDate = parameters.Parameters.LotDate?.Date ?? timeStamp,
                    LotSequence = parameters.Parameters.LotSequence,

                    VendorKey = parameters.VendorKey,
                    PurchaseOrderNumber = parameters.Parameters.PurchaseOrderNumber,
                    ShipperNumber = parameters.Parameters.ShipperNumber
                });

            return !packagingLotResult.Success ? packagingLotResult.ConvertTo<Lot>() : new SuccessResult<Lot>(packagingLotResult.ResultingObject.Lot);
        }
    }
}