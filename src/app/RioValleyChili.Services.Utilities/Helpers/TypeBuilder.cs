using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace RioValleyChili.Services.Utilities.Helpers
{
    /// <summary>
    /// Based off of answer by Ethan J. Brown @ http://stackoverflow.com/questions/606104/how-to-create-linq-expression-tree-with-anonymous-type-in-it
    /// </summary>
    public static class TypeBuilder
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName("DynamicTypesAssembly");
        private static readonly ModuleBuilder ModuleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        private static readonly Dictionary<int, Type> DynamicTypes = ModuleBuilder.GetTypes().ToDictionary(GetHashedFields, t => t);
        private static bool _buildingType;

        /// <summary>
        /// Returns a dynamically generated type wrapping around supplied field names and types, caching the resulting type according to supplied parameters for further requests.
        /// </summary>
        /// <param name="fields">Dictionary of field types keyed by desired name that resulting type should contain.</param>
        /// <param name="valueType">True if type should be a value type, false if reference type.</param>
        /// <returns>A dynamically generated type.</returns>
        public static Type GetDynamicType(IDictionary<string, Type> fields, bool valueType = false)
        {
            return GetDynamicType(fields, valueType, null, GetHashedFields(fields, valueType, null));
        }

        /// <summary>
        /// Returns a dynamically generated type wrapping around supplied field names and types, caching the resulting type according to supplied parameters for further requests.
        /// </summary>
        /// <param name="fields">Dictionary of field types keyed by desired name that resulting type should contain.</param>
        /// <param name="parent">Type that resulting class should derive from.</param>
        /// <returns>A dynamically generated type.</returns>
        public static Type GetDynamicType(IDictionary<string, Type> fields, Type parent)
        {
            if(parent.IsGenericTypeDefinition)
            {
                throw new ArgumentException("parent cannot be generic type definition - pass in type with generic arguments resolved instead.");
            }
            return GetDynamicType(fields, false, parent, GetHashedFields(fields, false, parent));
        }

        private static Type GetDynamicType(IDictionary<string, Type> fields, bool valueType, Type parent, int fieldHash)
        {
            var dynamicType = TryGetDynamicType(fieldHash);
            if(dynamicType != null)
            {
                return dynamicType;
            }
            
            if(_buildingType)
            {
                while(_buildingType) { }
                return GetDynamicType(fields, valueType, parent, fieldHash);
            }

            return BuildNewType(fields, valueType, parent, fieldHash);
        }

        private static Type BuildNewType(IDictionary<string, Type> fields, bool valueType, Type parent, int fieldHash)
        {
            Type dynamicType;
            _buildingType = true;

            try
            {
                var typeName = ConstructTypeName(fields, valueType, parent);
                System.Reflection.Emit.TypeBuilder typeBuilder;

                if(valueType)
                {
                    typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Serializable, typeof(ValueType));
                }
                else if(parent != null)
                {
                    typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, parent);
                }
                else
                {
                    typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
                }

                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
                foreach(var field in fields)
                {
                    typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
                }

                DynamicTypes.Add(fieldHash, dynamicType = typeBuilder.CreateType());
            }
            finally
            {
                _buildingType = false;
            }

            return dynamicType;
        }

        private static Type TryGetDynamicType(int fieldHash)
        {
            Type dynamicType;
            return DynamicTypes.TryGetValue(fieldHash, out dynamicType) ? dynamicType : null;
        }

        private static int GetHashedFields(IEnumerable<KeyValuePair<string, Type>> fields, bool valueType, Type parent)
        {
            unchecked
            {
                var seed = (valueType.GetHashCode() * 397) ^ (parent == null ? 0 : parent.GetHashCode());
                return fields.Aggregate(seed,
                    (h, s) =>
                    h ^
                    (s.Key == null ? 0 : s.Key.GetHashCode() * 397) ^
                    (s.Value.GetHashCode() * 397));
            }
        }

        private static int GetHashedFields(Type type)
        {
            Type parent = null;
            if(!type.IsValueType && type.DeclaringType != typeof(object))
            {
                parent = type.DeclaringType;
            }

            return GetHashedFields(type.GetFields(BindingFlags.Public).ToDictionary(f => f.Name, f => f.FieldType), type.IsValueType, parent);
        }

        private static string ConstructTypeName(IEnumerable<KeyValuePair<string, Type>> fields, bool valueType, Type parent)
        {
            var types = string.Join("; ", fields.Select(f => string.Format("{0} {1}", ResolveTypeName(f.Value), f.Key)).ToList());
            return string.Format("{0}{{ {1} }}", valueType ? "struct " : string.Format("class : {0} ", parent == null ? "" : ResolveTypeName(parent)), types);
        }

        private static string ResolveTypeName(Type type)
        {
            var typeName = type.Name;
            if(!type.IsGenericType)
            {
                return typeName;
            }

            var genericNames = type.GetGenericArguments().Select(ResolveTypeName).ToList();
            return typeName.Replace(string.Format("`{0}", genericNames.Count), string.Format("<{0}>", string.Join(", ", genericNames)));
        }
    }
}