using System;
using System.Collections.Generic;
using System.Linq;

namespace RioValleyChili.Data.DataSeeders.Utilities.Interfaces
{
    public interface IEmployeeIdentifiableDTO
    {
        int? EmployeeID { get; }
        DateTime? Timestamp { get; }
    }

    public static class IEmployeeIdentifiableDTOExtensions
    {
        public static IEmployeeIdentifiableDTO GetLatest(this List<IEmployeeIdentifiableDTO> list)
        {
            return list.OrderByDescending(e => e.Timestamp.HasValue).ThenByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public static void GetLatest(this List<IEmployeeIdentifiableDTO> list, out int? employeeID, out DateTime? timeStamp)
        {
            employeeID = null;
            timeStamp = null;
            var latest = list.GetLatest();
            if(latest != null)
            {
                employeeID = latest.EmployeeID;
                timeStamp = latest.Timestamp;
            }
        }
    }
}