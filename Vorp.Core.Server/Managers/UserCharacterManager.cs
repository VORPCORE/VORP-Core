using FxEvents;
using System.Threading.Tasks;
using Vorp.Core.Server.Events;
using Vorp.Shared.Models;

namespace Vorp.Core.Server.Managers
{
    public class UserCharacterManager : Manager<UserCharacterManager>
    {
        public override void Begin()
        {
            EventDispatcher.Mount("vorp:character:coords:save", new Func<ClientId, int, Vector3, float, Task<bool>>(OnSaveCoordsAsync));
            EventDispatcher.Mount("vorp:character:dead", new Func<ClientId, int, bool, Task<bool>>(OnUpdateIsDeadAsync));
        }

        private async Task<bool> OnUpdateIsDeadAsync(ClientId source, int id, bool isDead)
        {
            if (source.Handle != id) return false;
            return await source.User.ActiveCharacter.SetDead(isDead);
        }

        private async Task<bool> OnSaveCoordsAsync(ClientId source, int id, Vector3 coords, float heading)
        {
            if (source.Handle != id) return false;

            JsonBuilder jb = new();
            jb.Add("x", coords.X);
            jb.Add("y", coords.Y);
            jb.Add("z", coords.Z);
            jb.Add("heading", heading);

            source.User.ActiveCharacter.Coords = $"{jb}";
            return await source.User.ActiveCharacter.Save();
        }
    }
}
