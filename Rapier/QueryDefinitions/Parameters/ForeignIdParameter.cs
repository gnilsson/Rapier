﻿using Rapier.Descriptive;
using System;

namespace Rapier.QueryDefinitions.Parameters
{
    public class ForeignIdParameter : IParameter
    {
        public string[] ParentNavigationProperties { get; protected set; }
        public string[] NavigationProperties { get; protected set; }
        public object Value { get; private set; }
        public string Method { get; private set; }

        public void Set(string value)
        {
            Value = Guid.Parse(value);
            Method = QueryMethod.Equal;
        }

        public ForeignIdParameter(string value, string[] navigationNodes)
        {
            this.Set(value);
            NavigationProperties = navigationNodes;
        }
    }
}
