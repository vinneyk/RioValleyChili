using System;
using System.Globalization;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class CompanyKey : EntityKeyBase.Of<ICompanyKey>, IKey<Company>, IKey<Customer>, ICompanyKey
    {
        #region Fields and Constructors

        private readonly int _id;

        private CompanyKey(int id)
        {
            _id = id;
        }

        public CompanyKey() : this(Null) { }

        public CompanyKey(ICompanyKey companyKey)
        {
            _id = companyKey.CompanyKey_Id;
        }

        #endregion

        #region Overrides of EntityKeyBase.Of<ICompanyKey>

        protected override ICompanyKey ParseImplementation(string keyValue)
        {
            return new CompanyKey(int.Parse(keyValue));
        }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidCompanyKey, inputValue);
        }

        public override ICompanyKey Default
        {
            get { return Null; }
        }

        protected override ICompanyKey Key
        {
            get { return this; }
        }

        protected override string BuildKeyValueImplementation(ICompanyKey key)
        {
            return key.CompanyKey_Id.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Implementation of IKey<Company>

        public Expression<Func<Company, bool>> FindByPredicate
        {
            get { return v => v.Id == _id; }
        }

        #endregion

        #region Implementation of IKey<Customer>

        Expression<Func<Customer, bool>> IKey<Customer>.FindByPredicate
        {
            get { return v => v.Id == _id; }
        }

        #endregion

        #region Implementation of ICompanyKey

        public int CompanyKey_Id { get { return _id; } }

        #endregion

        public override bool Equals(object obj)
        {
            if(obj == null) { return false; }
            if(ReferenceEquals(obj, this)) { return true; }
            return obj as ICompanyKey != null && Equals(obj as ICompanyKey);
        }

        protected bool Equals(ICompanyKey other)
        {
            return _id == other.CompanyKey_Id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static ICompanyKey Null = new NullCompanyKey();

        private class NullCompanyKey : ICompanyKey
        {
            public int CompanyKey_Id { get { return -1; } }
        }
    }

    public static class ICompanyExtensions
    {
        public static CompanyKey ToCompanyKey(this ICompanyKey k)
        {
            return new CompanyKey(k);
        }
    }
}