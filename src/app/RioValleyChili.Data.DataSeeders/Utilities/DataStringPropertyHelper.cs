using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RioValleyChili.Business.Core.Helpers;

namespace RioValleyChili.Data.DataSeeders.Utilities
{
    public static class DataStringPropertyHelper
    {
        public static List<Result> ValidateStrings<TDataModel>(TDataModel dataModel, params Expression<Func<TDataModel, string>>[] stringSelectors)
            where TDataModel : class
        {
            return stringSelectors.Select(s => ValidateString(dataModel, s)).Where(r => r != null).ToList();
        }

        public static Result ValidateString<TDataModel>(TDataModel dataModel, Expression<Func<TDataModel, string>> stringSelector)
            where TDataModel : class
        {
            var propertyInfo = GetPropertyInfo(stringSelector);
            var stringProperty = GetStringProperty<TDataModel>(propertyInfo);
            return stringProperty != null ? stringProperty.Process(dataModel) : null;
        }

        public static List<Result> GetTruncatedStrings<TDataModel>(TDataModel dataModel)
            where TDataModel : class
        {
            return GetStringProperties<TDataModel>().Select(s => s.Process(dataModel)).Where(r => r.TruncatedLength >= 0).ToList();
        }
        
        public static List<Result> GetTruncatedStringsInGraph<TDateModel>(TDateModel dataModel)
            where TDateModel : class
        {
            if(dataModel == null)
            {
                return new List<Result>();
            }

            _processedNodes = new HashSet<object>();
            _results = new List<Result>();
            try
            {
                _GetTruncatedStringsInGraph(dataModel);
            }
            catch(Exception ex)
            {
                Console.WriteLine("DateStringPropertyHelper failed @ object[{0}] Exception[{1}]", _lastObject == null ? "null" : _lastObject.GetType().Name, ex.Message);
                throw;
            }
            _processedNodes.Clear();
            _processedNodes = null;

            var @return = _results.ToList();
            _results.Clear();
            _results = null;
            _lastObject = null;

            return @return;
        }

        public class Result
        {
            public string PropertyName = null;
            public bool SetRequiredToEmpty = false;
            public string TruncatedString = null;
            public int TruncatedLength = -1;
        }

        private static readonly Dictionary<PropertyInfo, IStringProperty> StringProperties = new Dictionary<PropertyInfo, IStringProperty>();
        private static readonly Dictionary<Type, List<IStringProperty>> TypeStringProperties = new Dictionary<Type, List<IStringProperty>>();
        private static readonly Dictionary<Type, List<Func<object, object>>> TypeChildrenGetters = new Dictionary<Type, List<Func<object, object>>>();
        private static readonly Dictionary<Type, Action<object>> TypeGetTruncatedStringInGraphDelegates = new Dictionary<Type, Action<object>>();
        private static readonly MethodInfo GetTruncatedMethodInfo = typeof(DataStringPropertyHelper).GetMethod("_GetTruncatedStringsInGraph", BindingFlags.NonPublic | BindingFlags.Static);

        private static HashSet<object> _processedNodes;
        private static List<Result> _results;
        private static object _lastObject;

        private static void _GetTruncatedStringsInGraph<TNode>(TNode node)
            where TNode : class
        {
            _lastObject = node;
            if(_processedNodes.Contains(node))
            {
                return;
            }

            _processedNodes.Add(node);
            _results.AddRange(GetTruncatedStrings(node));

            foreach(var getChild in GetChildrenGetters<TNode>())
            {
                var child = getChild(node);
                if(child != null)
                {
                    var enumerableChildren = child as IEnumerable;
                    if(enumerableChildren != null)
                    {
                        foreach(var enumeratedChild in enumerableChildren)
                        {
                            if(enumeratedChild != null)
                            {
                                var del = GetGraphDelegate(enumeratedChild);
                                if(del != null)
                                {
                                    del(enumeratedChild);
                                }
                            }
                        }
                    }
                    else
                    {
                        var del = GetGraphDelegate(child);
                        if(del != null)
                        {
                            del(child);
                        }
                    }
                }
            }
        }

