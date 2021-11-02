using System.Collections.Generic;
using System.Linq.Expressions;

namespace RioValleyChili.Client.Reporting
{
    public class MemberAccessPathBuilder : ExpressionVisitor
    {
        public static string GetPath(Expression expression)
        {
            return new MemberAccessPathBuilder().BuildPath(expression);
        }

        private readonly List<string> _members = new List<string>();

        private MemberAccessPathBuilder() { }

        private string BuildPath(Expression expression)
        {
            Visit(expression);
            return string.Join(".", _members);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _members.Insert(0, node.Member.Name);
            return base.VisitMember(node);
        }
    }
}