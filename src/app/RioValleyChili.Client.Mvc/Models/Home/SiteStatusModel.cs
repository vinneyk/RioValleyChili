using System;
using System.ComponentModel.DataAnnotations;

namespace RioValleyChili.Client.Mvc.Models.Home
{
    public class SiteStatusModel
    {
        [Display(Name = "Last Data Migration Date"), DisplayFormat(DataFormatString = "{0:r}", NullDisplayText = "No known successful data migration")]
        public DateTime? LastDataMigrationTimeStamp { get; set; }

        [Display(Name = "New DB Connected (RvcData)")]
        public bool RvcDataDatabaseConnected { get; set; }

        [Display(Name = "Old DB Connected (RioAccessSQL)")]
        public bool RioAccessSqlDatabaseConnected { get; set; }

        [Display(Name = "Membership DB Connected (RvcMemberStore)")]
        public bool RvcMemberStoreDatabaseConnected { get; set; }

        public bool KillswitchEngaged { get; set; }

        public DateTime? KillswitchEngagedTimeStamp { get; set; }
    }
}