using System.Runtime.Serialization;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Models.Inventory
{
    [KnownType(typeof(LotAttributeDefect))]
    [KnownType(typeof(LotDefect))]
    public class LotDefect 
    {
        public string LotDefectKey { get; set; }
        public DefectTypeEnum DefectType { get; set; }
        public string Description { get; set; }
        public LotDefectResolutionReturn Resolution { get; set; }
        public AttributeDefect AttributeDefect { get; set; }
        
        public string DefectTypeName
        {
            get
            {
                switch (DefectType)
                {
                    case DefectTypeEnum.ProductSpec: return "Product Out Of Spec";
                    case DefectTypeEnum.InHouseContamination: return "In-house Contamination";
                    case DefectTypeEnum.BacterialContamination: return "Bacterial Contamination";
                    default: return DefectType.ToString();
                }
            }
        }

        public virtual string DefectTitle { get { return DefectTypeName; } }
    }
    
    public class AttributeDefect 
    {
        public string AttributeShortName { get; set; }
        public double OriginalValue { get; set; }
        public double OriginalMinLimit { get; set; }
        public double OriginalMaxLimit { get; set; }
    }

    public class LotAttributeDefect : LotDefect
    {
        public string AttributeShortName { get; set; }
        public double OriginalValue { get; set; }
        public double OriginalMinLimit { get; set; }
        public double OriginalMaxLimit { get; set; }
        
        public override string DefectTitle { get { return string.Format("{0} ({1})", DefectTypeName, AttributeShortName); } }
    }
}