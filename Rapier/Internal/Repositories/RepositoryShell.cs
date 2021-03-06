using Rapier.Internal.Utility;

namespace Rapier.Internal.Repositories
{
    public class RepositoryShell
    {
        public ExpressionUtility.ConstructorDelegate Constructor { get; set; }
        public object QueryConfiguration { get; set; }

        public RepositoryShell(
            ExpressionUtility.ConstructorDelegate repositoryConstructor,
            object queryConfiguration)
        {
            Constructor = repositoryConstructor;
            QueryConfiguration = queryConfiguration;
        }

    }
}
