using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                .FirstOrDefault(x => x.GetTypeInfo().ImplementedInterfaces.Contains(interfaceType));
            return parentInterface != null;
        }

        public static bool TryGetPropertyValue(
            this PropertyInfo property,
            object data,
            out KeyValuePair<string, object> value)
        {
            var propValue = property.GetValue(data);
            value = KeyValuePair.Create(property.Name, propValue);
            return !string.IsNullOrWhiteSpace(propValue?.ToString());
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

    }
}
