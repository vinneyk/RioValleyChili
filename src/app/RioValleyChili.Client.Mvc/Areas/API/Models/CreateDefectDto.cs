using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.LotService;

namespace RioValleyChili.Client.Mvc.Areas.API.Models
{
    public class CreateDefectDto : ICreateLotDefectParameters
    {
        [Required]
        public string LotKey { get; set; }

        [Required]
        public string Description { get; set; }

        public DefectTypeEnum DefectType { get; set; }

        string IUserIdentifiable.UserToken { get; set; }
    }
}