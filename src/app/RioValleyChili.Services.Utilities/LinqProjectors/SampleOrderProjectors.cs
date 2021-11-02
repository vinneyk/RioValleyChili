// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SampleOrderProjectors
    {
        internal static Expression<Func<SampleOrder, SampleOrderKeyReturn>> SelectKey()
        {
            return s => new SampleOrderKeyReturn
                {
                    SampleOrderKey_Year = s.Year,
                    SampleOrderKey_Sequence = s.Sequence
                };
        }

        internal static Expression<Func<SampleOrder, SampleOrderSummaryReturn>> SelectSummary()
        {
            var key = SelectKey();
            var company = CompanyProjectors.SelectHeader();
            var employee = EmployeeProjectors.SelectSummary();

            return Projector<SampleOrder>.To(s => new SampleOrderSummaryReturn
                {
                    DateDue = s.DateDue,
                    DateReceived = s.DateReceived,
                    DateCompleted = s.DateCompleted,
                    Status = s.Status,

                    RequestedByCompany = new [] { s.RequestCustomer }.Where(c => c != null).Select(c => company.Invoke(c.Company)).FirstOrDefault(),
                    Broker = new [] { s.Broker }.Where(c => c != null).Select(c => company.Invoke(c)).FirstOrDefault(),
                    CreatedByUser = employee.Invoke(s.Employee),

                    SampleOrderKeyReturn = key.Invoke(s),
                });
        }

        internal static IEnumerable<Expression<Func<SampleOrder, SampleOrderDetailReturn>>> SelectDetail()
        {
            var key = SelectKey();
            var company = CompanyProjectors.SelectHeader();
            var employee = EmployeeProjectors.SelectSummary();
            var item = SampleOrderItemProjectors.Select();
            var journal = SampleOrderJournalEntryProjectors.Select();

            return new Projectors<SampleOrder, SampleOrderDetailReturn>
                {
                    s => new SampleOrderDetailReturn
                        {
                            DateDue = s.DateDue,
                            DateReceived = s.DateReceived,
                            DateCompleted = s.DateCompleted,
                            Status = s.Status,
                            Active = s.Active,
                            FOB = s.FOB,
                            ShipVia = s.ShipmentMethod,

                            RequestedByShippingLabel = s.Request,
                            ShipToCompany = s.ShipToCompany,
                            ShipToShippingLabel = s.ShipTo,

                            Comments = s.Comments,
                            NotesToPrint = s.PrintNotes,

                            SampleOrderKeyReturn = key.Invoke(s),
                        },
                    s => new SampleOrderDetailReturn
                        {
                            RequestedByCompany = new [] { s.RequestCustomer }.Where(c => c != null).Select(c => company.Invoke(c.Company)).FirstOrDefault(),
                            Broker = new [] { s.Broker }.Where(c => c != null).Select(c => company.Invoke(c)).FirstOrDefault(),
                            CreatedByUser = employee.Invoke(s.Employee),
                        },
                    s => new SampleOrderDetailReturn
                        {
                            Items = s.Items.Select(i => item.Invoke(i))
                        },
                    s => new SampleOrderDetailReturn
                        {
                            JournalEntries = s.JournalEntries.Select(j => journal.Invoke(j))
                        }
                };
        }

        internal static Expression<Func<SampleOrder, SampleOrderMatchingSummaryReportReturn>> SelectMatchingSummaryReport(IKey<SampleOrderItem> itemKeyFilter = null)
        {
            var key = SelectKey();
            var lotKey = LotProjectors.SelectLotKey<Lot>();
            var itemFilter = itemKeyFilter == null ? PredicateBuilder.True<SampleOrderItem>() : itemKeyFilter.FindByPredicate;

            return Projector<SampleOrder>.To(s => new SampleOrderMatchingSummaryReportReturn
                {
                    SampleOrderKeyReturn = key.Invoke(s),
                    Company = new[] { s.RequestCustomer }.Where(c => c != null).Select(c => c.Company.Name).FirstOrDefault(),
                    Contact = s.Request.Name,
                    Items = s.Items
                    .Where(i => itemFilter.Invoke(i))
                    .Select(i => new SampleOrderMatchingItemReturn
                        {
                            SampleMatch = i.CustomerProductName,
                            Spec = i.Spec,
                            Match = i.Match,
                            LotAttributes = new[] { i.Lot }.Where(c => c != null).SelectMany(l => l.Attributes),

                            OrderStatus = s.Status,
                            ProductName = new[] { i.Product }.Where(p => p != null).Select(p => p.Name).FirstOrDefault(),
                            LotKeyReturn = new[] { i.Lot }.Where(l => l != null).Select(l => lotKey.Invoke(l)).FirstOrDefault(),
                            Quantity = i.Quantity,
                            Description = i.Description,
                            Employee = s.Employee.UserName,
                            Received = s.DateReceived,
                            SampleDate = s.DateDue,
                            Completed = s.DateCompleted
                        })
                });
        }

        internal static Expression<Func<SampleOrder, SampleOrderRequestReportReturn>> SelectSampleOrderRequestReport()
        {
            var key = SelectKey();

            return Projector<SampleOrder>.To(s => new SampleOrderRequestReportReturn
                {
                    SampleOrderKeyReturn = key.Invoke(s),
                    ShipByDate = s.DateDue,

                    Broker = new[]{ s.Broker }.Where(c => c != null).Select(b => b.Name).FirstOrDefault(),
                    ShipVia = s.ShipmentMethod,
                    FOB = s.FOB,
                    RequestedByCompanyName = s.RequestCustomer.Company.Name,
                    ShipToCompanyName = s.ShipToCompany,

                    RequestedBy = s.Request,
                    ShipTo = s.ShipTo,

                    SpecialInstructions = s.PrintNotes,
                    Items = s.Items.Select(i => new SampleOrderRequestItemReportReturn
                        {
                            ProductCode = new []{ i.Product }.Where(p => p != null).Select(p => p.ProductCode).FirstOrDefault(),
                            ProductName = new []{ i.Product }.Where(p => p != null).Select(p => p.Name).FirstOrDefault(),
                            SampleMatch = i.CustomerProductName,
                            Quantity = i.Quantity,
                            Description = i.Description
                        })
                });
        }

    }
}

// ReSharper restore ConvertClosureToMethodGroup