using Rapier.External;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public interface IRepositoryWrapper
    {
        IRepository<TEntity, TResponse> Get<TEntity, TResponse>() where TEntity : Entity;
        IGeneralRepository General { get; }
        Task SaveAsync();
    }
}