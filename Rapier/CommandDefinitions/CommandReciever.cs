using Rapier.External.Models;
using System;
using System.Collections.Generic;

namespace Rapier.CommandDefinitions
{
    public abstract class CommandReciever<TCommand> : 
        ICommand, 
        ICommandReciever<TCommand>
        where TCommand : IModifyRequest
    {
        public string[] IgnoredProperties { get; set; }
        public Dictionary<string, (object, Type)> RequestPropertyValues { get; set; }
        public Guid Id { get; }
        public TCommand Command { get; }
        public string IncludeNavigation { get; set; }

        public CommandReciever(TCommand request, Guid id)
        {
            Command = request;
            Id = id;
        }
    }
}
