using Rapier.External;
using Rapier.External.Attributes;
using Rapier.External.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rapier.Internal.Utility
{
    public static class ReflectionUtility
    {
        public static T Invoke<T>(
            this Action<T> action)
            where T : new()
        {
            var obj = new T();
            action.Invoke(obj);
            return obj;
        }

        public static bool ParentHasInterface(
            this Type type,
            Type interfaceType,
            out Type parentInterface)
        {
            parentInterface = type
                .GetInterfaces()?
                .FirstOrDefault(x => x.GetTypeInfo()
                    .ImplementedInterfaces.Contains(interfaceType));
            return parentInterface != null;
        }

        public static bool TryGetRequestValue(
            this PropertyInfo property,
            object data,
            out KeyValuePair<string, object> value)
        {
            var attribute = property.GetCustomAttribute<RequestParameterAttribute>();
            if (attribute?.Mode == RequestParameterMode.Hidden)
            {
                value = default;
                return false;
            }

            var propertyValue = property.GetValue(data);

            var propertyName = attribute == null && property.PropertyType == typeof(List<Guid>) ? 
                property.Name.Replace("Id", string.Empty) : attribute == null ?  
                property.Name : attribute.EntityProperty;



            value = KeyValuePair.Create(propertyName, propertyValue);

            return propertyValue is not null ||
                  (propertyValue is int and 0) ||
                  (propertyValue is string and "");
        }

        public static Type GetFirstClassChild(
            this IEnumerable<Type> types,
            Type baseType,
            string name) =>
            types.FirstOrDefault(
                x => x.BaseType == baseType && x.Name.Contains(name));

        public static Type GetFirstInterfaceChild(
            this IEnumerable<Type> types,
            Type baseType,
            string name) =>
            types.FirstOrDefault(
                x => x.GetInterface(baseType.Name) != null && x.Name.Contains(name));

        public static bool IsEnumerableType(
            this Type type)
            => type.GetInterface(nameof(IEnumerable)) != null;

        public static bool TryGetInterfaceType(
            this Type baseType,
            string interfaceName,
            out Type interfaceType)
        {
            interfaceType = baseType.GetInterface(interfaceName);
            return interfaceType != null;
        }

        public static async Task<object> InvokeAsync(
            this MethodInfo method,
            object obj,
            params object[] parameters)
        {
            var task = await Task.FromResult(method.Invoke(obj, parameters));
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        public static bool IsEntity(
            this Type type) =>
            type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity));

        public static bool IsEntityCollection(
            this object obj, out Type entity)
        {
            var genericArgs = obj.GetType().GetGenericArguments();
            entity = null;
            if (genericArgs.Length == 0)
                return false;

            entity = genericArgs.FirstOrDefault();
            return entity.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity));
        }

        public static bool TryGetAttribute<T>(this PropertyInfo property, out T attribute)
            where T : Attribute
        {
            attribute = property.GetCustomAttribute<T>();
            return attribute != null;
        }
    }
}
