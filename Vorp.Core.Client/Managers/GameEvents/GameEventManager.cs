using System;
using System.Collections.Generic;

namespace Vorp.Core.Client.Managers.GameEvents
{
    public class GameEventManager : Manager<GameEventManager>
    {
        public override void Begin()
        {
            Event("gameEventTriggered", new Action<string, List<dynamic>>(OnGameEventTriggered));
        }

        void OnGameEventTriggered(string name, List<dynamic> args)
        {
            Logger.Info($"game event '{name}' ({String.Join(", ", args.ToArray())})");
        }
    }
}
