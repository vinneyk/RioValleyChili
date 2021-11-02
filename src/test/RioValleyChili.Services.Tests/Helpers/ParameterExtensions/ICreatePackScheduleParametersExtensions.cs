using NUnit.Framework;
using RioValleyChili.Business.Core.Keys;
using RioValleyChili.Data.Models;
using RioValleyChili.Services.Interfaces.Parameters.PackScheduleService;
using RioValleyChili.Services.Tests.Helpers.DataModelExtensions;

namespace RioValleyChili.Services.Tests.Helpers.ParameterExtensions
{
    internal static class ICreatePackScheduleParametersExtensions
    {
        internal static void AssertAsExpected(this ICreatePackScheduleParameters parameters, PackSchedule packSchedule)
        {
            Assert.AreEqual(parameters.UserToken, packSchedule.Employee.UserName);
            Assert.AreEqual(parameters.WorkTypeKey, new WorkTypeKey(packSchedule).KeyValue);
            Assert.AreEqual(parameters.ChileProductKey, new ChileProductKey(packSchedule).KeyValue);
            Assert.AreEqual(parameters.PackagingProductKey, new PackagingProductKey(packSchedule).KeyValue);
            Assert.AreEqual(parameters.ProductionLineKey, new LocationKey(packSchedule).KeyValue);
            Assert.AreEqual(parameters.ScheduledProductionDate, packSchedule.ScheduledProductionDate);
            Assert.AreEqual(parameters.ProductionDeadline, packSchedule.ProductionDeadline);
            Assert.AreEqual(parameters.SummaryOfWork, packSchedule.SummaryOfWork);
            if(parameters.CustomerKey == null)
            {
                Assert.IsNull(packSchedule.CustomerId);
                Assert.IsNull(packSchedule.Customer);
            }
            else
            {
                Assert.AreEqual(parameters.CustomerKey, new CustomerKey(packSchedule).KeyValue);
            }
            Assert.AreEqual(parameters.OrderNumber, packSchedule.OrderNumber);
            parameters.AssertAsExpected(packSchedule.DefaultBatchTargetParameters);
        }
    }
}
