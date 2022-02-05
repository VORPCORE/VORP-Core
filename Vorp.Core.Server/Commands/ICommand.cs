using System.Collections.Generic;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Commands
{
    public interface ICommand
    {
        void On(User user, Player player, List<string> arguments);
    }
}