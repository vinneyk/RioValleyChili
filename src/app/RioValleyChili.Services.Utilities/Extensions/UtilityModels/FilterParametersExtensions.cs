using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Utilities.Helpers;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Extensions.UtilityModels
{
    internal static class FilterParametersExtensions
    {
        internal static IResult<ContractPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterCustomerContractsParameters parameters)
        {
            if(parameters == null) { return new SuccessResult<ContractPredicateBuilder.PredicateBuilderFilters>(); }

            var result = new ContractPredicateBuilder.PredicateBuilderFilters
                {
                    ContractStatus = parameters.ContractStatus,
                    TermBeginRangeStart = parameters.TermBeginRangeStart,
                    TermBeginRangeEnd = parameters.TermBeginRangeEnd
                };

            if(parameters.CustomerKey != null)
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<ContractPredicateBuilder.PredicateBuilderFilters>();
                }
                result.CustomerKey = customerKeyResult.ResultingObject;
            }

            if(parameters.BrokerKey != null)
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<ContractPredicateBuilder.PredicateBuilderFilters>();
                }
                result.BrokerKey = brokerKeyResult.ResultingObject;
            }

            return new SuccessResult<ContractPredicateBuilder.PredicateBuilderFilters>(result);
        }

        internal static IResult<SalesOrderPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterSalesOrdersParameters parameters)
        {
            if(parameters == null) { return new SuccessResult<SalesOrderPredicateBuilder.PredicateBuilderFilters>(); }

            var result = new SalesOrderPredicateBuilder.PredicateBuilderFilters
                {
                    SalesOrderStatus = parameters.SalesOrderStatus,
                    OrderReceivedRangeStart = parameters.OrderReceivedRangeStart,
                    OrderReceivedRangeEnd = parameters.OrderReceivedRangeEnd,
                    ScheduledShipDateRangeStart = parameters.ScheduledShipDateRangeStart,
                    ScheduledShipDateRangeEnd = parameters.ScheduledShipDateRangeEnd,
                };

            if(parameters.CustomerKey != null)
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<SalesOrderPredicateBuilder.PredicateBuilderFilters>();
                }
                result.CustomerKey = customerKeyResult.ResultingObject;
            }

            if(parameters.BrokerKey != null)
            {
                var brokerKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!brokerKeyResult.Success)
                {
                    return brokerKeyResult.ConvertTo<SalesOrderPredicateBuilder.PredicateBuilderFilters>();
                }
                result.BrokerKey = brokerKeyResult.ResultingObject;
            }

            return new SuccessResult<SalesOrderPredicateBuilder.PredicateBuilderFilters>(result);
        }

        internal static IResult<InventoryAdjustmentPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterInventoryAdjustmentParameters parameters)
        {
            if(parameters == null) { return new SuccessResult<InventoryAdjustmentPredicateBuilder.PredicateBuilderFilters>(null); }

            var result = new InventoryAdjustmentPredicateBuilder.PredicateBuilderFilters
                {
                    AdjustmentDateRangeStart = parameters.AdjustmentDateRangeStart,
                    AdjustmentDateRangeEnd = parameters.AdjustmentDateRangeEnd
                };

            if(parameters.LotKey != null)
            {
                var lotKeyResult = KeyParserHelper.ParseResult<ILotKey>(parameters.LotKey);
                if(!lotKeyResult.Success)
                {
                    return lotKeyResult.ConvertTo<InventoryAdjustmentPredicateBuilder.PredicateBuilderFilters>();
                }
                result.LotKey = new LotKey(lotKeyResult.ResultingObject);
            }

            return new SuccessResult<InventoryAdjustmentPredicateBuilder.PredicateBuilderFilters>(result);
        }

        internal static IResult<SampleOrderPredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterSampleOrdersParameters parameters)
        {
            if(parameters == null) { return new SuccessResult<SampleOrderPredicateBuilder.PredicateBuilderFilters>(); }

            var result = new SampleOrderPredicateBuilder.PredicateBuilderFilters
                {
                    DateReceivedStart = parameters.DateReceivedStart,
                    DateReceivedEnd = parameters.DateReceivedEnd,
                    DateCompletedStart = parameters.DateCompletedStart,
                    DateCompletedEnd = parameters.DateCompletedEnd,
                    Status = parameters.Status,
                };

            if(!string.IsNullOrWhiteSpace(parameters.RequestedCompanyKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.RequestedCompanyKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<SampleOrderPredicateBuilder.PredicateBuilderFilters>();
                }

                result.RequestedCustomerKey = customerKeyResult.ResultingObject.ToCustomerKey();
            }

            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!companyKeyResult.Success)
                {
                    return companyKeyResult.ConvertTo<SampleOrderPredicateBuilder.PredicateBuilderFilters>();
                }

                result.BrokerKey = companyKeyResult.ResultingObject.ToCompanyKey();
            }

            return new SuccessResult<SampleOrderPredicateBuilder.PredicateBuilderFilters>(result);
        }

        internal static IResult<SalesQuotePredicateBuilder.PredicateBuilderFilters> ParseToPredicateBuilderFilters(this FilterSalesQuotesParameters parameters)
        {
            if(parameters == null) { return new SuccessResult<SalesQuotePredicateBuilder.PredicateBuilderFilters>(); }

            var result = new SalesQuotePredicateBuilder.PredicateBuilderFilters();

            if(!string.IsNullOrWhiteSpace(parameters.CustomerKey))
            {
                var customerKeyResult = KeyParserHelper.ParseResult<ICustomerKey>(parameters.CustomerKey);
                if(!customerKeyResult.Success)
                {
                    return customerKeyResult.ConvertTo<SalesQuotePredicateBuilder.PredicateBuilderFilters>();
                }

                result.CustomerKey = customerKeyResult.ResultingObject.ToCustomerKey();
            }

            if(!string.IsNullOrWhiteSpace(parameters.BrokerKey))
            {
                var companyKeyResult = KeyParserHelper.ParseResult<ICompanyKey>(parameters.BrokerKey);
                if(!companyKeyResult.Success)
                {
                    return companyKeyResult.ConvertTo<SalesQuotePredicateBuilder.PredicateBuilderFilters>();
                }

                result.BrokerKey = companyKeyResult.ResultingObject.ToCompanyKey();
            }

            return new SuccessResult<SalesQuotePredicateBuilder.PredicateBuilderFilters>(result);
        }
    }
}