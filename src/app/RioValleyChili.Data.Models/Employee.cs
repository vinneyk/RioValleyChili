using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core.Interfaces.Keys;

namespace RioValleyChili.Data.Models
{
    public class Employee : IEmployeeKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeId { get; set; }

        [Required, StringLength(15)]
        public string UserName { get; set; }
        [Required, StringLength(25)]
        public string DisplayName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Serialized JSON object containing claims for an employee.
        /// </summary>
        public string Claims { get; set; }

        public int EmployeeKey_Id { get { return EmployeeId; } }
    }
}