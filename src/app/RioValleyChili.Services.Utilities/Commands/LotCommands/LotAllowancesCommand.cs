using System.Linq;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Data.Interfaces.UnitsOfWork;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Utilities.Commands.Parameters;
using RioValleyChili.Services.Utilities.LinqPredicates;
using Solutionhead.Services;

namespace RioValleyChili.Services.Utilities.Commands.LotCommands
{
    internal class LotAllowancesCommand
    {
        private readonly ILotUnitOfWork _lotUnitOfWork;

        internal LotAllowancesCommand(ILotUnitOfWork lotUnitOfWork)
        {
            _lotUnitOfWork = lotUnitOfWork;
        }

        internal IResult Execute(LotAllowanceParameters parameters, bool add)
        {
            var lot = _lotUnitOfWork.LotRepository.FindByKey(parameters.LotKey,
                l => l.CustomerAllowances,
                l => l.SalesOrderAllowances,
                l => l.ContractAllowances);
            if(lot == null)
            {
                return new InvalidResult(string.Format(UserMessages.LotNotFound, parameters.LotKey));
            }

            var noWorkRequired = true;
            if(parameters.CustomerKey != null)
            {
                var byCustomer = LotCustomerAllowancePredicates.ByCustomerKey(parameters.CustomerKey).Compile();
                var customerAllowance = lot.CustomerAllowances.FirstOrDefault(byCustomer);
                if(add)
                {
                    if(customerAllowance == null)
                    {
                        lot.CustomerAllowances.Add(new LotCustomerAllowance
                            {
                                CustomerId = parameters.CustomerKey.CustomerKey_Id
                            });
                        noWorkRequired = false;
                    }
                }
                else
                {
                    if(customerAllowance != null)
                    {
                        lot.CustomerAllowances.Remove(customerAllowance);
                        noWorkRequired = false;
                    }
                }
            }

            if(parameters.ContractKey != null)
            {
                var byContract = LotContractAllowancePredicates.ByContractKey(parameters.ContractKey).Compile();
                var contractAllowance = lot.ContractAllowances.FirstOrDefault(byContract);
                if(add)
                {
                    if(contractAllowance == null)
                    {
                        lot.ContractAllowances.Add(new LotContractAllowance
                            {
                                ContractYear = parameters.ContractKey.ContractKey_Year,
                                ContractSequence = parameters.ContractKey.ContractKey_Sequence
                            });
                        noWorkRequired = false;
                    }
                }
                else
                {
                    if(contractAllowance != null)
                    {
                        lot.ContractAllowances.Remove(contractAllowance);
                        noWorkRequired = false;
                    }
                }
            }

            if(parameters.SalesOrderKey != null)
            {
                var byCustomerOrder = LotCustomerOrderAllowancePredicates.ByCustomerOrderKey(parameters.SalesOrderKey).Compile();
                var customerOrderAllowance = lot.SalesOrderAllowances.FirstOrDefault(byCustomerOrder);
                if(add)
                {
                    if(customerOrderAllowance == null)
                    {
                        lot.SalesOrderAllowances.Add(new LotSalesOrderAllowance
                            {
                                SalesOrderDateCreated = parameters.SalesOrderKey.SalesOrderKey_DateCreated,
                                SalesOrderSequence = parameters.SalesOrderKey.SalesOrderKey_Sequence,
                            });
                        noWorkRequired = false;
                    }
                }
                else
                {
                    if(customerOrderAllowance != null)
                    {
                        lot.SalesOrderAllowances.Remove(customerOrderAllowance);
                        noWorkRequired = false;
                    }
                }
            }

            return noWorkRequired ? (IResult)new NoWorkRequiredResult() : new SuccessResult();
        }
    }
}