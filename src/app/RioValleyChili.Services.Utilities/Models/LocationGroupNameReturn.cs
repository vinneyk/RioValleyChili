using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces;

namespace RioValleyChili.Services.Utilities.Models
{
    internal class LocationGroupNameReturn : ILocationGroupNameReturn
    {
        public string LocationGroupName
        {
            get
            {
                string street;
                int row;
                return LocationDescriptionHelper.GetStreetRow(Description, out street, out row) ? street : Description;
            }
        }

        internal string Description { get; set; }
    }
}