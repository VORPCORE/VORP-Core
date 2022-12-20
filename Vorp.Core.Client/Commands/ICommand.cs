using Vorp.Core.Client.Environment.Entities;

namespace Vorp.Core.Client.Commands
{
    public interface ICommand
    {
        void On(VorpPlayer player, List<string> arguments);
    }
}