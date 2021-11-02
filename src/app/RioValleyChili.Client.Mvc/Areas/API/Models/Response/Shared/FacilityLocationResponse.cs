using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;

namespace RioValleyChili.Client.Mvc.Areas.API.Models.Response.Shared
{
    public class FacilityLocationResponse
    {
        public string LocationKey { get; set; }
        public string GroupName { get; private set; }
        public int Row { get; private set; }

        public string FacilityKey { get; set; }
        public string FacilityName { get; set; }
        /// <summary>
        /// Note that this refers to the Location's Facility Active state, not the Active state of the Location itself (for that, expect the Status property to be "InActive" or "Available") - RI 2016-12-5
        /// </summary>
        public bool Active { get; set; }
        
        public LocationStatus? Status { get; set; }
        
        public string Description 
        {
            get { return _description; }
            set
            {
                string street;
                int row;
                _description = LocationDescriptionHelper.GetStreetRow(value, out street, out row)
                    ? string.Format("{0}{1:00}", street, row)
                    : value;

                GroupName = street;
                Row = row;
            } 
        }
        private string _description;
    }
}