using Rapier.Descriptive;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Rapier.Internal
{
    public static class MethodFactory
    {
        public static IDictionary<string, MethodInfo> QueryMethodContainer { get; }
        public static MethodInfo Contains { get; }
        public static MethodInfo CompareTo { get; }
        public static MethodInfo GetManyAsync { get; }
        static MethodFactory()
        {
            var flags = BindingFlags.Public | BindingFlags.Static;
            QueryMethodContainer = new Dictionary<string, MethodInfo>
            {
                {QueryMethod.CallStringContains,typeof(QueryUtility)
                    .GetMethod(nameof(ExpressionUtility.CallStringContains), flags) },   
                
                {QueryMethod.CallDateTimeCompare,typeof(QueryUtility)
                    .GetMethod(nameof(ExpressionUtility.CallDateTimeCompare), flags)}
            };

            Contains = typeof(string).GetMethod(nameof(Method.Contains), new[] { typeof(string) });
            CompareTo = typeof(DateTime).GetMethod(Method.CompareTo, new[] { typeof(DateTime) });
            GetManyAsync = typeof(IGeneralRepository).GetMethod(nameof(IGeneralRepository.GetManyAsync), 1,
                            new[] { typeof(IEnumerable<Guid>), typeof(CancellationToken) });
        }
    }
}
