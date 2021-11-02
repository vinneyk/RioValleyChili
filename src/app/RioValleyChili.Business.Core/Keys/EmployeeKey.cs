using System;
using System.Linq.Expressions;
using RioValleyChili.Business.Core.Resources;
using RioValleyChili.Core.Interfaces.Keys;
using RioValleyChili.Data.Models;
using Solutionhead.Data;
using Solutionhead.EntityKey;

namespace RioValleyChili.Business.Core.Keys
{
    public class EmployeeKey : EntityKey<IEmployeeKey>.With<int>, IKey<Employee>, IEmployeeKey
    {
        public EmployeeKey() { }

        public EmployeeKey(IEmployeeKey key) : base(key) { }

        public override string GetParseFailMessage(string inputValue = null)
        {
            return string.Format(UserMessages.InvalidEmployeeKey, inputValue);
        }

        protected override IEmployeeKey ConstructKey(int field0)
        {
            return new EmployeeKey
                {
                    Field0 = field0
                };
        }

        protected override With<int> DeconstructKey(IEmployeeKey key)
        {
            return new EmployeeKey
                {
                    EmployeeKey_Id = key.EmployeeKey_Id
                };
        }

        public Expression<Func<Employee, bool>> FindByPredicate
        {
            get { return e => e.EmployeeId == Field0; }
        }

        public int EmployeeKey_Id { get { return Field0; } private set { Field0 = value; } }

        public static IEmployeeKey Null = new EmployeeKey();
    }

    public static class IEmployeeKeyExtensions
    {
        public static EmployeeKey ToEmployeeKey(this IEmployeeKey k)
        {
            return new EmployeeKey(k);
        }
    }
}