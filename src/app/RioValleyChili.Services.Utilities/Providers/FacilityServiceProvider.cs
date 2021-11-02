using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Business.Core.Helpers;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Interfaces;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.OldContextSynchronization.Parameters;
using RioValleyChili.Services.OldContextSynchronization.Synchronize;
using RioValleyChili.Services.Utilities.Commands;
using RioValleyChili.Services.Utilities.Commands.Facilities;
using RioValleyChili.Services.Utilities.Extensions.Parameters;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.OldContextSynchronization;
using Solutionhead.Core;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Providers
{
    public class FacilityServiceProvider : IUnitOfWorkContainer<IFacilityUnitOfWork>
    {
        IFacilityUnitOfWork IUnitOfWorkContainer<IFacilityUnitOfWork>.UnitOfWork { get { return _facilityUnitOfWork; } }

        private readonly IFacilityUnitOfWork _facilityUnitOfWork;
        private readonly ITimeStamper _timeStamper;

        public FacilityServiceProvider(IFacilityUnitOfWork facilityUnitOfWork, ITimeStamper timeStamper)
        {
            if(facilityUnitOfWork == null) { throw new ArgumentNullException("facilityUnitOfWork"); }
            if(timeStamper == null) { throw new ArgumentNullException("timeStamper"); }

            _facilityUnitOfWork = facilityUnitOfWork;
            _timeStamper = timeStamper;
        }

        public IResult<IQueryable<ILocationReturn>> GetRinconLocations()
        {
            var result = new GetFacilitiesCommand(_facilityUnitOfWork).Execute(GlobalKeyHelpers.RinconFacilityKey.ToFacilityKey(), true, true);

            if(!result.Success)
            {
                return new FailureResult<IQueryable<ILocationReturn>>(null, result.Message);
            }

            var locations = result.ResultingObject
                .SelectMany(f => f.Locations).ToList()
                .OrderBy(l => l.FacilityName).ThenBy(l => l.Description);

            return new SuccessResult<IQueryable<ILocationReturn>>(locations.AsQueryable());
        }

        public IResult<IQueryable<IFacilityDetailReturn>> GetFacilities(string facilityKeyValue, bool includeLocations, bool includeAddress)
        {
            FacilityKey facilityKey = null;
            if(!string.IsNullOrEmpty(facilityKeyValue))
            {
                var keyResult = KeyParserHelper.ParseResult<IFacilityKey>(facilityKeyValue);
                if(!keyResult.Success)
                {
                    return keyResult.ConvertTo<IQueryable<IFacilityDetailReturn>>();
                }
                facilityKey = keyResult.ResultingObject.ToFacilityKey();
            }

            return new GetFacilitiesCommand(_facilityUnitOfWork).Execute(facilityKey, includeLocations, includeAddress);
        }

        [SynchronizeOldContext(NewContextMethod.SyncFacility)]
        public IResult<string> CreateFacility(ICreateFacilityParameters parameters)
        {
            var facilityResult = new CreateFacilityCommand(_facilityUnitOfWork).CreateFacility(parameters);
            if(!facilityResult.Success)
            {
                return facilityResult.ConvertTo<string>();
            }

            _facilityUnitOfWork.Commit();

            var facilityKey = facilityResult.ResultingObject.ToFacilityKey();
            return SyncParameters.Using(new SuccessResult<string>(facilityKey), new SyncFacilityParameters
                {
                    FacilityKey = facilityKey
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncFacility)]
        public IResult UpdateFacility(IUpdateFacilityParameters parameters)
        {
            var key = KeyParserHelper.ParseResult<IFacilityKey>(parameters.FacilityKey);
            if(!key.Success)
            {
                return key;
            }

            var facilityResult = new UpdateFacilityCommand(_facilityUnitOfWork).UpdateFacility(key.ResultingObject, parameters);
            if(!facilityResult.Success)
            {
                return facilityResult.ConvertTo<string>();
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncFacilityParameters
                {
                    FacilityKey = key.ResultingObject
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncFacility)]
        public IResult DeleteFacility(string facilityKey)
        {
            var key = KeyParserHelper.ParseResult<IFacilityKey>(facilityKey);
            if(!key.Success)
            {
                return key;
            }

            var facilityResult = new DeleteFacilityCommand(_facilityUnitOfWork).DeleteFacility(key.ResultingObject);
            if(!facilityResult.Success)
            {
                return facilityResult;
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncFacilityParameters
                {
                    DeleteWHID = facilityResult.ResultingObject.WHID
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLocations)]
        public IResult<string> CreateLocation(ICreateLocationParameters parameters)
        {
            var parameterResult = parameters.ToParsedParameters();
            if(!parameterResult.Success)
            {
                return parameterResult.ConvertTo<string>();
            }

            var employeeResult = new GetEmployeeCommand(_facilityUnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<string>();
            }

            var result = new CreateLocationCommand(_facilityUnitOfWork).CreateLocation(parameterResult.ResultingObject);
            if(!result.Success)
            {
                return result.ConvertTo<string>();
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult<string>(new LocationKey(result.ResultingObject)), new SyncLocationsParameters
                {
                    EmployeeKey = employeeResult.ResultingObject,
                    Locations = new List<ILocationKey>
                        {
                            result.ResultingObject
                        }
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLocations)]
        public IResult UpdateLocation(IUpdateLocationParameters parameters)
        {
            var parameterResult = parameters.ToParsedParameters();
            if(!parameterResult.Success)
            {
                return parameterResult;
            }

            var employeeResult = new GetEmployeeCommand(_facilityUnitOfWork).GetEmployee(parameters);
            if(!employeeResult.Success)
            {
                return employeeResult.ConvertTo<string>();
            }

            var result = new UpdateLocationCommand(_facilityUnitOfWork).UpdateLocation(parameterResult.ResultingObject);
            if(!result.Success)
            {
                return result;
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncLocationsParameters
                {
                    EmployeeKey = employeeResult.ResultingObject,
                    Locations = new List<ILocationKey>
                            {
                                parameterResult.ResultingObject.LocationKey
                            }
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLocations)]
        public IResult LockLocations(IEnumerable<string> locationKeys)
        {
            var parsed = locationKeys.ToParsedParameters();

            var result = new LockLocationsCommand(_facilityUnitOfWork).Execute(parsed.ResultingObject, true);
            if(!result.Success)
            {
                return result;
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncLocationsParameters
                {
                    Locations = parsed.ResultingObject.Cast<ILocationKey>().ToList()
                });
        }

        [SynchronizeOldContext(NewContextMethod.SyncLocations)]
        public IResult UnlockLocations(IEnumerable<string> locationKeys)
        {
            var parsed = locationKeys.ToParsedParameters();

            var result = new LockLocationsCommand(_facilityUnitOfWork).Execute(parsed.ResultingObject, false);
            if(!result.Success)
            {
                return result;
            }

            _facilityUnitOfWork.Commit();

            return SyncParameters.Using(new SuccessResult(), new SyncLocationsParameters
                {
                    Locations = parsed.ResultingObject.Cast<ILocationKey>().ToList()
                });
        }
    }
}