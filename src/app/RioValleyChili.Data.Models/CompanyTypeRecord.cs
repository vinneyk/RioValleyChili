using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class CompanyTypeRecord : ICompanyTypeRecordKey
    {
        [Key, Column(Order =  1)]
        public virtual int CompanyId { get; set; }
        [Key, Column(Order = 2)]
        public virtual int CompanyType { get; set; }

        [NotMapped]
        public virtual CompanyType CompanyTypeEnum
        {
            get { return (CompanyType) CompanyType; }
            set { CompanyType = (int)value; }
        }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        public int CompanyKey_Id { get { return CompanyId; } }
    }
}