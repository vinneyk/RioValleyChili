using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using Solutionhead.Services;

namespace RioValleyChili.Services.Interfaces
{
    public interface IFacilityService
    {
        IResult<IQueryable<ILocationReturn>> GetRinconLocations();

        IResult<IQueryable<IFacilityDetailReturn>> GetFacilities(string warehouseKeyValue = "", bool includeLocations = false, bool includeAddress = false);
        
        IResult<string> CreateFacility(ICreateFacilityParameters parameters);

        IResult UpdateFacility(IUpdateFacilityParameters parameters);

        IResult DeleteFacility(string facilityKey);

        IResult<string> CreateLocation(ICreateLocationParameters parameters);

        IResult UpdateLocation(IUpdateLocationParameters parameters);

        IResult LockLocations(IEnumerable<string> locationKeys);

        IResult UnlockLocations(IEnumerable<string> locationKeys);
    }
}