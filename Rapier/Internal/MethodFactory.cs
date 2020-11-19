using Rapier.Descriptive;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rapier.Internal
{
    public static class MethodFactory
    {
        public static IDictionary<string, MethodInfo> QueryMethodContainer { get; }
        public static MethodInfo Contains { get; }
        public static MethodInfo CompareTo { get; }
        static MethodFactory()
        {
            var flags = BindingFlags.Public | BindingFlags.Static;
            QueryMethodContainer = new Dictionary<string, MethodInfo>
            {
                {QueryMethods.CallStringContains,typeof(QueryUtility)
                    .GetMethod(nameof(QueryUtility.CallStringContains), flags) },   
                
                {QueryMethods.CallDateTimeCompare,typeof(QueryUtility)
                    .GetMethod(nameof(QueryUtility.CallDateTimeCompare), flags)}
            };

            Contains = typeof(string).GetMethod(nameof(Methods.Contains), new[] { typeof(string) });
            CompareTo = typeof(DateTime).GetMethod(Methods.CompareTo, new[] { typeof(DateTime) });
        }
    }
}
