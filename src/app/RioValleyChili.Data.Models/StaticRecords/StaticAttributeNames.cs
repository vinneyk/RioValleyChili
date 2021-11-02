using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.Models.Helpers;
using AttributeKeys = RioValleyChili.Core.Helpers.Constants.ChileAttributeKeys;

namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticAttributeNames
    {
        static StaticAttributeNames()
        {
            AttributeNames = new List<AttributeName>
                {
                    Asta,
                    Scoville, 
                    Scan,
                    H2O,
                    AB,
                    Yeast,
                    InsectParts,
                    ColiForms,
                    TPC,
                    Lead,
                    EColi,
                    Salmonella,
                    RodentHairs,
                    Granularity,
                    Mold,
                    Ash,
                    AIA,
                    Ethox,
                    BI,
                    AToxin,
                    Gluten
                };
        }

        public static List<AttributeName> AttributeNames;

        public static AttributeName Asta = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("Asta", AttributeKeys.Asta), a => a.ActualValueRequired = true);
        public static AttributeName Scoville = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("Scoville", AttributeKeys.Scov), a => a.ActualValueRequired = false);
        public static AttributeName Scan = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("Scan", AttributeKeys.Scan, true, true, true, false, 35), a => a.ActualValueRequired = true);
        public static AttributeName H2O = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("H2O", AttributeKeys.H2O), a => a.ActualValueRequired = true);
        public static AttributeName AB = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("AB", AttributeKeys.AB), a => a.ActualValueRequired = true);
        public static AttributeName Yeast = AttributeNameFactory.CreateAttributeName("Yeast", AttributeKeys.Yeast, true, true, true, false, 100, DefectTypeEnum.ActionableDefect);
        public static AttributeName InsectParts = AttributeNameFactory.CreateAttributeName("Insect Parts", AttributeKeys.InsP, true, true, true, false, null, DefectTypeEnum.ActionableDefect);
        public static AttributeName ColiForms = AttributeNameFactory.CreateAttributeName("Coli Forms", AttributeKeys.ColiF, true, true, true, false, 10, DefectTypeEnum.ActionableDefect);
        public static AttributeName TPC = AttributeNameFactory.CreateAttributeName("Total Platelet Count", AttributeKeys.TPC, true, true, true, false, 50000, DefectTypeEnum.ActionableDefect);
        public static AttributeName Lead = AttributeNameFactory.CreateAttributeName("Lead", AttributeKeys.Lead, true, true, true, false, null, DefectTypeEnum.ActionableDefect);
        public static AttributeName EColi = AttributeNameFactory.CreateAttributeName("EColi", AttributeKeys.EColi, true, true, true, false, 3, DefectTypeEnum.BacterialContamination);
        public static AttributeName Salmonella = AttributeNameFactory.CreateAttributeName("Salmonella", AttributeKeys.Sal, true, true, true, false, 0, DefectTypeEnum.BacterialContamination);
        public static AttributeName RodentHairs = AttributeNameFactory.CreateAttributeName("Rodent Hairs", AttributeKeys.RodHrs, true, true, true, false, null, DefectTypeEnum.ActionableDefect);
        public static AttributeName Granularity = AttributeNameFactory.Init(AttributeNameFactory.CreateAttributeName("Granularity", AttributeKeys.Gran), a => a.ActualValueRequired = true);
        public static AttributeName Mold = AttributeNameFactory.CreateAttributeName("Mold", AttributeKeys.Mold, true, true, true, false, 100, DefectTypeEnum.ActionableDefect);
        public static AttributeName Ash = AttributeNameFactory.CreateAttributeName("Ash", AttributeKeys.Ash);
        public static AttributeName AIA = AttributeNameFactory.CreateAttributeName("AIA", AttributeKeys.AIA);
        public static AttributeName Ethox = AttributeNameFactory.CreateAttributeName("Ethoxyquin", AttributeKeys.Ethox);
        public static AttributeName BI = AttributeNameFactory.CreateAttributeName("Bulk Index", AttributeKeys.BI);
        public static AttributeName AToxin = AttributeNameFactory.CreateAttributeName("AToxin", AttributeKeys.AToxin, true, true, true, false, null, DefectTypeEnum.BacterialContamination);
        public static AttributeName Gluten = AttributeNameFactory.CreateAttributeName("Gluten", AttributeKeys.Gluten, true, true, true, false, null, DefectTypeEnum.ActionableDefect);

        public static List<AttributeName> GetAttributesWithAssociatedLotStats()
        {
            return new[]
                {
                    Granularity,
                    H2O,
                    Scan,
                    Asta,
                    AB,
                    Scoville
                }.ToList();
        }
    }
}
