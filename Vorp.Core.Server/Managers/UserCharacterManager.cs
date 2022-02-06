using System.Threading.Tasks;
using Vorp.Core.Server.Events;
using Vorp.Shared.Models;

namespace Vorp.Core.Server.Managers
{
    public class UserCharacterManager : Manager<UserCharacterManager>
    {
        public override void Begin()
        {
            ServerGateway.Mount("vorp:character:coords:save", new Func<ClientId, int, Vector3, float, Task<bool>>(OnSaveCoords));
            ServerGateway.Mount("vorp:character:dead", new Func<ClientId, int, bool, Task<bool>>(OnUpdateIsDead));
        }

        private async Task<bool> OnUpdateIsDead(ClientId source, int id, bool isDead)
        {
            if (source.Handle != id) return false;
            return await source.User.ActiveCharacter.SetDead(isDead);
        }

        private async Task<bool> OnSaveCoords(ClientId source, int id, Vector3 coords, float heading)
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
