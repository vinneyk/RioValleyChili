using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class ProductionScheduleItem : IProductionScheduleItemKey, IPackScheduleKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime ProductionDate { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ProductionLineLocationId { get; set; }
        [Key, Column(Order = 2)]
        public virtual int Index { get; set; }
        
        public virtual bool FlushBefore { get; set; }
        public virtual bool FlushAfter { get; set; }
        public virtual string FlushBeforeInstructions { get; set; }
        public virtual string FlushAfterInstructions { get; set; }

        public virtual DateTime PackScheduleDateCreated { get; set; }
        public virtual int PackScheduleSequence { get; set; }

        [ForeignKey("ProductionDate, ProductionLineLocationId")]
        public virtual ProductionSchedule ProductionSchedule { get; set; }
        [ForeignKey("PackScheduleDateCreated, PackScheduleSequence")]
        public virtual PackSchedule PackSchedule { get; set; }

        DateTime IPackScheduleKey.PackScheduleKey_DateCreated { get { return PackScheduleDateCreated; } }
        int IPackScheduleKey.PackScheduleKey_DateSequence { get { return PackScheduleSequence; } }

        DateTime IProductionScheduleKey.ProductionScheduleKey_ProductionDate { get { return ProductionDate; } }
        int ILocationKey.LocationKey_Id { get { return ProductionLineLocationId; } }
        int IProductionScheduleItemKey.ProductionScheduleItemKey_Index { get { return Index; } }
    }
}