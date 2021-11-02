using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Telerik.Reporting;
using Telerik.Reporting.Expressions;

namespace RioValleyChili.Client.Reporting
{
    public interface IEntityReport<out TReportModel> { }

    public static class EntityReportExtensions
    {
        public static void Bind<TReportModel, TReportItem, TBind>(this IEntityReport<TReportModel> report, TReportItem reportItem, Expression<Func<TReportItem, TBind>> member, Expression<Func<TReportModel, TBind>> value, string formatString = null)
            where TReportItem : ReportItemBase
        {
            reportItem.Bindings.Add(new Binding(MemberAccessPathBuilder.GetPath(member), Field(null, value, formatString)));
        }

        public static string Field<TReportModel, TMember>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, TMember>> memberAccessExpression, string formatString = null)
        {
            return string.IsNullOrWhiteSpace(formatString)
                ? BuildFieldBinding(memberAccessExpression)
                : _Format(memberAccessExpression, formatString);
        }

        public static string Format<TReportModel>(this IEntityReport<TReportModel> report, string formatString, params string[] values)
        {
            if (values == null || values.Length == 0) throw new Exception("Expression parameters are required.");

            var paramsString = string.Join(", ", values);
            return string.Format("=Format('{0}', {1})", formatString, paramsString); // todo: clean formatString 
        }

        public static string PartialFormat<TReportModel>(this IEntityReport<TReportModel> report, string formatString, params string[] values)
        {
            if (values == null || values.Length == 0) throw new Exception("Expression parameters are required.");

            var paramsString = string.Join(", ", values);
            return string.Format("Format('{0}', {1})", formatString, paramsString); // todo: clean formatString 
        }

