using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public interface IDataBoundUIBuilder<TModel>
    {
        ViewDataDictionary<TModel> ViewData { get; }

        MvcHtmlString BoundFieldSet(string expression, string dataBindingArgument = null);

        MvcHtmlString BoundFieldSetFor<TValue>(Expression<Func<TModel, TValue>> expression);

        MvcHtmlString BoundFieldSetFor<TValue>(Expression<Func<TModel, TValue>> expression, string dataBoundArgument);
        
        MvcHtmlString BoundEditor(string expression, string dataBoundArgument = null);
        
        MvcHtmlString BoundEditorFor<TValue>(Expression<Func<TModel, TValue>> expression, string dataBindingArguments = null);

        MvcHtmlString BoundEditorViewFor<TValue>(Expression<Func<TModel, TValue>> expression);
        MvcHtmlString BoundFieldsetDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression);
        MvcHtmlString BoundTemplatedDisplayFor<TValue>(Expression<Func<TModel, TValue>> expression);
    }
}