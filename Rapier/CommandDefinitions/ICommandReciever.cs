using Rapier.External.Models;

namespace Rapier.CommandDefinitions
{
    public interface ICommandReciever<out TCommand> where TCommand : IModifyRequest
    {
        public TCommand Command { get; }
    }
}
