using MediatR;
using Rapier.CommandDefinitions;
using Rapier.External.Handlers;
using Rapier.External.PipelineBehaviours;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class HandlerTypesContainer : List<Type[]>
    {
        public HandlerTypesContainer()
        {
            AddRange(new List<Type[]>
            {
                new [] { typeof(IRequestHandler<,>), typeof(CreateHandler<,,>) },
                new [] { typeof(IRequestHandler<,>), typeof(GetHandler<,,>) },
                new [] { typeof(IModifier<,>), typeof(Modifier<,>) },
        //    //new [] { typeof(IRequestHandler<,>), typeof(GetByIdHandler<,,,>) },
        //    //new [] { typeof(IRequestHandler<,>), typeof(UpdateHandler<,,,>) },
        //    //new [] { typeof(IRequestHandler<,>), typeof(DeleteHandler<,>) },
                //new [] { typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,,>) },
                //new [] { typeof(IPipelineBehavior<,>), typeof(ProvideCommandBehaviour<,>)},
                //new [] { typeof(IPipelineBehavior<,>), typeof(ProvideQueryBehaviour<,>)},
            });
        }
    }
}
