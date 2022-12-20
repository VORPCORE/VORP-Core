namespace Vorp.Core.Client.Managers.Legacy
{
    public class LegacyInstanceManager : Manager<LegacyInstanceManager>
    {
        public override void Begin()
        {
            Event("vorp:setInstancePlayer", new Action<bool>(OnSetPlayerInstance));
        }

        void OnSetPlayerInstance(bool instance)
        {

            switch (instance)
            {
                case true:
                    VorpAPI.StartSoloTutorialSession();
                    break;
                case false:
                    VorpAPI.EndTutorialSession();
                    break;
            }
        }
    }
}
