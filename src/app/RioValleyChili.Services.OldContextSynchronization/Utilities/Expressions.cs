using System;
using System.Linq;
using System.Linq.Expressions;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class Expressions
    {
        public static Expression<Func<Contract, bool>> ContractByCommentsNotebook(INotebookKey notebookKey)
        {
            return c => c.CommentsDate == notebookKey.NotebookKey_Date && c.CommentsSequence == notebookKey.NotebookKey_Sequence;
        }

        public static Expression<Func<Lot, object>>[] SynchLotPaths
        {
            get
            {
                return new Expression<Func<Lot, object>>[]
                    {
                        l => l.Attributes.Select(a => a.AttributeName),
                        l => l.LotDefects.Select(d => d.Resolution),
                        l => l.AttributeDefects.Select(d => d.LotDefect.Resolution),
                        l => l.AdditiveLot,
                        l => l.ChileLot.ChileProduct.ProductAttributeRanges,
                        l => l.PackagingLot,
                        l => l.ReceivedPackaging.Product,
                        l => l.CustomerAllowances.Select(c => c.Customer.Company),
                        l => l.ContractAllowances.Select(c => c.Contract),
                        l => l.SalesOrderAllowances.Select(c => c.SalesOrder.InventoryShipmentOrder),
                        l => l.Vendor
                    };
            }
        }

        public static Expression<Func<Contract, object>>[] SynchContractPaths
        {
            get
            {
                return new Expression<Func<Contract, object>>[]
                    {
                        c => c.Customer.Company,
                        c => c.Broker,
                        c => c.Comments.Notes,
                        c => c.DefaultPickFromFacility,
                        c => c.ContractItems.Select(i => i.ChileProduct.Product),
                        c => c.ContractItems.Select(i => i.PackagingProduct.Product)
                    };
            }
        }
    }
}