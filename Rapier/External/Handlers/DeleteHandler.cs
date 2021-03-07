using MediatR;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
using System.Threading;
using System.Threading.Tasks;
namespace Rapier.External.Handlers
{
    public class DeleteHandler<TEntity, TRequest> :
                 IRequestHandler<TRequest, DeleteResponse>
                 where TEntity : class, IEntity
                 where TRequest : DeleteCommand
    {
        internal readonly IRepositoryWrapper _repositoryWrapper;
        public DeleteHandler(
            IRepositoryWrapper repositoryWrapper) =>
            _repositoryWrapper = repositoryWrapper;

        public virtual async Task<DeleteResponse> Handle(
            TRequest request, CancellationToken cancellationToken)
        {
            var entity = await _repositoryWrapper.General.FindAsync<TEntity>(
                request.Id, cancellationToken);

            if (entity == null) return null;

            _repositoryWrapper.General.Delete(entity);
            await _repositoryWrapper.SaveAsync();
            return new DeleteResponse { Message = "1" };  // Todo: proper response
        }
    }
}
