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
    internal class LotTraceOutputCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;
        private readonly Expression<Func<Lot, LotOutputTraceSelect>> _selectOutputs;

        internal LotTraceOutputCommand(ILotUnitOfWork lotUnitOfWork)
        {
            _lotUnitOfWork = lotUnitOfWork;
            _selectOutputs = LotProjectors.SelectLotOutputTrace(lotUnitOfWork);
        }

        internal IResult<IEnumerable<ILotOutputTraceReturn>> Execute(ILotKey lotKey)
        {
            var key = lotKey.ToLotKey();
            var outputs = _lotUnitOfWork.LotRepository.FilterByKey(key).Select(_selectOutputs).FirstOrDefault();
            if(outputs == null)
            {
                return new InvalidResult<IEnumerable<ILotOutputTraceReturn>>(null, string.Format(UserMessages.LotNotFound, key));
            }

            var traceResults = new List<ILotOutputTraceReturn>();
            Trace(ref traceResults, outputs);

            return new SuccessResult<IEnumerable<ILotOutputTraceReturn>>(traceResults);
        }

        private void Trace(ref List<ILotOutputTraceReturn> traceResults, LotOutputTraceSelect current, LotOutputTraceSelect parentInput = null, params string[] pathToParent)
        {
            var pathToCurrent = pathToParent.Concat(new[] { current.LotKey.LotKey }).ToArray();

            traceResults.Add(new LotOutputTraceReturn
                {
                    LotPath = pathToCurrent,

                    Inputs = parentInput == null ? new List<LotOutputTraceInputReturn>() : parentInput.ProductionOutput
                        .Where(r => r.DestinationLot.LotKey == current.LotKey.LotKey)
                        .OrderBy(r => r.DestinationLot.LotKey)
                        .SelectMany(r => r.PickedTreatments
                            .OrderBy(t => t)
                            .Select(t => new LotOutputTraceInputReturn
                                {
                                    LotKey = parentInput.LotKey.LotKey,
                                    Treatment = t
                                }))
                        .ToList(),

                    Orders = current.OrderOutput
                        .OrderBy(o => o.MoveNum)
                        .SelectMany(o => o.PickedTreatments
                            .OrderBy(t => t)
                            .Select(t => new LotOutputTraceOrdersReturn
                                {
                                    Treatment = t,
                                    OrderNumber = o.MoveNum,
                                    ShipmentDate = o.ShipmentDate,
                                    CustomerName = o.CustomerName
                                }))
                        .ToList()
                });

            var productionSelect = _lotUnitOfWork.LotRepository
                .Filter(current.ProductionOutput.Select(p => p.DestinationLot.ToLotKey()).Aggregate(PredicateBuilder.False<Lot>(), (c, n) => c.Or(n.FindByPredicate).Expand()))
                .Select(_selectOutputs)
                .ToDictionary(s => s.LotKey.LotKey);
            foreach(var output in current.ProductionOutput)
            {
                Trace(ref traceResults, productionSelect[output.DestinationLot.LotKey], current, pathToCurrent);
            }
        }
    }
}