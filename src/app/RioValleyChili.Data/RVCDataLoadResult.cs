using System;
using System.Linq;
using RioValleyChili.Core;

namespace RioValleyChili.Data
{
    public class RVCDataLoadResultObtainer : IDataLoadResultObtainer
    {
        public IDataLoadResult GetDataLoadResult()
        {
            try
            {
                using(var context = new RioValleyChiliDataContext())
                {
                    return context.DataLoadResult.ToList().OrderBy(r => r.Id).LastOrDefault();
                }
            }
            catch(Exception)
            {
                return new LoadResult
                {
                    Success = false,
                };
            }
        }

        public IDataLoadResult SetDataLoadResult(IDataLoadResult value)
        {
            using (var context = new RioValleyChiliDataContext())
            {
                var result = context.DataLoadResult.ToList().OrderBy(r => r.Id).LastOrDefault() ?? context.DataLoadResult.Add(new Models.DataLoadResult());
                result.Success = value.Success;
                result.RanToCompletion = value.RanToCompletion;
                result.TimeStamp = value.TimeStamp;
                context.SaveChanges();
            }
            return value;
        } 

        public class LoadResult : IDataLoadResult
        {
            public bool Success { get; set; }
            public bool RanToCompletion { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}