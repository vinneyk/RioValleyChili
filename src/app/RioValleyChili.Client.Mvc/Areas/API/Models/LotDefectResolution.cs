using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class LotDefectResolution : IResolveLotDefectParameters
    {
        public string LotDefectKey { get; set; }

        [Required]
        public ResolutionTypeEnum ResolutionType { get; set; }

        [Required]
        public string Description { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}