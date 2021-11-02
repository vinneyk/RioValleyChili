using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace RioValleyChili.Data.Helpers
{
    public interface IBulkInsertContext
    {
        string ConnectionString { get; }
        ObjectContext ObjectContext { get; }
    }

    public static class DbContextExtensions
    {
        public static int BulkAddAll<TEntity>(this IBulkInsertContext context, IEnumerable<TEntity> entities, Action callback = null) where TEntity : class
        {
            var entitiesWithRequiredUser = entities as IList<TEntity> ?? entities.ToList();
            BulkInsert(context, entitiesWithRequiredUser, callback);
            return entitiesWithRequiredUser.Count;
        }

        public static void BulkInsert<TEntity>(this IBulkInsertContext context, IList<TEntity> entities, Action callback = null) where TEntity : class
        {
            // inspired by http://elegantcode.com/2012/01/26/sqlbulkcopy-for-generic-listt-useful-for-entity-framework-nhibernate/
            using(var bulkCopy = new SqlBulkCopy(context.ConnectionString))
            {
                bulkCopy.BulkCopyTimeout = 300;
                var objContext = context.ObjectContext;
                var entityType = objContext.GetEntityType<TEntity>();
                var tableName = objContext.GetTableName<TEntity>();
                
                bulkCopy.BatchSize = entities.Count;
                bulkCopy.DestinationTableName = tableName;
                
                var columns = DataTableColumnHelper.GetDataTableColumns(entityType.Properties);
                var table = BuildDataTable(bulkCopy, columns);
                LoadDataTable(entities, columns, table, callback);

                bulkCopy.WriteToServer(table);
            }
        }

        public static EntityType GetEntityType<T>(this ObjectContext context) where T : class
        {
            var entities = context.MetadataWorkspace.GetItems(DataSpace.CSpace).Where(b => b.BuiltInTypeKind == BuiltInTypeKind.EntityType);
            var entityName = typeof(T).Name;
            // for some reason, the item object's namespace is RioValleyChili.Data; that's why we're using Name rather than FullName.
            return entities.Cast<EntityType>().FirstOrDefault(item => item.Name == entityName);
        }

        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            var sql = context.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex("FROM (?<table>.*) AS");
            var match = regex.Match(sql);

            var table = match.Groups["table"].Value;
            return table;
        }
        
        private static DataTable BuildDataTable(SqlBulkCopy bulkCopy, IEnumerable<DataTableColumnHelper.DataTableColumn> dataTableColumns)
        {
            var table = new DataTable();

            foreach(var dataTableColumn in dataTableColumns)
            {
                bulkCopy.ColumnMappings.Add(dataTableColumn.ColumnName, dataTableColumn.ColumnName);
                table.Columns.Add(dataTableColumn.ColumnName, dataTableColumn.Type);
            }
            return table;
        }

        private static void LoadDataTable<TEntity>(IEnumerable<TEntity> entities, IReadOnlyList<DataTableColumnHelper.DataTableColumn> columns, DataTable table, Action callback = null) where TEntity : class
        {
            var values = new object[columns.Count];

            var userDateTimeSetter = new RequiredUserDateTimeSetter("DataInitialize", "User");
            foreach(var entity in entities)
            {
                if(callback != null) { callback(); }

                userDateTimeSetter.SetRequiredUser(entity);

                for(var c = 0; c < columns.Count; ++c)
                {
                    values[c] = columns[c].GetColumnValue(entity);
                }

                table.Rows.Add(values);
            }
        }
    }
}