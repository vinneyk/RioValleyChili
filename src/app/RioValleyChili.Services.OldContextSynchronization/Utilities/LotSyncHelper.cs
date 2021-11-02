using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Utilities.Interfaces;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class LotSyncHelper
    {
        [Issue("Updated to set TesterID, which would otherwise break historyical data being viewable in Access system. -RI 2016-11-09",
            References = new [] { "RVCADMIN-1369" })]
        public static void SetTestData(tblLot oldLot, Lot newLot)
        {
            var actualAttributes = newLot.Attributes.Where(a => !a.Computed).ToList();
            var testDate = actualAttributes.Select(a => (DateTime?)a.AttributeDate).DefaultIfEmpty(null).Max();
            var testerId = actualAttributes.OrderByDescending(a => a.AttributeDate).Select(a => (int?) a.EmployeeId).FirstOrDefault();

            oldLot.TestDate = testDate == DateTime.MinValue ? null : testDate;
            oldLot.TesterID = testerId;
        }

        public static void SetLotAttributes(tblLot oldLot, List<LotAttribute> newAttributes)
        {
            var tblLotAttributeHistory = oldLot.tblLotAttributeHistory == null ? null : oldLot.tblLotAttributeHistory.Where(h => h.TestDate == null).OrderBy(h => h.ArchiveDate).FirstOrDefault();
            foreach(var oldAttribute in oldLot.GetAttributes(tblLotAttributeHistory))
            {
                var newAttribute = newAttributes.FirstOrDefault(a => oldAttribute.AttributeNameKey.Equals(a));
                var newAttributeIsComputed = newAttribute != null && newAttribute.Computed;
                var newAttributeMustBeActual = oldAttribute.ActualValueRequired || !oldAttribute.Computed;
                if(newAttributeMustBeActual && newAttributeIsComputed)
                {
                    continue;
                }

                oldAttribute.Value = newAttribute == null ? null : (decimal?) newAttribute.AttributeValue;
            }
        }

        public static void SetLotStat(tblLot oldLot, Lot newLot, bool completedOverride, IEnumerable<IAttributeRange> productSpec = null)
        {
            if(completedOverride)
            {
                oldLot.LotStat = (int?) LotStat.Completed;
                return;
            }

            if(newLot.QualityStatus == LotQualityStatus.Contaminated)
            {
                oldLot.LotStat = (int?) LotStat.Contaminated;
                return;
            }

            if(newLot.QualityStatus == LotQualityStatus.Rejected)
            {
                oldLot.LotStat = (int?) LotStat.Rejected;
                return;
            }

            if(SetLotHoldStatus(oldLot, newLot))
            {
                return;
            }

            switch(newLot.QualityStatus)
            {
                case LotQualityStatus.Released:
                    if(SetProductSpecDefectStat(oldLot, newLot.Attributes, productSpec, newLot.AttributeDefects) || SetInHouseContamination(oldLot, newLot))
                    {
                        break;
                    }
                    oldLot.LotStat = (int?) LotStat.Completed;
                    break;

                case LotQualityStatus.Pending:
                    oldLot.LotStat = (int?) LotStat.InProcess;
                    break;

                default: throw new ArgumentOutOfRangeException("LotStatus");
            }
        }

        public static bool SetInHouseContamination(tblLot oldLot, Lot newLot)
        {
            var inHouseResult = GetLotStatHelper.GetInHouseContaminationLotStat(newLot.LotDefects);
            if(inHouseResult != null)
            {
                oldLot.LotStat = (int?) inHouseResult.LotStat;
                if(inHouseResult.LotStat == LotStat.See_Desc)
                {
                    oldLot.Notes = inHouseResult.Description;
                }
                return true;
            }

            return false;
        }

        public static bool SetProductSpecDefectStat(tblLot oldLot, IEnumerable<LotAttribute> lotAttributes, IEnumerable<IAttributeRange> productSpec, IEnumerable<IAttributeDefect> lotAttributeDefects)
        {
            var specDefects = GetProductSpecDefects(lotAttributes, productSpec, lotAttributeDefects);
            var defectLotStat = GetLotStatHelper.GetProductSpecDefectStat(specDefects);

            if(defectLotStat != null)
            {
                oldLot.LotStat = (int?) defectLotStat.LotStat;
                if(defectLotStat.LotStat == LotStat.See_Desc && !string.IsNullOrWhiteSpace(defectLotStat.Description))
                {
                    if(string.IsNullOrWhiteSpace(oldLot.Notes))
                    {
                        oldLot.Notes = defectLotStat.Description;
                    }
                    else if(!oldLot.Notes.ToUpper().Contains(defectLotStat.Description.ToUpper()))
                    {
                        oldLot.Notes = string.Format("{0} / {1}", oldLot.Notes, defectLotStat.LotStat);
                    }
                }

                return true;
            }

            return false;
        }

        public static bool SetLotHoldStatus(tblLot oldLot, Lot newLot)
        {
            var lotStat = GetLotStatHelper.GetHoldLotStat(newLot.Hold, newLot.ProductionStatus);
            if(lotStat != null)
            {
                oldLot.LotStat = (int) lotStat.Value;
                return true;
            }

            return false;
        }

        #region Private Parts

        [Issue("Attributes with resolved defects should not be considered for product spec defects." +
               "Should handle an attribute having multiple defects. -RI 8/1/2016",
            References = new[] { "RVCADMIN-1194", "RVCADMIN-1203" })]
        private static IEnumerable<IAttributeDefect> GetProductSpecDefects(IEnumerable<LotAttribute> lotAttributes, IEnumerable<IAttributeRange> productSpec, IEnumerable<IAttributeDefect> lotAttributeDefects)
        {
            var existingDefects = lotAttributeDefects == null ? new Dictionary<AttributeNameKey, List<IAttributeDefect>>() : lotAttributeDefects
                .Where(d => d.AttributeNameKey_ShortName != null)
                .GroupBy(d => d.ToAttributeNameKey())
                .ToDictionary(g => g.Key, g => g.ToList());

            var unresolvedAttributes = lotAttributes == null ? new Dictionary<string, double>() : lotAttributes.Where(a =>
                    {
                        List<IAttributeDefect> defects;
                        return !existingDefects.TryGetValue(a.ToAttributeNameKey(), out defects) || defects.Any(d => !d.HasResolution);
                    })
                .ToDictionary(a => a.ToAttributeNameKey().KeyValue, a => a.AttributeValue);

            var specDefects = productSpec == null ? new Dictionary<AttributeNameKey, List<IAttributeDefect>>() :
                LotStatusHelper.GetAttributeDefects(unresolvedAttributes, productSpec.ToDictionary(s => s.ToAttributeNameKey().KeyValue))
                .GroupBy(d => d.ToAttributeNameKey())
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach(var unresolvedDefects in existingDefects
                .ToDictionary(g => g.Key, g => g.Value.Where(d =>
                        !d.HasResolution &&
                        (d.DefectType == DefectTypeEnum.ProductSpec || d.DefectType == DefectTypeEnum.ActionableDefect))
                    .ToList())
                .Where(d => d.Value.Any()))
            {
                List<IAttributeDefect> defects;
                if(specDefects.TryGetValue(unresolvedDefects.Key, out defects))
                {
                    defects.AddRange(unresolvedDefects.Value);
                }
                else
                {
                    specDefects.Add(unresolvedDefects.Key, unresolvedDefects.Value);
                }
            }

            return specDefects.SelectMany(d => d.Value);
        }

        #endregion
    }
}