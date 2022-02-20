using Vorp.Shared.Records;

namespace Vorp.Core.Client.Managers.CharacterManagement
{
    public class CharacterManager : Manager<CharacterManager>
    {
        public override void Begin()
        {
            ClientGateway.Mount("vorp:character:list", new Action<Dictionary<int, Character>>(characters =>
            {
                Logger.Debug($"Received {characters.Count} characters from the server.");
                foreach(var kvp in characters)
                {
                    int characterId = kvp.Key;
                    Character character = kvp.Value;
                }
            }));
        }
    }
}
