using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.Core.Filters
{
    [ExtractIntoSolutionheadLibrary]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SelectListSourceAttribute : UIHintAttribute
    {
        #region fields and constructors

        private const string DEFAULT_TEMPLATE_NAME = "SelectList";
        private readonly string _dataSourcePropertyName;
        private readonly ISelectListBuilder _selectListBuilder;
        private readonly string _optionalLabel;

        /// <summary>
        /// Identifies the property as a select list bound object. This will result in rendering a select list UI element by default.
        /// </summary>
        /// <param name="dataSourcePropertyName">The name of the property which will provide the data source for this object.</param>
        /// <param name="selectListBuilderType">The SelectListBuilder type to use for the projection of the property's data into a new collection of SelectListItems.</param>
        /// <param name="optionalLabel">Inserts an additional select list item with an empty value at index 0 of the list. If null, no item is inserted.</param>
        /// <param name="templateName">The optional name of the view to override the default view.</param>
        public SelectListSourceAttribute(string dataSourcePropertyName, Type selectListBuilderType, string optionalLabel = null, string templateName = DEFAULT_TEMPLATE_NAME)
            : this(dataSourcePropertyName, ActivateSelectListBuilder(selectListBuilderType), optionalLabel, templateName) { }

        private static ISelectListBuilder ActivateSelectListBuilder(Type selectListBuilderType)
        {
            if (selectListBuilderType == null) { throw new ArgumentNullException("selectListBuilderType"); }
            var selectListBuilder = Activator.CreateInstance(selectListBuilderType) as ISelectListBuilder;
            if (selectListBuilder == null)
            {
                throw new ArgumentException(
                    "The selectListBuilderType is not valid. SelectListSourceAttribute only supports objects implementing ISelectListBuilder at this time.");
            }

            return selectListBuilder;
        }

        /// <summary>
        /// Identifies the property as a select list bound object. This will result in rendering a select list UI element by default.
        /// </summary>
        /// <param name="dataSourcePropertyName">The name of the property which will provide the data source for this object.</param>
        /// <param name="valueName">The name of the property to project into the Value property of the resulting SelectListItem.</param>
        /// <param name="textName">The name of the property to project into the Text property of the resulting SelectListItem.</param>
        /// <param name="optionalLabel">Inserts an additional select list item with an empty value at index 0 of the list. If null, no item is inserted.</param>
        /// <param name="templateName">The optional name of the view to override the default view.</param>
        public SelectListSourceAttribute(string dataSourcePropertyName, string valueName, string textName, string optionalLabel = null, string templateName = DEFAULT_TEMPLATE_NAME)
            : this(dataSourcePropertyName, new ReflectionSelectListBuilder(valueName, textName), optionalLabel, templateName) { }

        private SelectListSourceAttribute(string dataSourcePropertyName, ISelectListBuilder selectListBuilder, string optionalLabel, string templateName)
            : base(templateName)
        {
            if (selectListBuilder == null) { throw new ArgumentNullException("selectListBuilder"); }

            _dataSourcePropertyName = dataSourcePropertyName;
            _selectListBuilder = selectListBuilder;
            _optionalLabel = optionalLabel;
        }

        #endregion

        #region Properties

        public string DataSourceName
        {
            get { return _dataSourcePropertyName; }
        }

        public string OptionalLabel
        {
            get { return _optionalLabel; }
        }

        #endregion

        public IEnumerable<SelectListItem> BuildSelectListItems(IEnumerable sourceData)
        {
            var selectList = _selectListBuilder.BuildSelectListItemCollection(sourceData).ToList();
            if (OptionalLabel != null)
            {
                selectList.Insert(0, new SelectListItem
                                         {
                                             Text = OptionalLabel,
                                             Value = string.Empty,
                                         });
            }

            return selectList;
        }
    }
}