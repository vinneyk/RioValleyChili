using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.WarehouseService;
using RioValleyChili.Services.Interfaces.Returns.WarehouseService;
using RioValleyChili.Services.Utilities.Providers;
using Solutionhead.Services;

namespace RioValleyChili.Services
{
    public class FacilityService : IFacilityService
    {
        #region fields and constructors

        private readonly FacilityServiceProvider _facilityServiceProvider;
        private readonly IExceptionLogger _exceptionLogger;

        public FacilityService(FacilityServiceProvider facilityServiceProvider, IExceptionLogger exceptionLogger)
        {
            if(facilityServiceProvider == null) { throw new ArgumentNullException("facilityServiceProvider"); }
            if(exceptionLogger == null) { throw new ArgumentNullException("exceptionLogger"); }

            _facilityServiceProvider = facilityServiceProvider;
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region warehouse methods
        
        public IResult<IQueryable<ILocationReturn>> GetRinconLocations()
        {
            try
            {
                return _facilityServiceProvider.GetRinconLocations();
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<ILocationReturn>>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<IQueryable<IFacilityDetailReturn>> GetFacilities(string facilityKey = "", bool includeLocations = false, bool includeAddress = false)
        {
            try
            {
                return _facilityServiceProvider.GetFacilities(facilityKey, includeLocations, includeAddress);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<IQueryable<IFacilityDetailReturn>>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult<string> CreateFacility(ICreateFacilityParameters parameters)
        {
            try
            {
                return _facilityServiceProvider.CreateFacility(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult UpdateFacility(IUpdateFacilityParameters parameters)
        {
            try
            {
                return _facilityServiceProvider.UpdateFacility(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        public IResult DeleteFacility(string facilityKey)
        {
            try
            {
                return _facilityServiceProvider.DeleteFacility(facilityKey);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        public IResult<string> CreateLocation(ICreateLocationParameters parameters)
        {
            try
            {
                return _facilityServiceProvider.CreateLocation(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult<string>(null, ex.GetInnermostException().Message);
            }
        }

        public IResult UpdateLocation(IUpdateLocationParameters parameters)
        {
            try
            {
                return _facilityServiceProvider.UpdateLocation(parameters);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        public IResult LockLocations(IEnumerable<string> locationKeys)
        {
            try
            {
                return _facilityServiceProvider.LockLocations(locationKeys);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        public IResult UnlockLocations(IEnumerable<string> locationKeys)
        {
            try
            {
                return _facilityServiceProvider.UnlockLocations(locationKeys);
            }
            catch(Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return new FailureResult(ex.GetInnermostException().Message);
            }
        }

        #endregion
    }
}
