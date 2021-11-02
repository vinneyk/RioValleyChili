using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class Customer : ICustomerKey, ICompanyKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual int BrokerId { get; set; }

        [ForeignKey("Id")]
        public virtual Company Company { get; set; }
        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<SalesOrder> Orders { get; set; }
        public virtual ICollection<CustomerProductCode> ProductCodes { get; set; }
        public virtual ICollection<CustomerProductAttributeRange> ProductSpecs { get; set; }
        public virtual ICollection<CustomerNote> Notes { get; set; }
        public virtual ICollection<LotCustomerAllowance> LotAllowances { get; set; }

        #region Key Interface Implemenations

        public int CustomerKey_Id { get { return Id; } }
        public int CompanyKey_Id { get { return Id; } }

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}