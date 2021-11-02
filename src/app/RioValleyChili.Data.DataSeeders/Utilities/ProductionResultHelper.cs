using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public class ProductionResultHelper
    {
        private readonly NewContextHelper _newContextHelper;
        private readonly SerializedData _serializedData;

        public ProductionResultHelper(NewContextHelper newContextHelper, SerializedData serializedData)
        {
            _newContextHelper = newContextHelper;
            _serializedData = serializedData;
        }

        public enum CreateNewProductionResultResult
        {
            Success,
            OutputLotIsNull,
            OutputLotBatchStatIDIsNull,
            OutputLotBatchStatNotProduced,
            OutputLotBatchBegTimeIsNull,
            OutputLotBatchEndTimeIsNull,
            OutputLotEntryDateIsNull,
            OutputLotProductionLineIsNull,
            OutputLotProductionLineLocationNotFound
        }

        public CreateNewProductionResultResult CreateNewProductionResult(IPackScheduleKey packScheduleKey, ILotKey lotKey, ProductionBatchEntityObjectMother.tblLotResultDTO outputLot, out LotProductionResults productionResult)
        {
            productionResult = null;

            if(outputLot == null)
            {
                return CreateNewProductionResultResult.OutputLotIsNull;
            }

            if(outputLot.BatchStatID == null)
            {
                return CreateNewProductionResultResult.OutputLotBatchStatIDIsNull;
            }

            if(BatchStatIDHelper.GetBatchStatID(outputLot.BatchStatID.Value) != BatchStatID.Produced)
            {
                return CreateNewProductionResultResult.OutputLotBatchStatNotProduced;
            }

            if(outputLot.EntryDate == null)
            {
                return CreateNewProductionResultResult.OutputLotEntryDateIsNull;
            }
            var timeStamp = outputLot.EntryDate.Value.ConvertLocalToUTC();

            var beginTime = ((outputLot.BatchBegTime ?? outputLot.ProductionDate).Value).ConvertLocalToUTC();
            var endTime = ((outputLot.BatchEndTime ?? outputLot.ProductionDate).Value).ConvertLocalToUTC();
            var productionLine = outputLot.ProductionLine ?? 1;

            var lineLocation = _newContextHelper.GetProductionLine(productionLine);
            if(lineLocation == null)
            {
                return CreateNewProductionResultResult.OutputLotProductionLineLocationNotFound;
            }

            var deserializedResults = _serializedData.GetDeserialized<SerializableEmployeeIdentifiable>(SerializableType.LotProductionResults, outputLot.Lot.ToString());

            productionResult = new LotProductionResults
                {
                    EmployeeId = deserializedResults == null ? outputLot.EmployeeID.Value : deserializedResults.EmployeeKey.EmployeeKeyId,
                    TimeStamp = deserializedResults == null ? timeStamp : deserializedResults.TimeStamp,

                    LotDateCreated = lotKey.LotKey_DateCreated,
                    LotDateSequence = lotKey.LotKey_DateSequence,
                    LotTypeId = lotKey.LotKey_LotTypeId,

                    ProductionLineLocationId = lineLocation.Id,

                    ShiftKey = outputLot.Shift,
                    DateTimeEntered = timeStamp,
                    ProductionBegin = beginTime,
                    ProductionEnd = endTime
                };

            return CreateNewProductionResultResult.Success;
        }

        public enum CreateNewProductionResultItemResult
        {
            Success,
            PackagingProductNotFound,
            WarehouseLocationNotFound,
            InventoryTreatmentNotFound
        }

        public class ProductionResultItemResult
        {
            public CreateNewProductionResultItemResult Result;
            public ProductionBatchEntityObjectMother.tblIncomingDTO Source;
            public LotProductionResultItem ResultItem;
        }

        public IEnumerable<ProductionResultItemResult> CreateNewProductionResultItems(ILotKey lotKey, ProductionBatchEntityObjectMother.tblLotResultDTO outputLot)
        {
            var sequence = 1;
            var productionResults = outputLot.tblIncomings.Where(i => i.TTypeID == (int?)TransType.Production).ToList();
            foreach(var item in productionResults)
            {
                var result = CreateNewProductionResultItemResult.Success;
                LotProductionResultItem resultItem = null;

                var packagingProduct = _newContextHelper.GetPackagingProduct(item.PkgID);
                if(packagingProduct == null)
                {
                    result = CreateNewProductionResultItemResult.PackagingProductNotFound;
                    goto result;
                }

                var warehouseLocation = _newContextHelper.GetLocation(item.LocID);
                if(warehouseLocation == null)
                {
                    result = CreateNewProductionResultItemResult.WarehouseLocationNotFound;
                    goto result;
                }

                var treatment = _newContextHelper.GetInventoryTreatment(item.TrtmtID);
                if(treatment == null)
                {
                    result = CreateNewProductionResultItemResult.InventoryTreatmentNotFound;
                    goto result;
                }

                resultItem = new LotProductionResultItem
                    {
                        LotDateCreated = lotKey.LotKey_DateCreated,
                        LotDateSequence = lotKey.LotKey_DateSequence,
                        LotTypeId = lotKey.LotKey_LotTypeId,
                        ResultItemSequence = sequence++,

                        PackagingProductId = packagingProduct.Id,
                        LocationId = warehouseLocation.Id,
                        TreatmentId = treatment.Id,
                        Quantity = (int) (item.Quantity ?? 0)
                    };

            result:
                yield return new ProductionResultItemResult
                    {
                        Result = result,
                        Source = item,
                        ResultItem = resultItem
                    };
            }
        }
    }
}