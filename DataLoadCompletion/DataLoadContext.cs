using System.Data.Entity;
using RioValleyChili.Data.Models;

namespace DataLoadCompletion
{
    public class DataLoadContext : DbContext
    {
        public DbSet<DataLoadResult> DataLoadResult { get; set; }
    }
}