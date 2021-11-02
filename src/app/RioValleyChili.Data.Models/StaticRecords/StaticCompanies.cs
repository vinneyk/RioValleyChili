using System;
using System.Collections.Generic;
using RioValleyChili.Core;

namespace RioValleyChili.Data.Models.StaticRecords
{
    public static class StaticCompanies
    {
        static StaticCompanies()
        {
            RVCBroker = new Company
                {
                    Id = 0,
                    EmployeeId = 100,
                    TimeStamp = DateTime.UtcNow,

                    Name = "RVC HOUSE ACCOUNTS",
                    Active = true,
                    CompanyTypes = new List<CompanyTypeRecord>
                        {
                            new CompanyTypeRecord
                                {
                                    CompanyId = 0,
                                    CompanyTypeEnum = CompanyType.Broker
                                }
                        }
                };
        }

        /// <summary>
        /// RVC HOUSE ACCOUNTS - CompanyId and resulting Key should not be assumed to be valid, filter by Name instead.
        /// </summary>
        public static Company RVCBroker { get; private set; }
    }
}