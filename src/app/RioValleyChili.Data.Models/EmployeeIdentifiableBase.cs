using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    public abstract class EmployeeIdentifiableBase : IEmployeeIdentifiableEntity
    {
        public virtual int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        public virtual DateTime TimeStamp { get; set; }

        public int EmployeeKey_Id { get { return EmployeeId; } }
    }
}