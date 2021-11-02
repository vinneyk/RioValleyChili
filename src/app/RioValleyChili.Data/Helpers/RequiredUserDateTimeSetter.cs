using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RioValleyChili.Data.Helpers
{
    [Obsolete("This should eventually be removed when all relevant data models are derived from EmployeeIdentifiable and their Employee/TimeStamp settings properly set. - RI 2014/02/26")]
    public class RequiredUserDateTimeSetter
    {
        public string User;
        public string PropertyName;

        private readonly DateTime _minTimestamp = new DateTime(1753, 1, 1);

        public RequiredUserDateTimeSetter(string user, string propertyName)
        {
            if(string.IsNullOrEmpty(user)) { throw new ArgumentNullException("user"); }
            User = user;

            if(string.IsNullOrEmpty(propertyName)) { throw new ArgumentNullException("propertyName"); }
            PropertyName = propertyName;
        }

        public TEntity SetRequiredUser<TEntity>(TEntity entity)
        {
            foreach(var user in GetRequiredUserProperties<TEntity>())
            {
                var value = (string) user.GetValue(entity);
                if(string.IsNullOrEmpty(value))
                {
                    user.SetValue(entity, User);
                }
            }
                
            foreach(var datetime in GetDateTimeProperties<TEntity>())
            {
                var value = (DateTime)datetime.GetValue(entity);
                if(value == null || value < _minTimestamp)
                {
                    datetime.SetValue(entity, _minTimestamp);
                }
            }

            return entity;
        }

        private static List<PropertyInfo> GetRequiredUserProperties<TEntity>()
        {
            var key = typeof(TEntity);
            List<PropertyInfo> requiredUserProperties;
            if(!RequiredUserProperties.TryGetValue(key, out requiredUserProperties))
            {
                requiredUserProperties = key.GetProperties().Where(p => p.PropertyType == typeof(string) && p.Name == "User" && p.CustomAttributes.Any(a => a.AttributeType == typeof(RequiredAttribute))).ToList();
                RequiredUserProperties.Add(key, requiredUserProperties);
            }
            return requiredUserProperties;
        }

        private static List<PropertyInfo> GetDateTimeProperties<TEntity>()
        {
            var key = typeof(TEntity);
            List<PropertyInfo> dateTimeProperties;
            if(!DateTimeProperties.TryGetValue(key, out dateTimeProperties))
            {
                dateTimeProperties = key.GetProperties().Where(p => p.PropertyType == typeof(DateTime)).ToList();
                DateTimeProperties.Add(key, dateTimeProperties);
            }
            return dateTimeProperties;
        }

        private static readonly Dictionary<Type, List<PropertyInfo>> RequiredUserProperties = new Dictionary<Type, List<PropertyInfo>>();
        private static readonly Dictionary<Type, List<PropertyInfo>> DateTimeProperties = new Dictionary<Type, List<PropertyInfo>>();
    }
}
