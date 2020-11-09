using MediatR;
using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.CommandDefinitions
{
    //public abstract class CommandReciever<TResponse, TValid> :
    //    ICommand,
    //    IRequest<TResponse>
    //    where TValid : IValidationModel, new()

    //{
    //    public string[] IgnoredProperties { get; }
    //    public Dictionary<string, (object, Type)> RequestPropertyValues { get; }
    //    public Guid Id { get; set; }
    //    public TValid ValidationModel { get; }
    //    public CommandReciever(object request, Guid id)
    //    {
    //        if (request == null)
    //            return;
    //        if (typeof(TValid) != typeof(EmptyValidation))
    //        {
    //            ValidationModel = new TValid();
    //            ValidationModel.Set(request);
    //        }

    //        Id = id;
    //        IgnoredProperties = new string[]
    //        { nameof(Entity.CreatedDate), nameof(Entity.UpdatedDate) };
    //        RequestPropertyValues = new Dictionary<string, (object, Type)>
    //        {
    //            { nameof(ICommand.Id), (id,typeof(Guid)) }
    //        };

    //        var properties = request
    //            .GetType()
    //            .GetProperties()
    //            .Where(x => !IgnoredProperties.Contains(x.Name));

    //        foreach (var prop in properties)
    //        {
    //            if (prop.TryGetPropertyValue(request, out var value))
    //                RequestPropertyValues.Add(
    //                    value.Key, (value.Value, value.Value.GetType()));
    //        }
    //    }

    public abstract class CommandReciever<TCommand> : 
        ICommand, 
        ICommandReciever<TCommand>
        where TCommand : IModifyRequest
    {
        public string[] IgnoredProperties { get; set; }
        public Dictionary<string, (object, Type)> RequestPropertyValues { get; set; }
        public Guid Id { get; }
        public TCommand Command { get; }
        public CommandReciever(TCommand request, Guid id)
        {
            Command = request;
            Id = id;
        }
    }
}