        const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

        private static List<Func<object, object>> GetChildrenGetters<TNode>()
        {
            var type = CachedType<TNode>.Type;
            List<Func<object, object>> getters;
            if(!TypeChildrenGetters.TryGetValue(type, out getters))
            {
                getters = type.GetProperties(bindingFlags).Where(p => p.GetGetMethod(false) != null && ValidMemberTypeNode(p.PropertyType))
                        .Select(CreateGetter<TNode, object>)
                    .Concat(type.GetFields(bindingFlags).Where(f => ValidMemberTypeNode(f.FieldType))
                        .Select(CreateGetter<TNode, object>))
                    .Select(d => (Func<object, object>)(o => d((TNode)o))).ToList();
                TypeChildrenGetters.Add(type, getters);
            }
            return getters;
        }

        private static Action<object> GetGraphDelegate(object obj)
        {
            var type = obj.GetType();
            Action<object> del;
            if(!TypeGetTruncatedStringInGraphDelegates.TryGetValue(type, out del))
            {
                var genericMethod = GetTruncatedMethodInfo.MakeGenericMethod(type);
                del = CreateCompatibleDelegate<Action<object>>(null, genericMethod);
                TypeGetTruncatedStringInGraphDelegates.Add(type, del);
            }
            return del;
        }

        private static IStringProperty GetStringProperty<TDataModel>(PropertyInfo propertyInfo)
            where TDataModel : class 
        {
            IStringProperty stringProperty;
            if(!StringProperties.TryGetValue(propertyInfo, out stringProperty))
            {
                stringProperty = new StringProperty<TDataModel>(propertyInfo);
                if(!stringProperty.Required && stringProperty.MaxLength == null)
                {
                    stringProperty = null;
                }
                StringProperties.Add(propertyInfo, stringProperty);
            }

            return stringProperty;
        }

        private static List<IStringProperty> GetStringProperties<TDataModel>()
            where TDataModel : class
        {
            var type = CachedType<TDataModel>.Type;
            List<IStringProperty> stringProperties;
            if(!TypeStringProperties.TryGetValue(type, out stringProperties))
            {
                stringProperties = type.GetProperties(bindingFlags)
                    .Where(p => p.PropertyType == CachedType<string>.Type && p.GetGetMethod(false) != null && p.GetSetMethod(false) != null)
                    .Select(GetStringProperty<TDataModel>)
                    .Where(s => s != null)
                    .ToList();
                TypeStringProperties.Add(type, stringProperties);
            }
            return stringProperties;
        }

        private static bool ValidMemberTypeNode(Type memberType)
        {
            return ((!memberType.IsValueType && !memberType.IsPrimitive && memberType.IsClass) || CachedType<IEnumerable>.Type.IsAssignableFrom(memberType))
                   && (memberType != CachedType<string>.Type && memberType != CachedType<DateTime>.Type);
        }

        private static Func<TDataModel, TMember> CreateGetter<TDataModel, TMember>(FieldInfo fieldInfo)
        {
            var instanceParameter = Expression.Parameter(CachedType<TDataModel>.Type);
            var accessField = Expression.PropertyOrField(instanceParameter, fieldInfo.Name);
            return Expression.Lambda<Func<TDataModel, TMember>>(accessField, instanceParameter).Compile();
        }

        private static Func<TDataModel, TProperty> CreateGetter<TDataModel, TProperty>(PropertyInfo propertyInfo)
        {
            return (Func<TDataModel, TProperty>)Delegate.CreateDelegate(CachedType<Func<TDataModel, TProperty>>.Type, propertyInfo.GetMethod);
        }

