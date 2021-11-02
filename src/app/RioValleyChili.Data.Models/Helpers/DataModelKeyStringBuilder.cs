using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RioValleyChili.Data.Models.Helpers
{
    public static class DataModelKeyStringBuilder
    {
        public static string BuildKeyString<T>(T dataModel)
            where T : class
        {
            var type = typeof(T);
            var keys = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).ToList();
            if(keys.Count == 0)
            {
                return string.Format("{0} with no key(s) found.", type.Name);
            }
            if(keys.Count == 1)
            {
                return string.Format("{0}", keys[0].GetValue(dataModel));
            }
            keys = keys.OrderBy(p =>
                {
                    var columnAttribute = p.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
                    return columnAttribute == null ? 0 : ((ColumnAttribute) columnAttribute).Order;
                }).ToList();
            return keys.Aggregate("", (s, p) => string.Format("{0}[{1}]", s, p.GetValue(dataModel).ToString()));
        }
    }
}