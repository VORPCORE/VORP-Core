using Vorp.Core.Client.Environment;
using Vorp.Core.Client.Environment.Config;

namespace Vorp.Core.Client.Managers
{
    public class ClientConfigManager : Manager<ClientConfigManager>
    {
        public override void Begin()
        {
            //_configCache = GetConfig();

            //if (!_configCache.PvpEnabled)
            //{
            //    NetworkSetFriendlyFireOption(true);
            //    uint playerGroup = (uint)GetHashKey("PLAYER");
            //    SetRelationshipBetweenGroups((int)eRelationshipType.Neutral, playerGroup, playerGroup);
            //}
        }
    }
}
