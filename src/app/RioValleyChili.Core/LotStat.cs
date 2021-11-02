using System;
using System.Linq;

namespace RioValleyChili.Core
{
    public enum LotStat
    {
        InProcess = 0,
        Completed = 1,
        ReBlend = 2,
        See_Desc = 3,
        Rework = 4,
        Completed_Hold = 5,
        RePack = 6,
        Dark_Specs = 7,
        Smoke_Cont = 8,
        Hard_BBs = 9,
        Soft_BBs = 10,
        Granulation = 11,
        High_Water = 12,
        Low_Water = 13,
        Scan = 14,
        Asta = 15,
        A_B = 16,
        Scov = 17,
        InProcess_Hold = 18,
        Contaminated = 19,
        _09Hold = 20,
        TBT = 21,
        Rejected = 22
    }

    public static class LotStatHelper
    {
        public static LotStat? GetLotStat(int? lotStat)
        {
            if(lotStat == null)
            {
                return null;
            }

            switch(lotStat)
            {
                case 0:		return LotStat.InProcess;
                case 1:		return LotStat.Completed;
                case 2:		return LotStat.ReBlend;
                case 3:		return LotStat.See_Desc;
                case 4:		return LotStat.Rework;
                case 5:		return LotStat.Completed_Hold;
                case 6:		return LotStat.RePack;
                case 7:		return LotStat.Dark_Specs;
                case 8:		return LotStat.Smoke_Cont;
                case 9:		return LotStat.Hard_BBs;
                case 10:	return LotStat.Soft_BBs;
                case 11:	return LotStat.Granulation;
                case 12:	return LotStat.High_Water;
                case 13:	return LotStat.Low_Water;
                case 14:	return LotStat.Scan;
                case 15:	return LotStat.Asta;
                case 16:	return LotStat.A_B;
                case 17:	return LotStat.Scov;
                case 18:	return LotStat.InProcess_Hold;
                case 19:	return LotStat.Contaminated;
                case 20:	return LotStat._09Hold;
                case 21:	return LotStat.TBT;
                case 22:    return LotStat.Rejected;

                default: throw new ArgumentOutOfRangeException("lotStat");
            }
        }

        public static string GetLotStatText(this LotStat? lotStat)
        {
            switch(lotStat)
            {
                case null: return null;
                case LotStat.InProcess: return "InProcess";
                case LotStat.Completed: return "Completed";
                case LotStat.ReBlend: return "ReBlend";
                case LotStat.See_Desc: return "See Desc";
                case LotStat.Rework: return "Rework";
                case LotStat.Completed_Hold: return "Completed Hold";
                case LotStat.RePack: return "RePack";
                case LotStat.Dark_Specs: return "Dark Specs";
                case LotStat.Smoke_Cont: return "Smoke Cont.";
                case LotStat.Hard_BBs: return "Hard BB's";
                case LotStat.Soft_BBs: return "Soft BB's";
                case LotStat.Granulation: return "Granulation";
                case LotStat.High_Water: return "High Water";
                case LotStat.Low_Water: return "Low Water";
                case LotStat.Scan: return "Scan";
                case LotStat.Asta: return "Asta";
                case LotStat.A_B: return "A/B";
                case LotStat.Scov: return "Scov";
                case LotStat.InProcess_Hold: return "InProcess Hold";
                case LotStat.Contaminated: return "Contaminated";
                case LotStat._09Hold: return "09Hold";
                case LotStat.TBT: return "TBT";
                case LotStat.Rejected: return "Rejected";

                default: throw new ArgumentOutOfRangeException("lotStat");
            }
        }

        public static int? GetLotStat(LotStat? lotStat)
        {
            if(lotStat == null)
            {
                return null;
            }

            return (int) lotStat.Value;
        }

        public static bool IsAny(this LotStat? lotStat, params LotStat[] statuses)
        {
            return statuses.Any(s => s == lotStat);
        }

        public static bool IsCompleted(this LotStat? lotStat)
        {
            return lotStat == LotStat.Completed || lotStat == LotStat.Completed_Hold;
        }

        public static bool IsProductSpec(this LotStat? lotStat)
        {
            switch(lotStat)
            {
                case LotStat.Granulation:
                case LotStat.High_Water:
                case LotStat.Low_Water:
                case LotStat.Scan:
                case LotStat.Asta:
                case LotStat.A_B:
                case LotStat.Scov:
                    return true;
            }

            return false;
        }

        public static bool IsInHouseContamination(this LotStat? lotStat)
        {
            switch(lotStat)
            {
                case LotStat.Dark_Specs:
                case LotStat.Smoke_Cont:
                case LotStat.Hard_BBs:
                case LotStat.Soft_BBs:
                    return true;
            }

            return false;
        }

        public static bool IsAcceptable(this LotStat? lotStat)
        {
            if(lotStat.IsCompleted() || lotStat.IsProductSpec() || lotStat.IsInHouseContamination())
            {
                return true;
            }
            
            switch(lotStat)
            {
                case LotStat.Rework:
                case LotStat.ReBlend:
                case LotStat.RePack:
                case LotStat.TBT:
                case LotStat.See_Desc:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsRequiresAttention(this LotStat? lotStat)
        {
            return lotStat == LotStat._09Hold || lotStat == LotStat.InProcess_Hold;
        }
    }
}