using System;
using NUnit.Framework;
using Solutionhead.EntityKey;

namespace RioValleyChili.Services.Tests.UnitTests.Keys
{
    [TestFixture]
    public abstract class DateSquenceKeyTestsBase<TKey, TKeyInterface> : KeyTestsBase<TKey, TKeyInterface>
        where TKey : EntityKeyBase.Of<TKeyInterface>, new()
        where TKeyInterface : class 
    {
        protected DateTime ExpectedDate { get { return new DateTime(2012, 3, 29); } }
        protected int ExpectedSequence { get { return 42; } }
        protected sealed override string ExpectedStringValue { get { return "20120329-42"; } }
        protected sealed override string ValidParseInput { get { return "20120329-42"; } }
        protected sealed override string InvalidParseInput { get { return "Oh no."; } }
    }
}