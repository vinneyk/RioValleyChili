using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.Helpers;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Conductors;
using Solutionhead.Data;

namespace LotAttributeFix
{
    public class ProcessLots : IDisposable
    {
        public static void Process(DateTime startDate)
        {
            using(var instance = new ProcessLots())
            {
                instance.ProcessInstance(startDate.Date);
            }
        }

        private RioAccessSQLEntities _oldContext;
        private RioValleyChiliDataContext _newContext;

        private ProcessLots()
        {
            ResetContexts();
        }

        private void ProcessInstance(DateTime startDate)
        {
            var totalLots = 0;
            var newContextSuccess = 0;
            var oldContextSuccess = 0;
            var stopwatch = Stopwatch.StartNew();
            foreach(var lotKey in GetLotKeysToProcess(startDate))
            {
                try
                {
                    totalLots++;
                    Console.WriteLine("Processing lot: {0}", lotKey);

                    var newLotAttributes = UpdateNewLotAttributes(lotKey);
                    if(newLotAttributes == null)
                    {
                        continue;
                    }

                    _newContext.SaveChanges();
                    newContextSuccess++;

                    new SetLotAttributes(_oldContext).UpdateOldLotAttributes(lotKey, newLotAttributes);
                    _oldContext.SaveChanges();
                    oldContextSuccess++;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error processing lot[{0}]: {1}", lotKey, string.IsNullOrWhiteSpace(ex.Message) ? "[No message]" : ex.Message);
                    ResetContexts();
                }
            }

            if(totalLots == 0)
            {
                Console.WriteLine("No Lots found to process.");
            }
            else
            {
                Console.WriteLine("New context lots processed: {0}/{1} = {2}%", newContextSuccess, totalLots, (newContextSuccess * 100.0) / totalLots);
                Console.WriteLine("Old context lots processed: {0}/{1} = {2}%", oldContextSuccess, totalLots, (oldContextSuccess * 100.0) / totalLots);
            }

            stopwatch.Stop();
            Console.WriteLine("Elapsed time: {0:c}", stopwatch.Elapsed);
        }

        private IEnumerable<LotKey> GetLotKeysToProcess(DateTime startDate)
        {
            return _newContext.ProductionBatches
                              .Where(b => b.LotDateCreated >= startDate)
                              .ToList()
                              .Select(b => new LotKey(b))
                              .ToList();
        }

        private List<LotAttribute> UpdateNewLotAttributes(LotKey lotKey)
        {
            var predicate = ((IKey<ProductionBatch>) lotKey).FindByPredicate;
            var batch = _newContext.ProductionBatches
                .Include
                    (
                        b => b.Production.ResultingChileLot.Lot.Attributes.Select(a => a.AttributeName),
                        b => b.Production.PickedInventory.Items
                    )
                .FirstOrDefault(predicate);
            if(batch == null)
            {
                throw new Exception(string.Format("Could not find ProductionBatch[{0}]", lotKey.KeyValue));
            }
            if (!batch.Production.PickedInventory.Items.Any())
            {
                Console.WriteLine("No picked items for this lot. Skipping.");
                return null;
            }

            var result = new SetLotWeightedAttributesConductor(new EFRVCUnitOfWork(_newContext)).Execute(batch.Production.ResultingChileLot, batch.Production.PickedInventory.Items.ToList());
            if(!result.Success)
            {
                throw new Exception(result.Message);
            }

            return batch.Production.ResultingChileLot.Lot.Attributes.ToList();
        }

        private void ResetContexts()
        {
            ((IDisposable)this).Dispose();
            _newContext = new RioValleyChiliDataContext();
            _oldContext = new RioAccessSQLEntities();
        }

        void IDisposable.Dispose()
        {
            if(_oldContext != null)
            {
                _oldContext.Dispose();
                _oldContext = null;
            }

            if(_newContext != null)
            {
                _newContext.Dispose();
                _newContext = null;
            }
        }
    }
}