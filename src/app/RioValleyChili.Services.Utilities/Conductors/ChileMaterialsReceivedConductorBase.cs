using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Inventory;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.Extensions.DataModels;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class ChileMaterialsReceivedConductorBase
    {
        protected readonly IMaterialsReceivedUnitOfWork MaterialsReceivedUnitOfWork;

        internal ChileMaterialsReceivedConductorBase(IMaterialsReceivedUnitOfWork materialsReceivedUnitOfWork)
        {
            if(materialsReceivedUnitOfWork == null) { throw new ArgumentNullException("materialsReceivedUnitOfWork"); }
            MaterialsReceivedUnitOfWork = materialsReceivedUnitOfWork;
        }

        protected IResult<ChileMaterialsReceived> Set(ChileMaterialsReceived received, ChileProduct chileProduct, ISetChileMaterialsReceivedParameters parameters, Employee employee, DateTime timeStamp)
        {
            if(received.ChileProductId != chileProduct.Id)
            {
                if(MaterialsReceivedUnitOfWork.PickedInventoryItemRepository.Filter(PickedInventoryItemPredicates.FilterByLotKey(received)).Any())
                {
                    return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.LotHasExistingPickedInventory, received.ToLotKey()));
                }
            }

            var previousTreatment = received.ToInventoryTreatmentKey();
            var treatment = MaterialsReceivedUnitOfWork.InventoryTreatmentRepository.FindByKey(parameters.TreatmentKey);
            if(treatment == null)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.InventoryTreatmentNotFound, parameters.TreatmentKey));
            }

            var supplier = MaterialsReceivedUnitOfWork.CompanyRepository.FindByKey(parameters.SupplierKey, c => c.CompanyTypes);
            if(supplier == null)
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.SupplierCompanyNotFound, parameters.SupplierKey));
            }

            if(parameters.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Dehydrated && supplier.CompanyTypes.All(c => c.CompanyTypeEnum != CompanyType.Dehydrator))
            {
                return new InvalidResult<ChileMaterialsReceived>(null, string.Format(UserMessages.CompanyNotOfType, parameters.SupplierKey, CompanyType.Dehydrator));
            }

            received.EmployeeId = employee.EmployeeId;
            received.TimeStamp = timeStamp;

            received.ChileMaterialsReceivedType = parameters.ChileMaterialsReceivedType;
            received.DateReceived = parameters.DateReceived;
            received.LoadNumber = parameters.LoadNumber;
            received.ChileLot.Lot.PurchaseOrderNumber = parameters.PurchaseOrder;
            received.ChileLot.Lot.ShipperNumber = parameters.ShipperNumber;
            
            received.TreatmentId = treatment.Id;

            received.ChileProductId = chileProduct.Id;
            received.ChileLot.ChileProductId = chileProduct.Id;

            received.Supplier = supplier;
            received.SupplierId = supplier.Id;

            var setItemsResult = SetItems(received, parameters.Items, previousTreatment, employee, timeStamp);
            if(!setItemsResult.Success)
            {
                return setItemsResult.ConvertTo<ChileMaterialsReceived>();
            }

            return new SuccessResult<ChileMaterialsReceived>(received);
        }

        private IResult SetItems(ChileMaterialsReceived received, IEnumerable<SetChileMaterialsReceivedItemParameters> setItems, IInventoryTreatmentKey previousTreatment, IEmployeeKey employee, DateTime timeStamp)
        {
            var maxSequence = received.Items.Select(i => i.ItemSequence).DefaultIfEmpty(0).Max();
            var itemsToRemove = received.Items.ToDictionary(i => i.ToChileMaterialsReceivedItemKey());

            var modifications = itemsToRemove.Values.Select(i => i.ToRemoveInventoryParameters(previousTreatment)).ToList();
            foreach(var setItem in setItems)
            {
                ChileMaterialsReceivedItem item;
                if(setItem.ItemKey != null && itemsToRemove.TryGetValue(setItem.ItemKey, out item))
                {
                    itemsToRemove.Remove(setItem.ItemKey);
                }
                else
                {
                    item = MaterialsReceivedUnitOfWork.ChileMaterialsReceivedItemRepository.Add(new ChileMaterialsReceivedItem
                        {
                            LotDateCreated = received.LotDateCreated,
                            LotDateSequence = received.LotDateSequence,
                            LotTypeId = received.LotTypeId,
                            ItemSequence = ++maxSequence,
                        });
                    received.Items.Add(item);
                }

                item.GrowerCode = setItem.GrowerCode;
                item.ToteKey = setItem.ToteKey;
                item.ChileVariety = setItem.ChileVariety;
                item.Quantity = setItem.Quantity;
                item.LocationId = setItem.LocationKey.LocationKey_Id;
                item.PackagingProductId = setItem.PackagingProductKey.PackagingProductKey_ProductId;
            }

            itemsToRemove.Values.ForEach(MaterialsReceivedUnitOfWork.ChileMaterialsReceivedItemRepository.Remove);
            modifications.AddRange(received.Items.Select(i => i.ToAddInventoryParameters(received)));

            return new ModifyInventoryCommand(MaterialsReceivedUnitOfWork).Execute(modifications,
                new InventoryTransactionParameters(employee, timeStamp,
                    received.ChileMaterialsReceivedType == ChileMaterialsReceivedType.Dehydrated
                    ? InventoryTransactionType.ReceivedDehydratedMaterials
                    : InventoryTransactionType.ReceivedOtherMaterials, received.ToLotKey()));
        }
    }
}