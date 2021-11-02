using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Initialize.Logging
{
    public class LotInventoryLogger<TLot> : EntityLoggerBase<LotInventoryEntityObjectMotherBase<TLot>.CallbackParameters, LotInventoryEntityObjectMotherBase<TLot>.CallbackReason>
        where TLot : class, IDerivedLot
    {
        public LotInventoryLogger(string logFilePath, string logSummaryName) : base(logFilePath)
        {
            _logSummaryName = logSummaryName;
        }

        protected override string LogSummaryName { get { return _logSummaryName; } }
        private readonly string _logSummaryName;

        protected override string GetLogMessage(LotInventoryEntityObjectMotherBase<TLot>.CallbackParameters parameters)
        {
            switch(parameters.CallbackReason)
            {
                case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.InvalidLotNumber:
                    return string.Format("LotNumber[{0}] invalid LotNumber.", parameters.InventoryByLot.Key);

                case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.WarehouseLocationNotLoaded:
                    return string.Format("LotNumber[{0}] WarehouseLocation[{1}] not loaded.", parameters.Inventory.Lot, parameters.Inventory.LocID);

                case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.PackagingNotLoaded:
                    return string.Format("LotNumber[{0}] Packaging[{1}] not loaded.", parameters.Inventory.Lot, parameters.Inventory.PkgID);

                case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.QuantityLessThanOne:
                    return string.Format("LotNumber[{0}] WarehouseLocation[{1}] Packaging[{2}] Quantity is less than 1.", parameters.Inventory.Lot, parameters.Inventory.LocID, parameters.Inventory.PkgID);

                case LotInventoryEntityObjectMotherBase<TLot>.CallbackReason.LotNotLoaded:
                    return string.Format("LotNumber[{0}] lot not loaded.", parameters.LotKey);
            }

            return null;
        }
    }
}