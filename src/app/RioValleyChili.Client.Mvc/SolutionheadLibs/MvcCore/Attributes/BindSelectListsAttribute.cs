using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.SolutionheadLibs.Core;
using RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Helpers;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore.Attributes
{
#warning This filter uses reflection and recursion to find instances of the SelectListSourceAttribute attribute. Is this the best way to handle this? 

    /// <summary>
    /// Binds any properties on the view model, which are decorated with a SelectListSourceAttribute flag, to the specified
    /// SelectListItem data source. Currently, not supported are: nested struct objects and enumerable objects.
    /// </summary>
    [ExtractIntoSolutionheadLibrary]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BindSelectListsAttribute : ActionFilterAttribute
    {
        private const string VIEW_DATA_TAG = "_SelectListOptions";

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model;
            ProcessSelectListSourceAttributes(viewModel, filterContext.Controller);
            base.OnResultExecuting(filterContext);
        }

        private void ProcessSelectListSourceAttributes(object viewModel, ControllerBase controller)
        {
            if (viewModel == null) { return; }
            var viewModelType = viewModel.GetType();
            if (!viewModelType.IsComplexType()) { return; }

            foreach (var property in ViewHelper.GetViewPropertiesForModel(viewModel.GetType()))
            {
                if (RecurseIfClass(controller, property, viewModel)) continue;

                var selectListSourceAttribute = GetCustomAttribute(property, typeof(SelectListSourceAttribute)) as SelectListSourceAttribute;
                if (selectListSourceAttribute != null)
                {
                    BuildDataSourceAndInsertIntoViewData(viewModel, selectListSourceAttribute, property, controller.ViewData);
                }
            }
        }

        private static void BuildDataSourceAndInsertIntoViewData(object viewModel, SelectListSourceAttribute attr, PropertyInfo boundProperty, IDictionary<string, object> viewData)
        {
            if (boundProperty.PropertyType.IsClass && boundProperty.PropertyType != typeof(string))
            {
                throw new ArgumentException(
                    String.Format("The BindSelectListsAttribute does not support binding to a class objects. The property attempting to be bound is '{0}' which is of type '{1}'.",
                        boundProperty.Name,
                        viewModel.GetType().FullName));
            }

            var dataSource = GetDataSource(attr, viewModel);
            var viewDataKey = BuildViewDataKey(boundProperty.Name);

            viewData[viewDataKey] = viewData[viewDataKey] ?? attr.BuildSelectListItems(dataSource);
        }

        public static string BuildViewDataKey(string propertyName)
        {
            return string.Format("{0}{1}", propertyName, VIEW_DATA_TAG);
        }

        private static IEnumerable GetDataSource(SelectListSourceAttribute attribute, object viewModel)
        {
            var dataSourceProperty = viewModel.GetType().GetProperty(attribute.DataSourceName);
            if (dataSourceProperty == null)
            {
                throw new ArgumentException(
                    String.Format(
                        "The type '{0}' does not contain a property named '{1}' as indicated by the SelectListSourceAttribute.",
                        viewModel.GetType().FullName, attribute.DataSourceName));
            }

            var dataSource = dataSourceProperty.GetValue(viewModel);
            if (dataSource == null)
            {
                throw new ArgumentException(string.Format("The data source property '{0}' cannot be null.", attribute.DataSourceName));
            }
            if (dataSource as IEnumerable == null)
            {
                throw new ArgumentException(String.Format("In order to be utilized as a SelectListSource, the property '{0}' must implement IEnumerable.", attribute.DataSourceName));
            }

            return dataSource as IEnumerable;
        }

        private bool RecurseIfClass(ControllerBase controller, PropertyInfo property, object viewModel)
        {
            if (property.PropertyType.IsArray || property.PropertyType.GetInterfaces().Any(i => i.Name == "IEnumerable"))
            {
                return false;
            }

            if (property.PropertyType.IsComplexType())
            {
                ProcessSelectListSourceAttributes(property.GetValue(viewModel), controller);
                return true;
            }

            return false;
        }
    }
}