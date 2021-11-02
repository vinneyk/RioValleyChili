// ReSharper disable ConvertClosureToMethodGroup
using System;
using System.Linq;
using System.Linq.Expressions;
using EF_Projectors;
using LinqKit;
using RioValleyChili.Core;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Models;
using RioValleyChili.Services.Utilities.Models.KeyReturns;

namespace RioValleyChili.Services.Utilities.LinqProjectors
{
    internal static class SalesQuoteProjectors
    {
        internal static Expression<Func<SalesQuote, SalesQuoteKeyReturn>> SelectKey()
        {
            return q => new SalesQuoteKeyReturn
                {
                    SalesQuoteKey_DateCreated = q.DateCreated,
                    SalesQuoteKey_Sequence = q.Sequence
                };
        }

        internal static Expression<Func<SalesQuote, SalesQuoteSummaryReturn>> SelectSummary()
        {
            var key = SelectKey();

            return Projector<SalesQuote>.To(q => new SalesQuoteSummaryReturn
                {
                    QuoteNumber = q.QuoteNum,
                    CustomerName = new[] { q.Customer }.Where(c => c != null).Select(c => c.Company.Name).FirstOrDefault(),
                    BrokerName = new[] { q.Broker }.Where(b => b != null).Select(b => b.Name).FirstOrDefault(),
                    SourceFacilityName = new[] { q.SourceFacility }.Where(f => f != null).Select(f => f.Name).FirstOrDefault(),
                    QuoteDate = q.QuoteDate,
                    ShipmentDate = q.ShipmentInformation.ShipmentDate,

                    SalesQuoteKeyReturn = key.Invoke(q)
                });
        }

        public static Expression<Func<SalesQuote, SalesQuoteDetailReturn>> SelectDetail()
        {
            var key = SelectKey();
            var shipment = ShipmentInformationProjectors.SelectDetail(InventoryOrderEnum.Unknown);
            var facility = FacilityProjectors.Select(false, false);
            var company = CompanyProjectors.SelectSummary();
            var item = SalesQuoteItemProjectors.Select();

            return Projector<SalesQuote>.To(q => new SalesQuoteDetailReturn
                {
                    SalesQuoteKeyReturn = key.Invoke(q),
                    QuoteNumber = q.QuoteNum,
                    QuoteDate = q.QuoteDate,
                    DateReceived = q.DateReceived,
                    CalledBy = q.CalledBy,
                    TakenBy = q.TakenBy,
                    PaymentTerms = q.PaymentTerms,
                    ShipFromReplace = q.SoldTo,
                    Shipment = shipment.Invoke(q.ShipmentInformation),

                    SourceFacility = new[] { q.SourceFacility }.Where(f => f != null).Select(f => facility.Invoke(f)).FirstOrDefault(),
                    Customer = new[] { q.Customer }.Where(c => c != null).Select(c => company.Invoke(c.Company)).FirstOrDefault(),
                    Broker = new[] { q.Broker }.Where(b => b != null).Select(b => company.Invoke(b)).FirstOrDefault(),
                    Items = q.Items.Select(i => item.Invoke(i))
                });
        }

        public static Expression<Func<SalesQuote, SalesQuoteReportReturn>> SelectSalesQuoteReport()
        {
            return Projector<SalesQuote>.To(q => new SalesQuoteReportReturn
                {
                    QuoteNumber = q.QuoteNum,
                    QuoteDate = q.QuoteDate,
                    SoldTo = q.SoldTo,
                    ShipTo = q.ShipmentInformation.ShipTo,
                    SourceFacilityName = new[] { q.SourceFacility }.Where(f => f != null).Select(f => f.Name).FirstOrDefault(),
                    PaymentTerms = q.PaymentTerms,
                    Broker = new[] { q.Broker }.Where(b => b != null).Select(b => b.Name).FirstOrDefault(),
                    SpecialInstructions = q.ShipmentInformation.SpecialInstructions,

                    Items = q.Items.Select(i => new SalesQuoteItemReportReturn
                        {
                            ProductCodeSelect = i.Product.ProductCode,
                            ProductNameSelect = i.Product.Name,
                            CustomerCode = i.CustomerProductCode,
                            PackagingName = i.PackagingProduct.Product.Name,
                            Treatment = i.Treatment.ShortName,
                            Quantity = i.Quantity,
                            NetWeight = i.PackagingProduct.Weight * i.Quantity,
                            NetPrice = i.PriceBase + i.PriceFreight + i.PriceTreatment + i.PriceWarehouse - i.PriceRebate
                        })
                });
        }
    }
}
// ReSharper restore ConvertClosureToMethodGroup