using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class CompanyTypeRecordKey : EntityKey<ICompanyTypeRecordKey>.With<int, int>, IKey<CompanyTypeRecord>, ICompanyTypeRecordKey
    {
        public CompanyTypeRecordKey() { }

        public CompanyTypeRecordKey(ICompanyTypeRecordKey companyTypeRecordKey) : base(companyTypeRecordKey) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidCompanyTypeRecordKey, inputValue);
        }

        protected override ICompanyTypeRecordKey ConstructKey(int field0, int field1)
        {
            return new CompanyTypeRecordKey { CompanyKey_Id = field0, CompanyType = field1 };
        }

        protected override With<int, int> DeconstructKey(ICompanyTypeRecordKey key)
        {
            return new CompanyTypeRecordKey { CompanyKey_Id = key.CompanyKey_Id, CompanyType = key.CompanyType };
        }

        public Expression<Func<CompanyTypeRecord, bool>> FindByPredicate { get { return c => c.CompanyId == Field0 && c.CompanyType == Field1; } }

        public int CompanyKey_Id { get { return Field0; } private set { Field0 = value; } }

        public int CompanyType { get { return Field1; } private set { Field1 = value; } }

        public static ICompanyTypeRecordKey Null = new CompanyTypeRecordKey { CompanyKey_Id = -1, CompanyType = -1 };
    }
}