        public static string JoinField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, object>> expression, string separator)
        {
            return string.Format("=Join('{1}', {0})", PartialField(report, expression), separator);
        }

        #region SumofField

        public static string SumOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, int>> expression, string formatString = null)
        {
            return SumExpression(expression, formatString);
        }

        public static string SumOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, short>> expression, string formatString = null)
        {
            return SumExpression(expression, formatString);
        }

        public static string SumOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, double>> expression, string formatString = null)
        {
            return SumExpression(expression, formatString);
        }

        public static string SumOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, decimal>> expression, string formatString = null)
        {
            return SumExpression(expression, formatString);
        }

        public static string SumOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, long>> expression, string formatString = null)
        {
            return SumExpression(expression, formatString);
        }

        #endregion

        #region CountOfField

        public static string CountOfField<TReportModel>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, double>> expression, string formatString = null)
        {
            return CountExpression(expression, formatString);
        }

        #endregion

        public static string ChildMemberField<TReportModel, TParent>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, TParent>> parentMember, Expression<Func<TParent, object>> childMember, string formatString = null)
        {
            return string.IsNullOrWhiteSpace(formatString) 
                ? BuildFieldBinding(childMember)
                : _Format(childMember, formatString);
        }

        public static string ChildMemberField<TReportModel, TParent>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, IEnumerable<TParent>>> parentMember, Expression<Func<TParent, object>> childMember, string formatString = null)
        {
            return string.IsNullOrWhiteSpace(formatString) 
                ? BuildFieldBinding(childMember)
                : _Format(childMember, formatString);
        }

        public static TOutput ReadDataObjectProperty<TOutput>(object sender, string propertyName) where TOutput : class
        {
            var dataObject = (Telerik.Reporting.Processing.IDataObject)sender;
            return dataObject[propertyName] as TOutput;
        }

        public static TOutput ReadDataObjectProperty<TInput, TOutput>(object sender, Expression<Func<TInput, TOutput>> expression)
            where TOutput : class
            where TInput : class
        {
            return ReadDataObjectProperty<TOutput>(sender, MemberAccessPathBuilder.GetPath(expression));
        }

        public static string PartialField<TReportModel, TMember>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, TMember>> expression)
        {
            return string.Format("Fields.{0}", MemberAccessPathBuilder.GetPath(expression));
        }

        public static string PartialField<TReportModel, TMember>(this IEntityReport<TReportModel> report, Expression<Func<TReportModel, TMember>> expression, string formatString)
        {
            return _Format(expression, formatString, true);
        }

        [Function(Category = "My Functions", Namespace = "EntityReportExtensions")]
        public static ReportSource GetSubReportSource(SubReport subReport, object model)
        {
            InstanceReportSource reportSource = null;
            Dictionary<string, Func<object, Report>> subReportDelegates;
            var report = subReport.Report;
            if(ReportDelegates.TryGetValue(report, out subReportDelegates))
            {
                Func<object, Report> subReportDelegate;
                if(subReportDelegates.TryGetValue(subReport.Name, out subReportDelegate))
                {
                    subReportDelegates.Remove(subReport.Name);
                    if(!subReportDelegates.Any())
                    {
                        ReportDelegates.Remove(report);
                    }

                    var reportDocument = subReportDelegate(model);
                    reportSource = new InstanceReportSource
                        {
                            ReportDocument = reportDocument
                        };
                }
            }
            
            return reportSource;
        }

        public static SubReportBinder<TReportModel, TSubReportModel> Bind<TReportModel, TSubReportModel>(this IEntityReport<TReportModel> report, SubReport subreport, Func<TReportModel, TSubReportModel> source)
        {
            return new SubReportBinder<TReportModel, TSubReportModel>(subreport, source);
        }

        public class SubReportBinder<TReportModel, TSubReportModel>
        {
            private readonly SubReport _subReport;
            private readonly Func<TReportModel, TSubReportModel> _source;

            public SubReportBinder(SubReport subReport, Func<TReportModel, TSubReportModel> source)
            {
                _subReport = subReport;
                _source = source;
            }

            public void SubReport<TSubReport>(params Action<TSubReport>[] initializers)
                where TSubReport : Report, IEntityReport<TSubReportModel>, new()
            {
                Dictionary<string, Func<object, Report>> subReportDelegates;
                var report = _subReport.Report;
                if(!ReportDelegates.TryGetValue(report, out subReportDelegates))
                {
                    ReportDelegates.Add(report, subReportDelegates = new Dictionary<string, Func<object, Report>>());
                }

                if(!subReportDelegates.ContainsKey(_subReport.Name))
                {
                    subReportDelegates.Add(_subReport.Name, o =>
                    {
                        var subreport = new TSubReport
                        {
                            Name = string.Format(_subReport.Name),
                            DataSource = o == null ? default(TSubReportModel) : _source((TReportModel)o)
                        };
                        if(initializers != null)
                        {
                            initializers.ToList().ForEach(i => i(subreport));
                        }
                        return subreport;
                    });
                }

                _subReport.Bindings.Add(new Binding("ReportSource", "=EntityReportExtensions.GetSubReportSource(ReportItem.ItemDefinition, ReportItem.DataObject.RawData)"));
            }
        }

        private static readonly Dictionary<object, Dictionary<string, Func<object, Report>>> ReportDelegates = new Dictionary<object, Dictionary<string, Func<object, Report>>>();

        public static ReportTable<TReportModel, TTableModel> Table<TReportModel, TTableModel>(this IEntityReport<TReportModel> report, Table table, Expression<Func<TReportModel, TTableModel>> dataSourceExpression)
        {
            return new ReportTable<TReportModel, TTableModel>(table, dataSourceExpression);
        }

        public static ReportTable<TReportModel, TTableModel> Table<TReportModel, TTableModel>(this IEntityReport<TReportModel> report, Table table, Expression<Func<TReportModel, IEnumerable<TTableModel>>> dataSourceExpression)
        {
            return new ReportTable<TReportModel, TTableModel>(table, dataSourceExpression);
        }

        public class ReportTable<TReportModel, TTableModel> : IEntityReport<TTableModel>
        {
            private readonly Table _table;

            public ReportTable(Table table, Expression<Func<TReportModel, TTableModel>> dataSourceExpression)
            {
                _table = table;
                _table.Bindings.Add(new Binding("DataSource", Field(null, dataSourceExpression)));
            }

            public ReportTable(Table table, Expression<Func<TReportModel, IEnumerable<TTableModel>>> dataSourceExpression)
            {
                _table = table;
                _table.Bindings.Add(new Binding("DataSource", Field(null, dataSourceExpression)));
            }

            public ReportTable<TReportModel, TTableModel> AddSort<TSort>(Expression<Func<TTableModel, TSort>> memberAccessExpression, SortDirection sortDirection = SortDirection.Asc)
            {
                _table.Sortings.Add(this.Field(memberAccessExpression), sortDirection);
                return this;
            }

            public ReportTable<TReportModel, TTableModel> With(params Action<IEntityReport<TTableModel>>[] options)
            {
                if(options != null)
                {
                    foreach(var option in options)
                    {
                        option(this);
                    }
                }
                return this;
            }
        }

        public static TextBox Trimmed(this TextBox textBox)
        {
            textBox.ItemDataBound += TrimTextDelegate;
            return textBox;
        }

        #region Private Parts

        private static string _Format<TIn, TOut>(Expression<Func<TIn, TOut>> memberAccessExpression, string formatString, bool partial = false)
        {
            return _Format(MemberAccessPathBuilder.GetPath(memberAccessExpression), formatString, partial);
        }

        private static string _Format(string memberPath, string formatString, bool partial = false)
        {
            return string.Format("{2}Format('{0}', Fields.{1})", formatString, memberPath, partial ? "" : "=");
        }

        private static string BuildFieldBinding<TModel, TMember>(Expression<Func<TModel, TMember>> memberAccessExpression)
        {
            var path = MemberAccessPathBuilder.GetPath(memberAccessExpression);
            return string.IsNullOrWhiteSpace(path) ? "Empty Path!" : string.Format("=Fields.{0}", path);
        }

        private static string SumExpression<TReportModel, TOut>(Expression<Func<TReportModel, TOut>> expression, string formatString)
        {
            var sum = string.Format("Sum(Fields.{0})", MemberAccessPathBuilder.GetPath(expression));
            return string.IsNullOrWhiteSpace(formatString)
                       ? string.Format("={0}", sum)
                       : _Format(sum, formatString);
        }

        private static string CountExpression<TReportModel, TOut>(Expression<Func<TReportModel, TOut>> expression, string formatString)
        {
            var sum = string.Format("Count(Fields.{0})", MemberAccessPathBuilder.GetPath(expression));
            return string.IsNullOrWhiteSpace(formatString)
                       ? string.Format("={0}", sum)
                       : _Format(sum, formatString);
        } 

        private static void TrimTextDelegate(object sender, EventArgs eventArgs)
        {
            var box = sender as Telerik.Reporting.Processing.TextBox;
            if(box != null)
            {
                using(var graphics = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    var boxWidth = box.Width.ToPixels() - (box.Style.Padding.Left.ToPixels() + box.Style.Padding.Right.ToPixels());
                    var font = new Font(box.Style.Font.Name, box.Style.Font.Size.ToPoint().Value, box.Style.Font.Style, GraphicsUnit.Point);
                    while(graphics.MeasureString(box.Text, font).Width > boxWidth)
                    {
                        box.Value = box.Text.Substring(0, box.Text.Length - 1);
                    }
                }
            }
        }

        #endregion
    }
}