using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RioValleyChili.Client.Mvc.Core;
using RioValleyChili.Core;

namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcCore
{
    [ExtractIntoSolutionheadLibrary]
    public class ReflectionSelectListBuilder : ReflectionSelectListBuilder<object>
    {
        public ReflectionSelectListBuilder(string valueFieldName, string textFieldName)
            : base(valueFieldName, textFieldName) { }
    }
    
    [ExtractIntoSolutionheadLibrary]
    public class ReflectionSelectListBuilder<TSource> : ISelectListBuilder<TSource>, ISelectListBuilder
    {
        #region fields & constructors

        private readonly string _valueFieldName;
        private readonly string _textFieldName;

        public ReflectionSelectListBuilder(string valueFieldName, string textFieldName)
        {
            if (string.IsNullOrWhiteSpace(valueFieldName))
            {
                throw new ArgumentException("The argument valueFieldName is can not be null, empty or white space.");
            }
            _valueFieldName = valueFieldName;

            if (string.IsNullOrWhiteSpace(textFieldName))
            {
                throw new ArgumentException("The argument textFieldName is can not be null, empty or white space.");
            }
            _textFieldName = textFieldName;
        }

        #endregion

        #region Implementation of ISelectListBuilder<TSource>

        public IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable<TSource> source)
        {
            return BuildSelectListItemCollectionFromType(typeof(TSource), source);
        }

        #endregion

        #region Implementation of ISelectListBuilder

        public IEnumerable<SelectListItem> BuildSelectListItemCollection(IEnumerable source)
        {
            var type = source.GetType().GetGenericArguments().Single();
            return BuildSelectListItemCollectionFromType(type, source);
        }

        #endregion

        private IEnumerable<SelectListItem> BuildSelectListItemCollectionFromType(Type type, IEnumerable source)
        {
            var valueProperty = type.GetProperty(_valueFieldName);
            var textProperty = type.GetProperty(_textFieldName);
            var selectList = (from object item in source
                             select new SelectListItem
                                        {
                                            Value = valueProperty.GetValue(item).ToString(),
                                            Text = textProperty.GetValue(item).ToString(),
                                        });

            return selectList;
        }
    }
}