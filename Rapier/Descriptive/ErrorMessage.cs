namespace Rapier.Descriptive
{
    public static class ErrorMessage
    {
        public static class Query
        {
            public const string OrderParameterSeperator = "Order parameter must contain seperator ':'";
            public const string OrderParameterDescriptor = "Order parameter must contain a sort order description 'asc' or 'desc'";
            public const string OrderParameterField = "Order parameter must contain a legitimate field";
            public const string ExpandParameterField = "Expand parameter must contain a legitimate relational field";
        }

        public static class Configuration
        {
            public const string IdCollectionAttribute = "IdCollectionAttribute.EntityType must inherit IEntity.";
        }
    }
}
