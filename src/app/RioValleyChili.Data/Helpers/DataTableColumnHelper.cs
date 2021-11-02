using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace RioValleyChili.Data.Helpers
{
    public static class DataTableColumnHelper
    {
        public static List<DataTableColumn> GetDataTableColumns(IEnumerable<EdmProperty> properties)
        {
            return GetDataTableColumns(properties, null);
        }

        private static List<DataTableColumn> GetDataTableColumns(IEnumerable<EdmProperty> properties, DataTableColumn parentColumn)
        {
            var columns = new List<DataTableColumn>();

            foreach(var property in properties)
            {
                Type type = null;

                var primitiveType = property.TypeUsage.EdmType as PrimitiveType;
                if(primitiveType != null)
                {
                    type = primitiveType.ClrEquivalentType;
                }
                else if(property.TypeUsage.EdmType as SimpleType != null)
                {
                    type = typeof(int);
                }

                var columnName = parentColumn != null ? parentColumn.ColumnName + "_" + property.Name : property.Name;
                var column = new DataTableColumn
                    {
                        ColumnName = columnName,
                        PropertyName = property.Name,
                        ParentColumn = parentColumn,
                        Type = type
                    };
                
                if(column.Type != null)
                {
                    columns.Add(column);
                }
                else
                {
                    var complexType = property.TypeUsage.EdmType as ComplexType;
                    if(complexType != null)
                    {
                        columns.AddRange(GetDataTableColumns(complexType.Properties, column));
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("The EdmType '{0}' is not supported.", property.TypeUsage.EdmType.FullName));
                    }
                }
            }

            return columns;
        }

        public class DataTableColumn
        {
            public string ColumnName;

            public string PropertyName;

            public DataTableColumn ParentColumn;

            public Type Type;

            public object GetColumnValue(object root)
            {
                if(ParentColumn != null)
                {
                    root = ParentColumn.GetColumnValue(root);
                }

                var type = root.GetType();
                var property = GetProperty(type, PropertyName);
                if(property == null)
                {
                    throw new Exception(string.Format("The object of type '{0}' does not contain a property '{1}'.", Type.Name, PropertyName));
                }

                return property.GetValue(root);
            }

            private static PropertyInfo GetProperty(Type type, string propertyName)
            {
                if(typeProperties == null)
                {
                    typeProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
                }

                Dictionary<string, PropertyInfo> properties;
                if(!typeProperties.TryGetValue(type, out properties))
                {
                    typeProperties.Add(type, (properties = type.GetProperties().ToDictionary(p => p.Name, p => p)));
                }

                PropertyInfo propertyInfo;
                properties.TryGetValue(propertyName, out propertyInfo);
                return propertyInfo;
            }

            private static Dictionary<Type, Dictionary<string, PropertyInfo>> typeProperties;
        }
    }
}