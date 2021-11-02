using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using RioValleyChili.Data.DataSeeders.Utilities;

namespace RioValleyChili.Data.DataTests
{
    [TestFixture]
    public class DataStringPropertyHelperTests
    {
        public abstract class Base
        {
            [Required, StringLength(5)]
            public string BaseString { get; set; }

            public virtual string LeaveMeAloneString { get; set; }

            [Required]
            public virtual string RequiredString { get; set; }

            public virtual DateTime DateTest { get; set; }

            public TestDataModelChild BaseChild { get; set; }

            public virtual ICollection<TestDataModelChild> BaseChildren { get; set; }
        }

        public class TestDataModel : Base
        {
            public override string LeaveMeAloneString { get; set; }

            [StringLength(4)]
            public virtual string TruncateMe { get; set; }

            public virtual TestDataModelChild Child { get; set; }
        }

        public class TestDataModelChild
        {
            [StringLength(3)]
            public virtual string ChildString { get; set; }
        }

        [Test]
        public void Does_nothing_to_undecorated_string_property()
        {
            const string str = "LEAVE ME ALONE!";
            var testDataModel = new TestDataModel
                {
                    LeaveMeAloneString = str
                };
            var result = DataStringPropertyHelper.ValidateString(testDataModel, m => m.LeaveMeAloneString);
            Assert.AreEqual(str, testDataModel.LeaveMeAloneString);
            Assert.Null(result);
        }

        [Test]
        public void Sets_null_required_string_property_to_empty()
        {
            var testDataModel = new TestDataModel();
            Assert.IsNull(testDataModel.RequiredString);
            var result = DataStringPropertyHelper.ValidateString(testDataModel, m => m.RequiredString);
            Assert.AreEqual("", testDataModel.RequiredString);
            Assert.AreEqual("TestDataModel.RequiredString", result.PropertyName);
            Assert.True(result.SetRequiredToEmpty);
        }

        [Test]
        public void Truncates_string_as_expected()
        {
            var testDataModel = new TestDataModel
                {
                    TruncateMe = "TRUNCATE ME!"
                };
            var result = DataStringPropertyHelper.ValidateString(testDataModel, m => m.TruncateMe);
            Assert.AreEqual("TRUN", testDataModel.TruncateMe);
            Assert.AreEqual("TestDataModel.TruncateMe", result.PropertyName);
            Assert.AreEqual(4, result.TruncatedLength);
        }

        [Test]
        public void Leaves_string_to_truncate_as_null_if_it_is_not_required()
        {
            var testDataModel = new TestDataModel();
            DataStringPropertyHelper.ValidateString(testDataModel, m => m.TruncateMe);
            Assert.IsNull(testDataModel.TruncateMe);
        }

        [Test]
        public void Modifies_base_string_property_as_expected()
        {
            var testDataModel = new TestDataModel
                {
                    BaseString = "BASESTRING"
                };
            var result = DataStringPropertyHelper.ValidateString(testDataModel, m => m.BaseString);
            Assert.AreEqual("BASES", testDataModel.BaseString);
            Assert.AreEqual("TestDataModel.BaseString", result.PropertyName);
            Assert.False(result.SetRequiredToEmpty);
            Assert.AreEqual("BASESTRING", result.TruncatedString);
            Assert.AreEqual(5, result.TruncatedLength);
        }

        [Test]
        public void Returns_truncated_string_results()
        {
            var results = DataStringPropertyHelper.GetTruncatedStrings(new TestDataModel
                {
                    TruncateMe = "TRUNCATE ME!",
                    BaseString = "BASESTRING!"
                });
            Assert.AreEqual(2, results.Count);
            Assert.True(results.Count(r => r.TruncatedString == "TRUNCATE ME!") == 1);
            Assert.True(results.Count(r => r.TruncatedString == "BASESTRING!") == 1);
        }

        [Test]
        public void Iterates_through_object_graph_as_expected()
        {
            var test = new TestDataModel
                {
                    TruncateMe = "TRUNCATE ME!",
                    Child = new TestDataModelChild
                        {
                            ChildString = "AND ME!"
                        }
                };

            var results = DataStringPropertyHelper.GetTruncatedStringsInGraph(test);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("TRUN", test.TruncateMe);
            Assert.AreEqual("AND", test.Child.ChildString);
        }
    }
}