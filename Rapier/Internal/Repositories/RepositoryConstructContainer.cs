using Rapier.Internal.Utility;

namespace Rapier.Internal.Repositories
{
    public class RepositoryConstructContainer
    {
        public ExpressionUtility.ConstructorDelegate RepositoryConstructor { get; set; }
        public object QueryConfiguration { get; set; }

        public RepositoryConstructContainer(
            ExpressionUtility.ConstructorDelegate repositoryConstructor,
            object queryConfiguration)
        {
            RepositoryConstructor = repositoryConstructor;
            QueryConfiguration = queryConfiguration;
        }

    }
}
