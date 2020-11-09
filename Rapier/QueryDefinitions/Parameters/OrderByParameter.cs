using Rapier.External.Models;
using System;
using System.ComponentModel;

namespace Rapier.QueryDefinitions.Parameters
{
    public class OrderByParameter
    {
        public ListSortDirection SortDirection { get; private set; }
        public string Node { get; private set; }

        public OrderByParameter(GetRequest orderable)
        {
            var text = orderable.OrderBy.Split(":");
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
