using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ProductionSchedule : EmployeeIdentifiableBase, IProductionScheduleKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime ProductionDate { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ProductionLineLocationId { get; set; }

        [ForeignKey("ProductionLineLocationId")]
        public virtual Location ProductionLineLocation { get; set; }
        public virtual ICollection<ProductionScheduleItem> ScheduledItems { get; set; }

        DateTime IProductionScheduleKey.ProductionScheduleKey_ProductionDate { get { return ProductionDate; } }
        int ILocationKey.LocationKey_Id { get { return ProductionLineLocationId; } }
    }
}