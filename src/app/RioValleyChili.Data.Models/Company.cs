using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Attributes;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    [Issue("Old context does not have the notion of company addresses and so we are abandoning it until free from Access (if at all). - RI 2016-10-03",
        Flags = IssueFlags.TodoWhenAccessFreedom,
        References = new[] { "RVCADMIN-1321", "https://solutionhead.slack.com/files/vinneyk/F2H8352SD/Feature__Company___Contact_Maintenance" })]
    public class Company : EmployeeIdentifiableBase, ICompanyKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual bool Active { get; set; }
        [StringLength(Constants.StringLengths.CompanyName)]
        public virtual string Name { get; set; }
        
        [ForeignKey("Id")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Company> Children { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<CompanyTypeRecord> CompanyTypes { get; set; }

        #region Key Interface Implementations

        public int CompanyKey_Id { get { return Id; } }

        #endregion
    }
}