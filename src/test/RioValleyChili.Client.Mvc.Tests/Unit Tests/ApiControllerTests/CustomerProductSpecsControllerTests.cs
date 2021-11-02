using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RioValleyChili.Client.Core.Helpers;
using RioValleyChili.Client.Mvc.Areas.API.Controllers;
using RioValleyChili.Client.Mvc.Areas.API.Models.Parameters;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Services.Interfaces;
using RioValleyChili.Services.Interfaces.Parameters.Common;
using RioValleyChili.Services.Interfaces.Parameters.CustomerService;
using RioValleyChili.Services.Interfaces.Returns.CustomerService;
using RioValleyChili.Tests.Helpers;
using RioValleyChili.Tests.Unit_Tests.ApiControllerTests;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Tests.Unit_Tests.ApiControllerTests
{
    [TestFixture]
    public class CustomerProductSpecsControllerTests //: ApiControllerTestsBase<CustomerProductSpecsController>
    {
        private CustomerProductSpecsController _systemUnderTest;
        protected Mock<ICustomerService> MockCustomerService;
        protected Mock<IUserIdentityProvider> MockUserIdentityProvider;
        protected Type ControllerType = typeof (CustomerProductSpecsController);

        protected CustomerProductSpecsController SystemUnderTest
        {
            get { return _systemUnderTest; }
        }

        protected string[] ClaimResources
        {
            get { return new [] { ClaimTypes.QualityControlClaimTypes.CustomerProductSpec }; }
        }

        [SetUp]
        public void SetUp()
        {
            AutoMapperConfiguration.Configure();
            MockCustomerService = new Mock<ICustomerService>();
            MockUserIdentityProvider = new Mock<IUserIdentityProvider>();
            _systemUnderTest = new CustomerProductSpecsController(MockCustomerService.Object, MockUserIdentityProvider.Object);
        }

        [TestFixture]
        public class PostCustomerProductSpecTests : CustomerProductSpecsControllerTests
        {
            [Test]
            public void ParametersAreCorrectlyParsedIntoDataTransferObject()
            {
                // arrange
                const string custKey = "1234";
                const string productKey = "asdf";
                var values = AutoFixtureHelper.BuildFixture().Create<SetCustomerProductRangesRequest>();
                ISetCustomerProductAttributeRangesParameters actualValues = null;
                MockCustomerService.Setup(m => m.SetCustomerChileProductAttributeRanges(It.IsAny<ISetCustomerProductAttributeRangesParameters>()))
                    .Callback((ISetCustomerProductAttributeRangesParameters p) => actualValues = p)
                    .Returns(new SuccessResult());

                // act
                SystemUnderTest.Post(custKey, productKey, values);

                // assert
                Assert.IsNotNull(actualValues);
                Assert.AreEqual(custKey, actualValues.CustomerKey);
                Assert.AreEqual(productKey, productKey);
            }

            [Test]
            public void UtilizesUserIdentityProvider()
            {
                // arrange
                const string custKey = "1234";
                const string productKey = "asdf";
                var values = AutoFixtureHelper.BuildFixture().Create<SetCustomerProductRangesRequest>();
                const string expectedUserToken = "USER123";

                MockUserIdentityProvider.Setup(m => m.SetUserIdentity(It.IsAny<IUserIdentifiable>()))
                    .Callback((IUserIdentifiable t) =>
                    {
                        t.UserToken = expectedUserToken;
                    });

                ISetCustomerProductAttributeRangesParameters actualValues = null;
                MockCustomerService.Setup(m => m.SetCustomerChileProductAttributeRanges(It.IsAny<ISetCustomerProductAttributeRangesParameters>()))
                    .Callback((ISetCustomerProductAttributeRangesParameters p) => actualValues = p)
                    .Returns(new SuccessResult());

                // act
                SystemUnderTest.Post(custKey, productKey, values);

                // assert
                Assert.IsNotNull(actualValues);
                Assert.AreEqual(expectedUserToken, actualValues.UserToken);
            }

            [Test]
            public void PostMethodValidatesClaim()
            {
                ControllerType.AssertClaimsForMethod(
                    "Post",
                    ClaimResources,
                    ClaimActions.Full);
            }

            [Test]
            public void PostMethodValidatesAntiForgeryToken()
            {
                ControllerType.AssertAntiForgeryTokenValidationForMethod("Post");
            }
        }

        [TestFixture]
        public class DeleteCustomerProductSpecTests : CustomerProductSpecsControllerTests
        {
            [Test]
            public void ParametersAreMappedAsExpected()
            {
                // arrange
                const string customerKey = "1234";
                const string chileProductKey = "asdf";

                IRemoveCustomerChileProductAttributeRangesParameters actualParams = null;
                MockCustomerService.Setup(m => m.RemoveCustomerChileProductAttributeRanges(It.IsAny<IRemoveCustomerChileProductAttributeRangesParameters>()))
                    .Callback((IRemoveCustomerChileProductAttributeRangesParameters p) => actualParams = p)
                    .Returns(new SuccessResult());

                // act
                SystemUnderTest.Delete(customerKey, chileProductKey);

                // assert
                MockCustomerService.Verify(m => m.GetCustomerChileProductAttributeRanges(customerKey, chileProductKey), Times.Once());
                Assert.IsNotNull(actualParams);
                Assert.AreEqual(customerKey, actualParams.CustomerKey);
                Assert.AreEqual(chileProductKey, actualParams.ChileProductKey);
            }
            
            [Test]
            public void DeleteMethodValidatesAntiForgeryToken()
            {
                ControllerType.AssertAntiForgeryTokenValidationForMethod("Delete");
            }

            [Test]
            public void DeleteMethodValidatesClaim()
            {
                ControllerType.AssertClaimsForMethod("Delete", ClaimResources, ClaimActions.Delete);
            }
        }

        [TestFixture]
        public class GetCustomerProductSpecTests : CustomerProductSpecsControllerTests
        {
            [Test]
            public void Tests()
            {
                // arrange
                var expectedResult = AutoFixtureHelper.BuildFixture().Create<ICustomerChileProductAttributeRangesReturn>();
                MockCustomerService.Setup(m => m.GetCustomerChileProductAttributeRanges(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new SuccessResult<ICustomerChileProductAttributeRangesReturn>(expectedResult));

                // act
                SystemUnderTest.Get("1234", "asdf");

                Assert.Pass();
            }
        }
    }
}
