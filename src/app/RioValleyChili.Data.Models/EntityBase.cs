using System;
using System.ComponentModel.DataAnnotations;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Data.Models.Interfaces;

namespace RioValleyChili.Data.Models
{
    [Obsolete("Use EmployeeIdentifiableBase instead.")]
    public abstract class EntityBase : IOwnerIdentifiable
    {
        [Required, StringLength(Constants.StringLengths.User)]
        public virtual string User { get; set; }

        [Required]
        public virtual DateTime TimeStamp { get; set; }
    }
}