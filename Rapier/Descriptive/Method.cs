namespace Rapier.Descriptive
{
    public static class QueryMethod
    {
        public const string CallStringContains = nameof(CallStringContains);
        public const string CallDateTimeCompare = nameof(CallDateTimeCompare);
        public const string Equal = nameof(Equal);
    }

    public static class Method
    {
        public const string Contains = nameof(Contains);
        public const string CompareTo = nameof(CompareTo);
        public const string OrderBy = nameof(OrderBy);
        public const string OrderByDescending = nameof(OrderByDescending);
        public const string Include = nameof(Include);
        public const string ThenInclude = nameof(ThenInclude);
        public const string Add = nameof(Add);
    }
}
