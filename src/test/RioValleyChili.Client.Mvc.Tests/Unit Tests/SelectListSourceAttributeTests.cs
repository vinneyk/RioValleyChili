using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using NUnit.Framework;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Tests.TestObjects.Models;

namespace RioValleyChili.Tests.Unit_Tests
{
    [TestFixture]
    class SelectListSourceAttributeTests
    {   
        [Test]
        public void SetsTargetPropertyNameAsExpected()
        {
            // Arrange + Act
            var obj = new SampleViewModel();
            var attr = obj.GetSelectListAttribute();

            // Assert
            Assert.IsNotNull(attr);
            Assert.AreEqual("SelectListItems", attr.DataSourceName);
        }

        [Test]
        public void WhenOptionalLabelIsNotSupplied_NoLabelItemIsAdded()
        {
            // Arrange
            var classUnderTest = new SelectListSourceAttribute("DataSourceProp", "Id", "Text");
            var sourceItems = Builder<SelectListItemObject>.CreateListOfSize(3).Build();
            
            // Act
            var results = classUnderTest.BuildSelectListItems(sourceItems).ToList();

            // Assert
            Assert.AreEqual(sourceItems.Count, results.Count);
        }

        [Test]
        public void WhenOptionalLabelIsEmptyString_LabelItemIsAdded()
        {
            // Arrange
            var classUnderTest = new SelectListSourceAttribute("DataSourceProp", "Id", "Text", "");
            var sourceItems = Builder<SelectListItemObject>.CreateListOfSize(3).Build();
            
            // Act
            var results = classUnderTest.BuildSelectListItems(sourceItems).ToList();

            // Assert
            Assert.AreEqual(sourceItems.Count + 1, results.Count);
            Assert.AreEqual("", results[0].Text);
            Assert.AreEqual("", results[0].Value);
        }

        [Test]
        public void WhenOptionalLabelIsNotEmptyString_LabelItemIsAdded()
        {
            // Arrange
            const string expectedLabelText = "HELLO!";
            var classUnderTest = new SelectListSourceAttribute("DataSourceProp", "Id", "Text", expectedLabelText);
            var sourceItems = Builder<SelectListItemObject>.CreateListOfSize(3).Build();

            // Act
            var results = classUnderTest.BuildSelectListItems(sourceItems).ToList();

            // Assert
            Assert.AreEqual(sourceItems.Count + 1, results.Count);
            Assert.AreEqual(expectedLabelText, results[0].Text);
            Assert.AreEqual("", results[0].Value);
        }

        public class SampleViewModel
        {
            internal const string LABEL_TEXT = "Select one...";
            public SampleViewModel()
            {
                Prop1 = "Property 1";
                Prop2 = "Property 2";
                SelectListItems = new List<SelectListItemObject>
                                  {
                                      new SelectListItemObject
                                          {
                                              Id = 1,
                                              Text = "First"
                                          },
                                      new SelectListItemObject
                                          {
                                              Id = 2,
                                              Text = "Second"
                                          }
                                  };
            }

            [SelectListSource("SelectListItems", "Id", "Text")]
            public string Prop1 { get; set; }

            [SelectListSource("SelectListItems", "Id", "Text", LABEL_TEXT)]
            public string Prop2 { get; set; }

            public IEnumerable<SelectListItemObject> SelectListItems { get; set; }

            internal SelectListSourceAttribute GetSelectListAttribute()
            {
                var property = GetType().GetProperties().First(p => p.IsDefined(typeof(SelectListSourceAttribute), false));
                return property.GetCustomAttributes(typeof(SelectListSourceAttribute), false).First() as SelectListSourceAttribute;
            }
        }
    }
}