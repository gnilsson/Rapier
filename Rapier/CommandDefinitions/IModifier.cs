using Rapier.External;
using Rapier.External.Models;
using System;

namespace Rapier.CommandDefinitions
{
    public interface IModifier<TEntity, TCommand>
        where TEntity : IEntity
        where TCommand : ICommand
    {
        public Func<TCommand,TEntity> Create { get; }
        public Action<TEntity,TCommand> Update { get; }
        public void Append(params (string, object)[] properties);
    }
}
