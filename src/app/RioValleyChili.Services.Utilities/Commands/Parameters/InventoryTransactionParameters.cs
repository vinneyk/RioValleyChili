using System;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Services.Utilities.Commands.Parameters
{
    public class InventoryTransactionParameters
    {
        public IEmployeeKey EmployeeKey;
        public DateTime TimeStamp;
        public InventoryTransactionType TransactionType;
        public string Description;
        public string SourceReference;
        public ILotKey DestinationLotKey;

        public InventoryTransactionParameters(IEmployeeKey employeeKey, DateTime timeStamp, InventoryTransactionType transactionType, string sourceReference, ILotKey destinationLotKey = null)
        {
            EmployeeKey = employeeKey;
            TimeStamp = timeStamp;
            TransactionType = transactionType;
            Description = InventoryTransactionsHelper.GetDescription(transactionType);
            SourceReference = sourceReference;
            DestinationLotKey = destinationLotKey;
        }
    }
}