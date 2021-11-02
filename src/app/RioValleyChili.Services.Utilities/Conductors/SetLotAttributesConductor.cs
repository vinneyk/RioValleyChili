using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.LotService;
using RioValleyChili.Services.Models.Parameters;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.LotCommands;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;
using AddLotAttributeParameters = RioValleyChili.Services.Utilities.Commands.Parameters.AddLotAttributeParameters;
using SetLotAttributeParameters = RioValleyChili.Services.Utilities.Commands.Parameters.SetLotAttributeParameters;

namespace RioValleyChili.Services.Utilities.Conductors
{
    internal class SetLotAttributesConductor
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal SetLotAttributesConductor(ILotUnitOfWork lotUnitOfWork)
        {
            if(lotUnitOfWork == null) { throw new ArgumentNullException("lotUnitOfWork"); }
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult<ILotKey> Execute(DateTime timestamp, SetLotAttributeParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ILotKey>();
            }

            var dataResult = GetLotData(parameters.LotKey);
            if(!dataResult.Success)
            {
                return dataResult.ConvertTo<ILotKey>();
            }
            var data = dataResult.ResultingObject.First();

            var attributeNames = _lotUnitOfWork.AttributeNameRepository.Filter(a => true, a => a.ValidTreatments).ToList();
            var setAttributesResult = SetLotAttributes(data, employeeResult.ResultingObject, timestamp, attributeNames, parameters.Attributes);
            if(!setAttributesResult.Success)
            {
                return setAttributesResult.ConvertTo<ILotKey>();
            }

            data.Lot.Notes = parameters.Parameters.Notes;

            return new SuccessResult<ILotKey>(parameters.LotKey);
        }

        internal IResult AddLotAttributes(DateTime timestamp, AddLotAttributeParameters parameters)
        {
            if(parameters == null) { throw new ArgumentNullException("parameters"); }

            var lotKeys = parameters.LotKeys.Where(k => k != null).Distinct().ToList();
            if(!lotKeys.Any())
            {
                return new NoWorkRequiredResult();
            }

            var employeeResult = new GetEmployeeCommand(_lotUnitOfWork).GetEmployee(parameters.Parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<ILotKey>();
            }

            var dataResult = GetLotData(lotKeys.ToArray());
            if(!dataResult.Success)
            {
                return dataResult;
            }

            var attributeNames = _lotUnitOfWork.AttributeNameRepository.Filter(a => true, a => a.ValidTreatments).ToList();

            foreach(var chileLot in dataResult.ResultingObject)
            {
                var setResult = SetLotAttributes(chileLot, timestamp, parameters, employeeResult.ResultingObject, attributeNames);
                if(!setResult.Success)
                {
                    return setResult;
                }
            }

            return new SuccessResult();
        }

        private IResult SetLotAttributes(LotData data, DateTime timestamp, AddLotAttributeParameters parameters,Employee user, List<AttributeName> attributeNames)
        {
            var lotAttributes = parameters.Attributes.ToDictionary(a => a.Key, a => a.Value);
            foreach(var attribute in data.Lot.Attributes)
            {
                var attributeNameKey = attribute.ToAttributeNameKey();
                if(!lotAttributes.ContainsKey(attributeNameKey))
                {
                    lotAttributes.Add(attributeNameKey, new AttributeValueParameters
                        {
                            AttributeInfo = new AttributeInfoParameters
                                {
                                    Value = attribute.AttributeValue,
                                    Date = attribute.AttributeDate
                                }
                        });
                }
            }

            var setAttributesResult = SetLotAttributes(data, user, timestamp, attributeNames, lotAttributes);
            if(!setAttributesResult.Success)
            {
                return setAttributesResult;
            }

            if(parameters.Parameters.OverrideOldContextLotAsCompleted)
            {
                var setStatusResult = new SetLotStatusConductor(_lotUnitOfWork).Execute(timestamp, user, data.Lot.ToLotKey(), LotQualityStatus.Released);
                if(!setStatusResult.Success)
                {
                    return setStatusResult;
                }
            }

            return new SuccessResult();
        }

        private IResult<List<LotData>> GetLotData(params LotKey[] lotKeys)
        {
            var lotPredicate = lotKeys.Aggregate(PredicateBuilder.False<Lot>(), (c, n) => c.Or(n.FindByPredicate)).ExpandAll();
            var lots = _lotUnitOfWork.LotRepository
                .Filter(lotPredicate,
                    l => l.ChileLot.ChileProduct.ProductAttributeRanges.Select(r => r.AttributeName),
                    l => l.Inventory.Select(i => i.Treatment),
                    l => l.Attributes.Select(a => a.AttributeName),
                    l => l.LotDefects.Select(d => d.Resolution),
                    l => l.AttributeDefects.Select(d => d.LotDefect.Resolution))
                .ToList();

            var lotsReference = lots.ToList();
            var missingKeys = lotKeys.Where(k => lotsReference.RemoveAll(k.Equals) < 1).ToList();
            if(missingKeys.Any())
            {
                return new InvalidResult<List<LotData>>(null, string.Format(UserMessages.LotNotFound, missingKeys.First()));
            }

            var pickedPredicate = lotKeys
                .Aggregate(PredicateBuilder.False<PickedInventoryItem>(), (c, n) => c.Or(PickedInventoryItemPredicates.FilterByLotKey(n)))
                .And(i => !i.PickedInventory.Archived).ExpandAll();
            var pickedInventoryItems = _lotUnitOfWork.PickedInventoryItemRepository.Filter(pickedPredicate,
                    i => i.PickedInventory,
                    i => i.Treatment)
                .ToList()
                .GroupBy(i => i.ToLotKey())
                .ToDictionary(g => g.Key, g => g.ToList());

            var results = lots.Select(l =>
                {
                    var lotKey = l.ToLotKey();
                    List<PickedInventoryItem> picked;
                    if(!pickedInventoryItems.TryGetValue(lotKey, out picked))
                    {
                        picked = new List<PickedInventoryItem>();
                    }

                    return new LotData
                        {
                            Lot = l,
                            Picked = picked,
                        };
                }).ToList();

            return new SuccessResult<List<LotData>>(results);
        }

        private IResult SetLotAttributes(LotData data, Employee employee, DateTime timestamp, List<AttributeName> attributeNames, Dictionary<AttributeNameKey, IAttributeValueParameters> attributes)
        {
            var historyResult = new RecordLotHistoryCommand(_lotUnitOfWork).Execute(data.Lot, timestamp);
            if(!historyResult.Success)
            {
                return historyResult;
            }

            var setValuesResult = new SetLotAttributeValuesCommand(_lotUnitOfWork).Execute(new SetLotAttributesParameters
                {
                    Employee = employee,
                    TimeStamp = timestamp,

                    AttributeNames = attributeNames,
                    Lot = data.Lot,
                    LotUnarchivedPickedInventoryItems = data.Picked,
                    LotAttributeDefects = data.Lot.AttributeDefects.ToList(),
                    NewAttributes = attributes
                });
            if(!setValuesResult.Success)
            {
                return setValuesResult;
            }

            if(data.Lot.ChileLot != null)
            {
                var updateStatusResult = LotStatusHelper.UpdateChileLotStatus(data.Lot.ChileLot, attributeNames);
                if(!updateStatusResult.Success)
                {
                    return updateStatusResult;
                }
            }

            data.Lot.EmployeeId = employee.EmployeeId;
            data.Lot.TimeStamp = timestamp;

            return new SuccessResult();
        }

        private class LotData
        {
            public Lot Lot;
            public List<PickedInventoryItem> Picked;
        }
    }
}