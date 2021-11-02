using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using RioValleyChili.Core;
using RioValleyChili.Data.DataSeeders.Mothers.Base;
using RioValleyChili.Data.DataSeeders.Utilities;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders.Mothers.EntityObjectMothers
{
    public class CustomerProductCodeMother : EntityMotherLogBase<CustomerProductCode, CustomerProductCodeMother.CallbackParameters>
    {
        private readonly NewContextHelper _newContextHelper;

        public CustomerProductCodeMother(ObjectContext oldContext, RioValleyChiliDataContext newContext, Action<CallbackParameters> loggingCallback) : base(oldContext, loggingCallback)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            _newContextHelper = new NewContextHelper(newContext);
        }

        private enum EntityTypes
        {
            CustomerProductCode
        }

        private readonly MotherLoadCount<EntityTypes> _loadCount = new MotherLoadCount<EntityTypes>();

        protected override IEnumerable<CustomerProductCode> BirthRecords()
        {
            _loadCount.Reset();

            foreach(var order in GetCodes())
            {
                var customer = _newContextHelper.GetCompany(order.Company_IA, CompanyType.Customer);

                foreach(var product in order.Products)
                {
                    var codes = product.CustProductCodes
                        .Where(c => !string.IsNullOrWhiteSpace(c.CustProductCode))
                        .GroupBy(c => c.CustProductCode)
                        .OrderByDescending(g => g.Max(c => c.ODetail))
                        .Select(g => g.First())
                        .ToList();

                    if(!codes.Any())
                    {
                        continue;
                    }

                    if(codes.Count > 1)
                    {
                        Log(new CallbackParameters(CallbackReason.MultipleCodes)
                            {
                                Order = order,
                                Product = product
                            });
                    }

                    _loadCount.AddRead(EntityTypes.CustomerProductCode, (uint) codes.Count);

                    var chileProduct = _newContextHelper.GetChileProduct(product.ProdID);
                    if(chileProduct == null)
                    {
                        Log(new CallbackParameters(CallbackReason.ChileProductNotFound)
                            {
                                Product = product
                            });
                        continue;
                    }

                    if(customer == null)
                    {
                        Log(new CallbackParameters(CallbackReason.CustomerNotFound)
                            {
                                Order = order
                            });
                        break;
                    }

                    _loadCount.AddLoaded(EntityTypes.CustomerProductCode);

                    yield return new CustomerProductCode
                        {
                            CustomerId = customer.Id,
                            ChileProductId = chileProduct.Id,
                            Code = codes.First().CustProductCode
                        };
                }
            }

            _loadCount.LogResults(l => Log(new CallbackParameters(l)));
        }

        private IEnumerable<tblOrderDTO> GetCodes()
        {
            return OldContext.CreateObjectSet<tblOrderDetail>()
                .Where(d => d.CustProductCode != null && d.CustProductCode != "")
                .GroupBy(d => d.tblOrder.Company_IA)
                .Select(byCompany => new tblOrderDTO
                    {
                        Company_IA = byCompany.Key,
                        Products = byCompany.GroupBy(d => d.ProdID)
                            .Select(byProduct => new tblProductDTO
                                {
                                    ProdID = byProduct.Key,
                                    CustProductCodes = byProduct.Select(d => new tblOrderDetailDTO
                                        {
                                            ODetail = d.ODetail,
                                            CustProductCode = d.CustProductCode
                                        })
                                })
                    });
        }

        public class tblOrderDTO
        {
            public string Company_IA { get; set; }
            public IEnumerable<tblProductDTO> Products { get; set; }
        }

        public class tblProductDTO
        {
            public int? ProdID { get; set; }
            public IEnumerable<tblOrderDetailDTO> CustProductCodes { get; set; }
        }

        public class tblOrderDetailDTO
        {
            public DateTime ODetail { get; set; }
            public string CustProductCode { get; set; }
        }

        public enum CallbackReason
        {
            Exception,
            Summary,
            StringTruncated,
            CustomerNotFound,
            ChileProductNotFound,
            MultipleCodes
        }

        public class CallbackParameters : CallbackParametersBase<CallbackReason>
        {
            public CallbackParameters() { }
            public CallbackParameters(CallbackReason callbackReason) : base(callbackReason) { }
            public CallbackParameters(string summaryMessage) : base(summaryMessage) { }
            public CallbackParameters(DataStringPropertyHelper.Result stringResult) : base(stringResult) { }

            protected override CallbackReason ExceptionReason { get { return CustomerProductCodeMother.CallbackReason.Exception; } }
            protected override CallbackReason SummaryReason { get { return CustomerProductCodeMother.CallbackReason.Summary; } }
            protected override CallbackReason StringResultReason { get { return CustomerProductCodeMother.CallbackReason.StringTruncated; } }

            protected override ReasonCategory DerivedGetReasonCategory(CallbackReason reason)
            {
                switch(reason)
                {
                    case CustomerProductCodeMother.CallbackReason.CustomerNotFound:
                    case CustomerProductCodeMother.CallbackReason.ChileProductNotFound:
                        return ReasonCategory.RecordSkipped;
                }
                return base.DerivedGetReasonCategory(reason);
            }

            public tblOrderDTO Order { get; set; }
            public tblProductDTO Product { get; set; }
        }
    }
}