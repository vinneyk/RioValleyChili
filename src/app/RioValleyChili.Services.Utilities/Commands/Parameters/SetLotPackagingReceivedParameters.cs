using RioValleyChili.Business.Core.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    internal class SetLotPackagingReceivedParameters
    {
        internal LotKey LotKey { get; set; }
        internal PackagingProductKey PackagingProductKey { get; set; }
    }
}