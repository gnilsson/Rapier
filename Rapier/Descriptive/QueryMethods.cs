namespace Rapier.Descriptive
{
    public static class QueryMethods
    {
        public const string StringContains = nameof(StringContains);
        public const string DateTimeCompare = nameof(DateTimeCompare);
    }

    public static class Methods
    {
        public const string Contains = nameof(Contains);
        public const string CompareTo = nameof(CompareTo);
        public const string OrderBy = nameof(OrderBy);
        public const string OrderByDescending = nameof(OrderByDescending);
        public const string Include = nameof(Include);
        public const string ThenInclude = nameof(ThenInclude);
    }
}
