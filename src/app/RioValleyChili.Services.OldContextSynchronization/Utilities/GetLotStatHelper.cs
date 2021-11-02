using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Data.Models.Helpers;
using RioValleyChili.Data.Models.Interfaces;
using RioValleyChili.Data.Models.StaticRecords;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class GetLotStatHelper
    {
        public static LotStatResult GetProductSpecDefectStat(IEnumerable<IAttributeDefect> attributeDefects)
        {
            if(attributeDefects == null)
            {
                return null;
            }

            var mostDefective = attributeDefects.Where(d => !d.HasResolution)
                                                .OrderByDescending(d => d.NormalizedValueDelta() ?? 0.0)
                                                .FirstOrDefault();
            if(mostDefective == null)
            {
                return null;
            }

            var defectKey = mostDefective.ToAttributeNameKey();
            if(defectKey.Equals(StaticAttributeNames.Granularity))
            {
                return new LotStatResult(LotStat.Granulation);
            }
            if(defectKey.Equals(StaticAttributeNames.H2O))
            {
                return new LotStatResult(mostDefective.TooHigh() ? LotStat.High_Water : LotStat.Low_Water);
            }
            if(defectKey.Equals(StaticAttributeNames.Scan))
            {
                return new LotStatResult(LotStat.Scan);
            }
            if(defectKey.Equals(StaticAttributeNames.Asta))
            {
                return new LotStatResult(LotStat.Asta);
            }
            if(defectKey.Equals(StaticAttributeNames.AB))
            {
                return new LotStatResult(LotStat.A_B);
            }
            if(defectKey.Equals(StaticAttributeNames.Scoville))
            {
                return new LotStatResult(LotStat.Scov);
            }

            return new LotStatResult(string.Format("{0} {1}", mostDefective.AttributeNameKey_ShortName, mostDefective.TooHigh() ? "too high" : "too low"));
        }

        public static LotStatResult GetInHouseContaminationLotStat(IEnumerable<LotDefect> lotDefects)
        {
            var inHouseContamination = lotDefects.FirstOrDefault(d => d.DefectType == DefectTypeEnum.InHouseContamination && d.Resolution == null);
            if(inHouseContamination == null)
            {
                return null;
            }

            var description = inHouseContamination.Description.ToUpper();
            var match = InHouseContaminations.FirstOrDefault(c => Regex.Match(description, c.Item2).Success);
            return match != null ? new LotStatResult(match.Item1) : new LotStatResult(inHouseContamination.Description);
        }

        public static LotStat? GetHoldLotStat(LotHoldType? hold, LotProductionStatus productionStatus)
        {
            switch(hold)
            {
                case LotHoldType.HoldForCustomer: return LotStat.Completed_Hold;
                case LotHoldType.HoldForAdditionalTesting: return productionStatus == LotProductionStatus.Produced ? LotStat._09Hold : LotStat.InProcess_Hold;
                case LotHoldType.HoldForTreatment: return LotStat.TBT;
                default: return null;
            }
        }

        private static readonly List<Tuple<LotStat, string>> InHouseContaminations = new List<Tuple<LotStat, string>>
            {
                new Tuple<LotStat, string>(LotStat.Dark_Specs, @".*?DARK.*?SPEC.*?"),
                new Tuple<LotStat, string>(LotStat.Hard_BBs, @".*?HARD.*?BB.*?"),
                new Tuple<LotStat, string>(LotStat.Soft_BBs, @".*?SOFT.*?BB.*?"),
                new Tuple<LotStat, string>(LotStat.Smoke_Cont, @".*?SMOKE.*?CONT.*?")
            };

        public class LotStatResult
        {
            public readonly LotStat? LotStat;
            public readonly string Description;

            public LotStatResult(LotStat lotStat)
            {
                LotStat = lotStat;
            }

            public LotStatResult(string description)
            {
                LotStat = Core.LotStat.See_Desc;
                Description = description;
            }
        }
    }
}