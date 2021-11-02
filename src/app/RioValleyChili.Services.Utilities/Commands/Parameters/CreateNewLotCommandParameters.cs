using System;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal abstract class CreateNewLotCommandParameters
    {
        internal IEmployeeKey EmployeeKey { get; set; }
        internal DateTime TimeStamp { get; set; }
        internal DateTime LotDate { get; set; }
        internal LotTypeEnum LotType { get; set; }
        internal int? LotSequence { get; set; }
        internal abstract LotProductionStatus LotProductionStatus { get; }
        internal abstract LotQualityStatus LotQualityStatus { get; }
        internal abstract bool ProductSpecComplete { get; }
        internal abstract bool ProductSpecOutOfRange { get; }
        internal IPackagingProductKey PackagingReceivedKey { get; set; }
        internal ICompanyKey VendorKey { get; set; }
        internal string PurchaseOrderNumber { get; set; }
        internal string ShipperNumber { get; set; }
    }

    internal class CreateNewChileLotCommandParameters : CreateNewLotCommandParameters
    {
        internal IChileProductKey ChileProductKey { get; set; }
        internal override LotProductionStatus LotProductionStatus { get { return SetLotProductionStatus; } }
        internal override LotQualityStatus LotQualityStatus { get { return SetLotQualityStatus; } }
        internal override bool ProductSpecComplete { get { return false; } }
        internal override bool ProductSpecOutOfRange { get { return false; } }
        internal LotProductionStatus SetLotProductionStatus { get; set; }
        internal LotQualityStatus SetLotQualityStatus { get; set; }
    }

    internal class CreateNewAdditiveLotCommandParameters : CreateNewLotCommandParameters
    {
        internal IAdditiveProductKey AdditiveProductKey { get; set; }
        internal override LotProductionStatus LotProductionStatus { get { return LotProductionStatus.Produced; } }
        internal override LotQualityStatus LotQualityStatus { get { return LotQualityStatus.Released; } }
        internal override bool ProductSpecComplete { get { return true; } }
        internal override bool ProductSpecOutOfRange { get { return false; } }
    }

    internal class CreateNewPackagingLotCommandParameters : CreateNewLotCommandParameters
    {
        internal IPackagingProductKey PackagingProductKey { get; set; }
        internal override LotProductionStatus LotProductionStatus { get { return LotProductionStatus.Produced; } }
        internal override LotQualityStatus LotQualityStatus { get { return LotQualityStatus.Released; } }
        internal override bool ProductSpecComplete { get { return true; } }
        internal override bool ProductSpecOutOfRange { get { return false; } }
    }
}