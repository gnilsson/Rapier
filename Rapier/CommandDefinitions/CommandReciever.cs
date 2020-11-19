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
        public IDictionary<string, object> RequestPropertyValues { get; }
        public Guid Id { get; internal set; }
        public TCommand Command { get; }
        public string IncludeNavigation { get; set; }

        public CommandReciever(TCommand request)
        {
            Command = request;
            RequestPropertyValues = new Dictionary<string, object>();
        }
    }
}
