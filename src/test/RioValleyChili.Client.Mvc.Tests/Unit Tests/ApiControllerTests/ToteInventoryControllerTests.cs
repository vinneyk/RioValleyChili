using System;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Returns.InventoryServices;
using Solutionhead.Services;

namespace RioValleyChili.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class ToteInventoryControllerTests
    {
        private ToteInventoryController systemUnderTest;
        private Mock<IInventoryService> mockInventoryService;
        private readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();

        public ToteInventoryControllerTests()
        {
            _fixture.Customizations.Add(new ToteInventorySpecimenBuilder());
        }

        [SetUp]
        public void SetUp()
        {
            mockInventoryService = new Mock<IInventoryService>();
            systemUnderTest = new ToteInventoryController(mockInventoryService.Object);
        }

        [Test]
        public void GetById_CallsServiceMethodWithCorrectParameters()
        {
            // Arrange
            var toteKey = "12345";
            var serviceParameters = new FilterInventoryParameters { ToteKey = toteKey };
            SetupGetInventoryByToteKeysMethod(callback: p => serviceParameters = p);

            // Act
            systemUnderTest.Get(toteKey);

            // Assert
            Assess.IsNotNull(serviceParameters);
            mockInventoryService.Verify(m => m.GetInventory(serviceParameters), Times.Once());
        }

        /// <summary>
        /// Temporary utility used for handling two design issues: Tote as int (as opposed to string) and the IEnumerable of LotToteKeys on IToteKeysInventorySummaryReturn.
        /// </summary>
        private class ToteInventorySpecimenBuilder : ISpecimenBuilder
        {
            private static readonly IFixture _fixture = AutoFixtureHelper.BuildFixture();
            private readonly int[] lotToteKeys = new[] { _fixture.Create<int>() };
            private const string lotKey = "01 13 123 04";
            private readonly IInventoryProductReturn inventoryProduct = _fixture.Create<IInventoryProductReturn>();

            public object Create(object request, ISpecimenContext context)
            {
                var propertyInfo = request as PropertyInfo;
                if (propertyInfo == null)  {  return new NoSpecimen(request); }

                if (propertyInfo.DeclaringType == typeof (ToteInventory))
                {
                    if (propertyInfo.Name == "ToteKey")
                    {
                        return _fixture.Create<int>().ToString();
                    }
                }
                //else if (propertyInfo.DeclaringType == typeof (IToteKeysInventorySummaryReturn))
                //{
                //    if (propertyInfo.Name == "LotToteKeys")
                //    {
                //        return lotToteKeys;
                //    }
                //}
                else if (propertyInfo.DeclaringType == typeof(IInventorySummaryReturn))
                {
                    if (propertyInfo.Name == "LotKey")
                    {
                        return lotKey;
                    }
                    if (propertyInfo.Name == "InventoryProduct")
                    {
                        return inventoryProduct;
                    }
                }

                return new NoSpecimen(request);
            }
        }

        private void SetupGetInventoryByToteKeysMethod(IInventoryReturn returnData = null, Action<FilterInventoryParameters> callback = null)
        {
            returnData = returnData ?? _fixture.Create<IInventoryReturn>();
            Action<FilterInventoryParameters> defaultCallback = x => { };

            mockInventoryService
                .Setup(m => m.GetInventory(It.IsAny<FilterInventoryParameters>()))
                .Returns(() => new SuccessResult<IInventoryReturn>(returnData)).Callback(callback ?? defaultCallback);
        }
    }
}
