using Rapier.Descriptive;
using System;
using System.Reflection;

namespace Rapier.QueryDefinitions
{
    public class QueryMethodContainer
    {
        public MethodInfo Contains { get; }
        public MethodInfo CompareTo { get; }
        //    public ReadOnlyDictionary<string,ReadOnlyDictionary<string,MethodInfo>> QueryMethods { get; }

        public QueryMethodContainer()
        {
            Contains = typeof(string).GetMethod(nameof(Methods.Contains), new[] { typeof(string) });
            CompareTo = typeof(DateTime).GetMethod(Methods.CompareTo, new[] { typeof(DateTime) });
        }
    }
}
