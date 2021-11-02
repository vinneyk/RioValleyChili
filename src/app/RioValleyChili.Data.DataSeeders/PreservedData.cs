using System;
using System.Collections.Generic;
using System.Linq;
using RioValleyChili.Data.Models;

namespace RioValleyChili.Data.DataSeeders
{
    public static class PreservedData
    {
        public static void ObtainData(RioValleyChiliDataContext newContext)
        {
            if(newContext == null) { throw new ArgumentNullException("newContext"); }
            Employees = newContext.Employees.ToList();
        }

        public static List<Employee> Employees = new List<Employee>();
    }
}