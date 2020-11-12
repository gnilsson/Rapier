using Rapier.External;
using Rapier.External.Models;
using System;

namespace Rapier.CommandDefinitions
{
    public interface IModifier<TEntity, TCommand>
        where TEntity : IEntity
        where TCommand : ICommand
    {
        public UpdateDelegate Updater { get; }
        public CreateDelegate Creator { get; }

        public delegate void UpdateDelegate(TEntity entity, TCommand command);
        public delegate TEntity CreateDelegate(TCommand command);
        public void DetailedAppend(params (string, (object, Type))[] properties);
        public void Append(params (string, object)[] properties);
    }
}