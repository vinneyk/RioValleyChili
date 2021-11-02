using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc;
using RioValleyChili.Client.Mvc.Areas.API.Models.Response;
using RioValleyChili.Services.Interfaces.Returns.LotService;

namespace RioValleyChili.Tests.Unit_Tests.ObjectMappingTests
{
    [TestFixture]
    public class LabResultsIndexViewModelMapping
    {
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();

        public LabResultsIndexViewModelMapping()
        {
            AutoMapperConfiguration.Configure();
        }

        [Test]
        public void MyTest()
        {
            // Arrange
            var source = _fixture.CreateMany<ILotQualitySummaryReturn>();

            // Act
            var mapped = Mapper.Map<IEnumerable<LotSummary>>(source);

            // Assert
            Assert.IsNotNull(mapped);
        }
    }
}
