using System;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;

namespace RioValleyChili.Services.Models.Parameters
{
    public class CreateProductionScheduleParameters : ICreateProductionScheduleParameters
    {
        public string UserToken { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionLineLocationKey { get; set; }
    }
}