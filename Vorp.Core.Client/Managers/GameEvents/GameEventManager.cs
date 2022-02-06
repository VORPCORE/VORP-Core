using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorp.Core.Client.Managers.GameEvents
{
    public class GameEventManager : Manager<GameEventManager>
    {
        public override void Begin()
        {
            EventHandler.Add("gameEventTriggered", new Action<string, List<dynamic>>(OnGameEventTriggered));
        }

        void OnGameEventTriggered(string name, List<dynamic> args)
        {
            Logger.Info($"game event '{name}' ({String.Join(", ", args.ToArray())})");
        }
    }
}
