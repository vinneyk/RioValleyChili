using System;
using System.Linq;
using System.Text.RegularExpressions;
using RioValleyChili.Core;
using RioValleyChili.Core.Extensions;
using RioValleyChili.Data.DataSeeders;
using RioValleyChili.Data.DataSeeders.Serializable;
using RioValleyChili.Data.Models;
using Solutionhead.Data;

namespace RioValleyChili.Services.OldContextSynchronization.Utilities
{
    public static class SynchronizePackScheduleHelper
    {
        private const string DateTimeFormatString = "yyyy/MM/dd HH:mm:ss.fff";

        public static PackSchedule SelectPackScheduleForSynch(this IRepository<PackSchedule> packScheduleRepository, IKey<PackSchedule> packScheduleKey)
        {
            return packScheduleRepository.FindByKey(packScheduleKey,
                p => p.Customer.Company,
                p => p.PackagingProduct.Product,
                p => p.ChileProduct.Product,
                p => p.ProductionBatches.Select(b => b.Production.PickedInventory),
                p => p.ProductionLineLocation,
                p => p.WorkType);
        }

        public static string ToPackSchIdString(this DateTime packSchId)
        {
            return packSchId.ToString(DateTimeFormatString);
        }

        public static DateTime? ToPackSchId(string packSchId)
        {
            try
            {
                return DateTime.ParseExact(packSchId, DateTimeFormatString, null);
            }
            catch
            {
                return null;
            }
        }

        public static void SynchronizeOldContextPackSchedule(tblPackSch oldPackSchedule, PackSchedule newPackSchedule)
        {
            int chileProdId;
            if(!Int32.TryParse(newPackSchedule.ChileProduct.Product.ProductCode, out chileProdId))
            {
                throw new Exception(String.Format("Could not parse chile ProdID[{0}]", newPackSchedule.ChileProduct.Product.ProductCode));
            }
                
            oldPackSchedule.EmployeeID = newPackSchedule.EmployeeId;
            oldPackSchedule.ProductionDeadline = newPackSchedule.ProductionDeadline.GetDate();
            oldPackSchedule.PackSchDate = newPackSchedule.ScheduledProductionDate.Date;
                
            oldPackSchedule.ProdID = chileProdId;
            oldPackSchedule.PackSchDesc = newPackSchedule.SummaryOfWork;
            oldPackSchedule.BatchTypeID = (int?)BatchTypeIDHelper.GetBatchTypeID(newPackSchedule.WorkType.Description);
            oldPackSchedule.ProductionLine = GetProductionLine(newPackSchedule.ProductionLineLocation.Description);
                
            oldPackSchedule.TargetWgt = (decimal?)newPackSchedule.DefaultBatchTargetParameters.BatchTargetWeight;
            oldPackSchedule.TgtAsta = (decimal?)newPackSchedule.DefaultBatchTargetParameters.BatchTargetAsta;
            oldPackSchedule.TgtScan = (decimal?)newPackSchedule.DefaultBatchTargetParameters.BatchTargetScan;
            oldPackSchedule.TgtScov = (decimal?)newPackSchedule.DefaultBatchTargetParameters.BatchTargetScoville;

            oldPackSchedule.OrderNumber = newPackSchedule.OrderNumber;
            oldPackSchedule.Company_IA = newPackSchedule.Customer == null ? null : newPackSchedule.Customer.Company.Name;

            if(oldPackSchedule.tblLots != null)
            {
                foreach(var lot in oldPackSchedule.tblLots)
                {
                    lot.ProdID = oldPackSchedule.ProdID;
                    lot.ProductionDate = oldPackSchedule.PackSchDate;
                    lot.BatchTypeID = oldPackSchedule.BatchTypeID;
                    lot.Company_IA = oldPackSchedule.Company_IA;
                    lot.ProductionLine = oldPackSchedule.ProductionLine;
                }
            }

            oldPackSchedule.Serialized = SerializablePackSchedule.Serialize(newPackSchedule);
        }

        private static readonly Regex LineRegex = new Regex(@"Line.*(\d+)", RegexOptions.IgnoreCase);
        private static int GetProductionLine(string lineDescription)
        {
            try
            {
                var match = LineRegex.Match(lineDescription);
                return Int32.Parse(match.Groups[1].Value);
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("Could not determine production line from [{0}]", lineDescription), ex);
            }
        }
    }
}