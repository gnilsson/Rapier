using Rapier.CommandDefinitions;
using Rapier.External;
using Rapier.Internal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.Internal
{
    public class EntitiesFetcher<TRequest>
    {
        private readonly IGeneralRepository _repository;
        public EntitiesFetcher(IRepositoryWrapper repositoryWrapper)
        {
            _repository = repositoryWrapper.General;
        }

        public IEnumerable<IEntity> Fetch()
        {
            return null;
        }
    }
}
