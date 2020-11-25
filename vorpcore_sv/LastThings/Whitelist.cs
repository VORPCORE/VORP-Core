using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using vorpcore_sv.Utils;
using vorpcore_sv.Class;
using vorpcore_sv.Scripts;

namespace vorpcore_sv.Scripts
{
    class Whitelist : BaseScript
    {
        public static bool whitelistActive;

        public static List<string> whitelist = new List<string>();

        public Whitelist()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            LoadWhitelist();
        }

        public async Task LoadWhitelist()
        {
            await Delay(5000);
            Exports["ghmattimysql"].execute("SELECT * FROM whitelist", new[] { "" }, new Action<dynamic>((result) =>
            {
                if (result.Count > 0)
                {
                    foreach (var r in result)
                    {
                        whitelist.Add(r.identifier);
                    }
                }

            }));
        }

        private async void OnPlayerConnecting([FromSource]Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();

            await Delay(0);

            if (!LoadConfig.isConfigLoaded)
            {
                deferrals.done("Servers is loading, Please wait a minute.");
                setKickReason("Servers is loading, Please wait a minute.");
                return;
            }

            var steamIdentifier = player.Identifiers["steam"];

            if (steamIdentifier == null)
            {
                deferrals.done(LoadConfig.Langs["NoSteam"]);
                setKickReason(LoadConfig.Langs["NoSteam"]);
            }

            if (whitelistActive)
            {
                if (whitelist.Contains(steamIdentifier))
                {
                    deferrals.done();
                }
                else
                {
                    deferrals.done(LoadConfig.Langs["NoInWhitelist"]);
                    setKickReason(LoadConfig.Langs["NoInWhitelist"]);
                }
            }
            else
            {
                deferrals.done();
            }

            await Delay(1);

            Debug.WriteLine($"{playerName} is connecting with (Identifier: [{steamIdentifier}])");

            string sid = "steam:" + steamIdentifier;
            Debug.WriteLine(sid);
            Exports["ghmattimysql"].execute("SELECT * FROM characters WHERE identifier LIKE ?", new string[] { sid.ToString() }, new Action<dynamic>((result) =>
            {
                Debug.WriteLine(result.Count.ToString());
                if (result.Count != 0)
                {
                    string inventory = "{}";
                    if (!String.IsNullOrEmpty(result[0].inventory))
                    {
                        inventory = result[0].inventory;
                    }
                    LoadCharacter.characters[sid] = new Character(sid, result[0].group.ToString(), result[0].job.ToString(), result[0].jobgrade.ToString(), result[0].firstname.ToString(), result[0].lastname.ToString(), inventory, result[0].status.ToString(), result[0].coords.ToString(), double.Parse(result[0].money.ToString()), double.Parse(result[0].gold.ToString()), double.Parse(result[0].rol.ToString()), int.Parse(result[0].xp.ToString()), Convert.ToBoolean(result[0].isdead.ToString()));
                }
            }));
        }
    }
}
