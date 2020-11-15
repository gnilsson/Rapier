using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Rapier.CommandDefinitions;
using Rapier.External.Models;
using Rapier.Internal.Repositories;

namespace Rapier.External.PipelineBehaviours
{
    //public class ProcessCommandBehaviour<TRequest> : 
    //    IRequestPreProcessor<TRequest>
    //    where TRequest : ICommandReciever<IModifyRequest>
    //{
    //    private readonly IGeneralRepository _repository;
    //    public ProcessCommandBehaviour(IRepositoryWrapper repositoryWrapper)
    //    {
    //        _repository = repositoryWrapper.General;
    //    }
    //    public Task Process(TRequest request, CancellationToken cancellationToken)
    //    { // attach
    //      //  var type = request.Command.
    //      // throw new NotImplementedException();
    //        return Task.FromResult(0);
    //    }
    //}
}
