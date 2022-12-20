namespace Vorp.Core.Client.Managers.Legacy
{
    public class CfxEventsManager : Manager<CfxEventsManager>
    {
        public override void Begin()
        {
            Event("playerSpawned", new Action<dynamic>(OnPlayerSpawned));
        }

        private async void OnPlayerSpawned(dynamic obj)
        {
            await BaseScript.Delay(5000);
            // ClientGateway.Send("vorp:user:active", Session.ServerId);
        }
    }
}
