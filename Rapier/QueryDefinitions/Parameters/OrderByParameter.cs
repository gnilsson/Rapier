using Rapier.Descriptive;
using Rapier.External.Models;
using System;
using System.ComponentModel;

namespace Rapier.QueryDefinitions.Parameters
{
    public class OrderByParameter
    {
        public ListSortDirection SortDirection { get; }
        public string Node { get; }
        public OrderByParameter(string[] orderQuery)
        {
            SortDirection = orderQuery[0].Contains(
                OrderParameterDescriptor.Ascending, StringComparison.OrdinalIgnoreCase)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;
            Node = orderQuery[1];
        }
    }
}
