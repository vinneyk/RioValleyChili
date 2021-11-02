//using System;
//using System.Data.Entity.Core.Metadata.Edm;
//using System.Data.Entity.Core.Objects.DataClasses;
//using System.Reflection;
//using NUnit.Framework;
//using RioValleyChili.Data.DataSeeders;
//using RioValleyChili.Data.Models;
//using RioValleyChili.Services.Tests.Helpers;
//using Solutionhead.TestFoundations.Utilities;

//namespace RioValleyChili.Services.Tests.IntegrationTests
//{
//    [TestFixture]
//    public class DataContextTests 
//    {
//        [Test]
//        public void Create_object_graphs_for_all_types_of_entities_in_database()
//        {
//            object objectGraph = null;
//            RVCIntegrationTestHelper testHelper = null;
//            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
//            var createMethod = typeof(RVCIntegrationTestHelper).GetMethod("CreateObjectGraph", bindingFlags);
//            var insertMethod = typeof(RVCIntegrationTestHelper).GetMethod("InsertObjectGraphIntoDatabase", bindingFlags);
//            try
//            {
//                testHelper = new RVCIntegrationTestHelper();
//                foreach(var entity in testHelper.EntityParser.Entities)
//                {
//                    if(entity.Type == typeof(CompanyDepth)) { continue; }

//                    objectGraph = createMethod.MakeGenericMethod(entity.Type).Invoke(testHelper, new object[] { null });
//                    insertMethod.MakeGenericMethod(entity.Type).Invoke(testHelper, new[] { objectGraph, false, false });
//                }
//                testHelper.DisposeOfContext();
//            }
//            catch(Exception ex)
//            {
//                while(ex.InnerException != null)
//                {
//                    ex = ex.InnerException;
//                }
//                if(testHelper != null)
//                {
//                    testHelper.DisposeOfContext();
//                }
//                Assert.Fail("When attempting to insert '{0}', the following exception occured:\n{1}", objectGraph == null ? "[objectGraph is null]" : objectGraph.GetType().Name, ex.Message);
//            }
//        }

//        [Test]
//        public void OldContextTest()
//        {
//            //var tblLot = new tblLot();
//            //tblLot.tblBatchItem = new EntityCollection<tblBatchItem>
//            //    {
//            //        new tblBatchItem
//            //            {
//            //                tblLot = new tblLot()
//            //            }
//            //    };

//            //var inst = new ObjectInstantiator();
//            //var tblLot2 = inst.InstantiateObject<tblLot>();
//            var objectContext = new RioAccessSQLEntities();
//            var items = objectContext.MetadataWorkspace.GetItems(DataSpace.OSpace);
//            var test = typeof(int).GetProperty("");
//            //Delegate.CreateDelegate(test.GetGetMethod())

//            //var helper = new RioAccessSQLEntities();
//            //var lot = helper.CreateObjectGraphAndInsertIntoDatabase<tblLot>(false, false);
//        }
//    }
//}