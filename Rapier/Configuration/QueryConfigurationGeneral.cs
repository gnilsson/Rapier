using Rapier.QueryDefinitions;

namespace Rapier.Configuration
{
    public class QueryConfigurationGeneral
    {
        public QueryMethodContainer Methods { get; }
        public QueryConfigurationGeneral()
        {
            Methods = new QueryMethodContainer();
        }
    }
}
