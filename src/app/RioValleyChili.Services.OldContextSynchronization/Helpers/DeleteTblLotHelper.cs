using System;
using System.Linq;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Helpers;

namespace RioValleyChili.Services.OldContextSynchronization.Helpers
{
    public class DeleteTblLotHelper
    {
        private readonly RioAccessSQLEntities _oldContext;
        private readonly SerializedData _serializedData;

        public DeleteTblLotHelper(RioAccessSQLEntities oldContext)
        {
            if(oldContext == null) { throw new ArgumentNullException("oldContext"); }
            _oldContext = oldContext;
            _serializedData = new SerializedData(_oldContext);
        }

        public void DeleteLots(params int[] lotNumbers)
        {
            if(lotNumbers == null || !lotNumbers.Any())
            {
                return;
            }

            var lots = _oldContext.tblLots
                .Include
                (
                    l => l.inputBatchItems,
                    l => l.tblBatchInstr,
                    l => l.tblBOMs,
                    l => l.tblLotAttributeHistory,
                    l => l.tblIncomings,
                    l => l.tblOutgoingInputs
                )
                .Where(l => lotNumbers.Contains(l.Lot))
                .ToList();
            var notFound = lotNumbers.Where(l => lots.All(b => b.Lot != l)).ToList();
            if(notFound.Any())
            {
                throw new Exception(string.Format("Could not find tblLot[{0}]", notFound.First()));
            }

            foreach(var lot in lots)
            {
                DeleteLot(lot);
            }
        }

        public void DeleteLot(tblLot lot)
        {
            _serializedData.Remove(lot.Lot.ToString(), SerializableType.ChileLotProduction, SerializableType.LotProductionResults);

            foreach(var bom in lot.tblBOMs.ToList())
            {
                _oldContext.tblBOMs.DeleteObject(bom);
            }

            foreach(var instruction in lot.tblBatchInstr.ToList())
            {
                _oldContext.tblBatchInstrs.DeleteObject(instruction);
            }

            foreach(var batchItem in lot.inputBatchItems.ToList())
            {
                _oldContext.tblBatchItems.DeleteObject(batchItem);
            }

            foreach(var history in lot.tblLotAttributeHistory.ToList())
            {
                _oldContext.tblLotAttributeHistories.DeleteObject(history);
            }

            foreach(var incoming in lot.tblIncomings.ToList())
            {
                _oldContext.tblIncomings.DeleteObject(incoming);
            }

            foreach(var outgoing in lot.tblOutgoingInputs.ToList())
            {
                _oldContext.tblOutgoings.DeleteObject(outgoing);
            }

            _oldContext.tblLots.DeleteObject(lot);
        }
    }
}