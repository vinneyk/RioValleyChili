using System;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared
{
    [Obsolete("Use FacilityLocation instead.")]
    public class WarehouseLocationResponse 
    {
        public string WarehouseLocationKey { get; set; }

        public string LocationName { get; set; }
        
        public LocationStatus? Status { get; set; }

        public string WarehouseKey { get; set; }

        public string WarehouseName { get; set; }
        /// <summary>
        /// Note that this refers to the Location's Facility Active state, not the Active state of the Location itself (for that, expect the Status property to be "InActive" or "Available") - RI 2016-12-5
        /// </summary>
        public bool Active { get; set; }
    }
}