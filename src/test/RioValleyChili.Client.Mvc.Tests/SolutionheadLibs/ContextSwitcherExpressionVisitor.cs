using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Extensions;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding.Helpers;

namespace RioValleyChili.Tests.SolutionheadLibs
{
    [TestFixture]
    public class ContextSwitcherVisitorTests
    {

        class Parent
        {
            public Child Child { get; set; }
        }
        class Child 
        {
            public string Name { get; set; }
        }

        [Test]
        public void SwitchContextToChildSucceeds()
        {
            // Arrange
            var visitor = new ContextSwitcherVisitor();
            Expression<Func<Parent, Child>> origExpression = parent => parent.Child;
            
            // Act
            var visitedExpression = visitor.Visit(origExpression) as LambdaExpression;

            // Assert
            Assert.IsNotNull(visitedExpression);
            Assert.AreEqual(typeof(Child), visitedExpression.Parameters[0].Type);
        }

        [Test]
        public void UnchangedContextSucceeds()
        {
            // Arrange
            var visitor = new ContextSwitcherVisitor();
            Expression<Func<Parent, Parent>> origExpression = parent => parent;

            // Act
            var visitedExpression = visitor.Visit(origExpression) as LambdaExpression;

            // Assert
            Assert.IsNotNull(visitedExpression);
            Assert.AreEqual(typeof(Parent), visitedExpression.Parameters[0].Type);
        }
    }

    [TestFixture]
    public class ContextSwitcherTests
    {
        class Parent
        {
            public Child Child { get; set; }
        }
        class Child
        {
            public string Name { get; set; }
        }

        [Test]
        public void CanSwitchContextToChild()
        {
            // Arrange
            var vm = new Parent
                         {
                             Child = new Child {Name = "Rudy"}
                         };
            Expression<Func<Parent, Child>> expression = parent => parent.Child;

            // Act
            var newContext = ContextSwitcher.SwitchContext(expression, vm);

            // Assert
            Assert.IsNotNull(newContext);
            Assert.AreEqual(vm.Child, newContext);
        }
    }
}
