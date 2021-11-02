using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;

namespace RioValleyChili.Client.Mvc.Models.Inventory
{
    public class LotDefectResolutionReturn : ILotDefectResolutionReturn
    {
        public ResolutionTypeEnum? ResolutionType { get; set; }
        public string Description { get; set; }

        public bool CanDelete
        {
            get
            {
                switch (ResolutionType)
                {
                    case ResolutionTypeEnum.AcceptedByDataLoad:
                    case ResolutionTypeEnum.AcceptedByUser:
                        return true;
                    default: return false;
                }
            }
        }
    }
}