        private static Action<TDataModel, TProperty> CreateSetter<TDataModel, TProperty>(PropertyInfo propertyInfo)
        {
            return (Action<TDataModel, TProperty>)Delegate.CreateDelegate(CachedType<Action<TDataModel, TProperty>>.Type, propertyInfo.SetMethod);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
        /// </summary>
        private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = CachedType<TSource>.Type;

            var member = propertyLambda.Body as MemberExpression;
            if(member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda));
            }

            var propertyInfo = member.Member as PropertyInfo;
            if(propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda));
            }

            if(type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", propertyLambda, type));
            }

            return propertyInfo;
        }

        /// <summary>
        /// http://codereview.stackexchange.com/questions/1070/generic-advanced-delegate-createdelegate-using-expression-trees
        /// </summary>
        public static T CreateCompatibleDelegate<T>(object instance, MethodInfo method)
        {
            var delegateInfo = typeof(T).GetMethod("Invoke");
            var methodParameters = method.GetParameters();
            var delegateParameters = delegateInfo.GetParameters();

            // Convert the arguments from the delegate argument type
            // to the method argument type when necessary.
            var arguments = delegateParameters.Select(delegateParameter => Expression.Parameter(delegateParameter.ParameterType, delegateParameter.Name)).ToArray();
            var convertedArguments = new Expression[methodParameters.Length];
            for(var i = 0; i < methodParameters.Length; ++i)
            {
                var methodType = methodParameters[i].ParameterType;
                var delegateType = delegateParameters[i].ParameterType;
                convertedArguments[i] = methodType != delegateType ? (Expression)Expression.Convert(arguments[i], methodType) : arguments[i];
            }

            // Create method call.
            var methodCall = Expression.Call(instance == null ? null : Expression.Constant(instance), method, convertedArguments);

            // Convert return type when necessary.
            var convertedMethodCall = delegateInfo.ReturnType == method.ReturnType ? (Expression)methodCall : Expression.Convert(methodCall, delegateInfo.ReturnType);

            return Expression.Lambda<T>(convertedMethodCall, arguments).Compile();
        }

        private interface IStringProperty
        {
            int? MaxLength { get; }
            bool Required { get; }
            Result Process(object obj);
        }

        private static class CachedType<T>
        {
            public static readonly Type Type = typeof(T);
        }

        private class StringProperty<TDataModel> : IStringProperty
            where TDataModel : class
        {
            public int? MaxLength { get { return _maxLength; } }
            public bool Required { get { return _required; } }

            public Result Process(object obj)
            {
                var result = new Result { PropertyName = _propertyName };

                var dataModel = (TDataModel) obj;
                if(dataModel != null)
                {
                    var stringValue = _get(dataModel);
                    if(Required && stringValue == null)
                    {
                        result.SetRequiredToEmpty = true;
                        _set(dataModel, stringValue = "");
                    }

                    if(_maxLength != null && stringValue != null)
                    {
                        if(stringValue.Length > _maxLength.Value)
                        {
                            result.TruncatedString = stringValue;
                            result.TruncatedLength = _maxLength.Value;
                        }
                        _set(dataModel, stringValue.TrimTruncate(_maxLength.Value));
                    }
                }

                return result;
            }

            public StringProperty(PropertyInfo propertyInfo)
            {
                _propertyName = string.Format("{0}.{1}", CachedType<TDataModel>.Type.Name, propertyInfo.Name);
                _required = propertyInfo.GetCustomAttributes<RequiredAttribute>().Any();
                var stringLengthAttribute = propertyInfo.GetCustomAttributes<StringLengthAttribute>().FirstOrDefault();
                if(stringLengthAttribute != null)
                {
                    _maxLength = stringLengthAttribute.MaximumLength;
                }

                _get = CreateGetter<TDataModel, string>(propertyInfo);
                _set = CreateSetter<TDataModel, string>(propertyInfo);
            }

            private readonly string _propertyName;
            private readonly int? _maxLength;
            private readonly bool _required;

            private Func<TDataModel, string> _get;
            private Action<TDataModel, string> _set;
        }
    }
}