using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models
{
    public class DataLoadResult : IDataLoadResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual bool Success { get; set; }

        public virtual bool RanToCompletion { get; set; }

        [Column(TypeName = "DateTime")]
        public virtual DateTime TimeStamp { get; set; }
    }
}