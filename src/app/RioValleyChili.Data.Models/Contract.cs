// ReSharper disable RedundantExtendsListEntry

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RioValleyChili.Core;
using RioValleyChili.Core.Helpers;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Core.Models;
using RioValleyChili.Data.Models.Helpers;

namespace RioValleyChili.Data.Models
{
    public class Contract : EmployeeIdentifiableBase, IContractKey, ICustomerKey, IFacilityKey, INotebookKey
    {
        [Key, Column(Order = 0)]
        public virtual int ContractYear { get; set; }
        [Key, Column(Order = 1)]
        public virtual int ContractSequence { get; set; }

        public virtual int CustomerId { get; set; }
        public virtual int BrokerId { get; set; }
        public virtual int DefaultPickFromWarehouseId { get; set; }
        [Obsolete("\"Temporary\" while old context is still relevant. -RI 2014/11/10")]
        public virtual int? ContractId { get; set; }

        public virtual DateTime TimeCreated { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime ContractDate { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime? TermBegin { get; set; }
        [Column(TypeName = "Date")]
        public virtual DateTime? TermEnd { get; set; }
        public virtual ContractType ContractType { get; set; }
        public virtual ContractStatus ContractStatus { get; set; }
        public virtual DateTime CommentsDate { get; set; }
        public virtual int CommentsSequence { get; set; }
        public virtual string ContactName { get; set; }
        [StringLength(Constants.StringLengths.PaymentTerms)]
        public virtual string PaymentTerms { get; set; }
        [StringLength(300)]
        public virtual string NotesToPrint { get; set; }
        [StringLength(Constants.StringLengths.PurchaseOrderNumber)]
        public virtual string CustomerPurchaseOrder { get; set; }
        [StringLength(Constants.StringLengths.FOB)]
        public virtual string FOB { get; set; }

        public virtual Address ContactAddress { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("BrokerId")]
        public virtual Company Broker { get; set; }
        [ForeignKey("DefaultPickFromWarehouseId")]
        public virtual Facility DefaultPickFromFacility { get; set; }
        [ForeignKey("CommentsDate, CommentsSequence")]
        public virtual Notebook Comments { get; set; }

        public virtual ICollection<ContractItem> ContractItems { get; set; }
        public virtual ICollection<LotContractAllowance> LotAllowances { get; set; }

        #region Key Interface Implemenations

        #region IContractKey

        public int ContractKey_Year { get { return ContractYear; } }
        public int ContractKey_Sequence { get { return ContractSequence; } }

        #endregion

        public int CustomerKey_Id { get { return CustomerId; } }
        public int FacilityKey_Id { get { return DefaultPickFromWarehouseId; } }

        #region INotebookKey

        public DateTime NotebookKey_Date { get { return CommentsDate; } }
        public int NotebookKey_Sequence { get { return CommentsSequence; } }

        #endregion

        #endregion

        public override string ToString()
        {
            return DataModelKeyStringBuilder.BuildKeyString(this);
        }
    }
}

// ReSharper restore RedundantExtendsListEntry