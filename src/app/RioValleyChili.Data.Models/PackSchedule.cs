using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;

namespace RioValleyChili.Data.Models
{
    public class PackSchedule : EmployeeIdentifiableBase, IPackScheduleKey, IPackagingProductKey, IChileProductKey, IWorkTypeKey, ILocationKey, ICustomerKey
    {
        [Key, Column(Order = 0, TypeName = "Date")]
        public virtual DateTime DateCreated { get; set; }
        [Key, Column(Order = 1)]
        public virtual int SequentialNumber { get; set; }

        [Column(TypeName = "Date")]
        public virtual DateTime ScheduledProductionDate { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime? ProductionDeadline { get; set; }

        public virtual int ChileProductId { get; set; }
        public virtual int ProductionLineLocationId { get; set; }
        public virtual int PackagingProductId { get; set; }
        public virtual int WorkTypeId { get; set; }

        [StringLength(Constants.StringLengths.PackScheduleSummaryOfWork)]
        public virtual string SummaryOfWork { get; set; }
        public virtual ProductionBatchTargetParameters DefaultBatchTargetParameters { get; set; }

        [Obsolete("For data load purposes only. -RI 7/24/13")]
        public virtual DateTime PackSchID { get; set; }
        [Obsolete("For referencing old context. - RI 2014/4/7")]
        public virtual int? PSNum { get; set; }

        [Obsolete("Intended for \"temporary\" use while synchronizing with Access system. RI - 2014/04/24")]
        [StringLength(Constants.StringLengths.OrderNumber)]
        public virtual string OrderNumber { get; set; }
        [Obsolete("Intended for \"temporary\" use while synchronizing with Access system. RI - 2014/04/24")]
        public virtual int? CustomerId { get; set; }
        [Obsolete("Intended for \"temporary\" use while synchronizing with Access system. RI - 2014/04/24")]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("ChileProductId")]
        public virtual ChileProduct ChileProduct { get; set; }
        [ForeignKey("PackagingProductId")]
        public virtual PackagingProduct PackagingProduct { get; set; }
        [ForeignKey("WorkTypeId")]
        public virtual WorkType WorkType { get; set; }
        [ForeignKey("ProductionLineLocationId")]
        public virtual Location ProductionLineLocation { get; set; }
        public virtual ICollection<ProductionBatch> ProductionBatches { get; set; }
        public virtual ICollection<ProductionScheduleItem> ScheduledItems { get; set; }

        #region Key Interface Implementations.

        #region Implementation of IPackScheduleKey

        public DateTime PackScheduleKey_DateCreated { get { return DateCreated; } }
        public int PackScheduleKey_DateSequence { get { return SequentialNumber; } }

        #endregion

        #region Implementation of IPackagingProductKey

        public int PackagingProductKey_ProductId { get { return PackagingProductId; } }

        #endregion

        #region Implementation of IChileProductKey

        public int ChileProductKey_ProductId { get { return ChileProductId; } }

        #endregion

        #region Implementation of IWorkTypeKey

        public int WorkTypeKey_WorkTypeId { get { return WorkTypeId; } }

        #endregion

        public int LocationKey_Id { get { return ProductionLineLocationId; } }
        public int CustomerKey_Id { get { return CustomerId.Value; } }

        #endregion
    }
}