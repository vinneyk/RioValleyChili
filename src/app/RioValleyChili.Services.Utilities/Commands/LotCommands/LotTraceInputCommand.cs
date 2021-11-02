using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Returns.LotService;
using RioValleyChili.Services.Utilities.LinqProjectors;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class LotTraceInputCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;
        private readonly Expression<Func<Lot, LotInputTraceSelect>> _selectInputs = LotProjectors.SelectLotInputTrace();

        internal LotTraceInputCommand(ILotUnitOfWork lotUnitOfWork)
        {
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<IEnumerable<ILotInputTraceReturn>> Execute(ILotKey lotKey)
        {
            var key = lotKey.ToLotKey();
            var lotSelect = _lotUnitOfWork.LotRepository.FilterByKey(key).Select(_selectInputs).FirstOrDefault();
            if(lotSelect == null)
            {
                return new InvalidResult<IEnumerable<ILotInputTraceReturn>>(null, string.Format(UserMessages.LotNotFound, key));
            }

            var traceResults = new List<ILotInputTraceReturn>();
            Trace(ref traceResults, lotSelect, lotSelect.LotKey.LotKey);

            return new SuccessResult<IEnumerable<ILotInputTraceReturn>>(traceResults);
        }

        private void Trace(ref List<ILotInputTraceReturn> results, LotInputTraceSelect current, params string[] pathToCurrent)
        {
            var inputs = current.Inputs.ToDictionary(p => p.PickedLotKey.ToLotKey());
            var pickedSelect = _lotUnitOfWork.LotRepository
                .Filter(inputs.Aggregate(PredicateBuilder.False<Lot>(), (c, n) => c.Or(n.Key.FindByPredicate).Expand()))
                .Select(_selectInputs)
                .ToDictionary(s => s.LotKey.LotKey);
            
            foreach(var input in inputs.Select(i => new { stringKey = i.Key.KeyValue, lot = i }).OrderBy(i => i.stringKey))
            {
                var pathToPicked = pathToCurrent.Concat(new[] { input.stringKey }).ToArray();
                results.AddRange(input.lot.Value.PickedTreatments.Select(t => new LotInputTraceReturn
                    {
                        LotPath = pathToPicked,
                        Treatment = t
                    }));

                LotInputTraceSelect pickedLotSelect;
                if(pickedSelect.TryGetValue(input.stringKey, out pickedLotSelect))
                {
                    Trace(ref results, pickedLotSelect, pathToPicked);
                }
            }
        }
    }
}