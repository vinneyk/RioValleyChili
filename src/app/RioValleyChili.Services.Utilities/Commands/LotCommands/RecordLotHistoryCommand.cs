using System;
using Newtonsoft.Json;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class RecordLotHistoryCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal RecordLotHistoryCommand(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<LotHistory> Execute(Lot lot, DateTime timestamp)
        {
            var history = _lotUnitOfWork.LotHistoryRepository.Add(new LotHistory
                {
                    LotDateCreated = lot.LotDateCreated,
                    LotDateSequence = lot.LotDateSequence,
                    LotTypeId = lot.LotTypeId,
                    Sequence = new EFUnitOfWorkHelper(_lotUnitOfWork).GetNextSequence(LotPredicates.ConstructPredicate<LotHistory>(lot), h => h.Sequence),

                    EmployeeId = lot.EmployeeId,
                    TimeStamp = timestamp,

                    Serialized = JsonConvert.SerializeObject(new SerializedLotHistory(lot))
                });

            return new SuccessResult<LotHistory>(history);
        }
    }
}