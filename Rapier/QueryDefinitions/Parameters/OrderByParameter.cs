using Rapier.External.Models;
using System;
using System.ComponentModel;

namespace Rapier.QueryDefinitions.Parameters
{
    public class OrderByParameter
    {
        public ListSortDirection SortDirection { get; }
        public string Node { get; }
        public OrderByParameter(string orderable)
        {
            var text = orderable.Split(":");
            if (!text[0].Contains("asc", StringComparison.OrdinalIgnoreCase) &&
                !text[0].Contains("desc", StringComparison.OrdinalIgnoreCase))
                return;

            SortDirection = text[0].Contains("asc", StringComparison.OrdinalIgnoreCase)
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;
            Node = text[1];
        }
    }
}
