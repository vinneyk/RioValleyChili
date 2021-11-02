using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class CustomerCompany : ICustomerCompanyKey
    {
        [Key]
        [Column(Order = 0)]
        public virtual int CustomerId { get; set; }

        [Key]
        [Column(Order = 1)]
        public virtual int CompanyId { get; set; }

        public virtual int BrokerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }

        public virtual ICollection<CustomerOrder> Orders { get; set; }

        #region ICustomerCompanyKey Implementation

        public int CustomerKey_Id { get { return CompanyId; } }

        public int CompanyKey_Id { get { return CompanyId; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}