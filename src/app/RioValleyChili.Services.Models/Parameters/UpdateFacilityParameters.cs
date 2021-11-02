using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class UpdateFacilityParameters : CreateFacilityParameters, IUpdateFacilityParameters
    {
        public string FacilityKey { get; set; }
    }
}