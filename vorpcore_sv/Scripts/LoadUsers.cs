using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using vorpcore_sv.Class;
using vorpcore_sv.Utils;

namespace vorpcore_sv.Scripts
{
    public class LoadUsers:BaseScript
    {
        public static Dictionary<string, User> _users;
        public static List<string> _whitelist;
        public static bool _usingWhitelist;
        public LoadUsers()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["vorp:playerSpawn"] += new Action<Player>(PlayerSpawnFunction);
            EventHandlers["vorp:getUser"] += new Action<int,CallbackDelegate>((source,cb) =>
            {
                string steam = "steam:" + Players[source].Identifiers["steam"];
                if (_users.ContainsKey(steam))
                {
                    cb.Invoke(_users[steam].GetUser());
                }
            });
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            _users = new Dictionary<string, User>();
            _whitelist = new List<string>();
            Tick += SaveUsersInServer;
        }

        private async Task SaveUsersInServer()
        {
            await Delay(300000);
            foreach (KeyValuePair<string,User> user in _users)
            {
                await Delay(1000);
                user.Value.SaveUser();
            }
        }
        
        private void OnPlayerDropped([FromSource]Player player, string reason)
        {
            Debug.WriteLine($"Player {player.Name} dropped (Reason: {reason}).");
            string identifier = "steam:" + player.Identifiers["steam"];
            SaveCoordsDB.LastCoordsInCache.Remove(player);
            if (_users.ContainsKey(identifier))
            {
                _users[identifier].SaveUser();
                _users.Remove(identifier);
            }
            Debug.WriteLine($"Saved player {player.Name}.");
        }

        private async Task<bool> LoadUser([FromSource]Player source)
        {
            string identifier = "steam:" + source.Identifiers["steam"];
            string license = "license:" + source.Identifiers["license"];
            List<object> resultList = await Exports["ghmattimysql"].executeSync("SELECT * FROM users WHERE identifier = ?", new[] {identifier});
            if (resultList.Count > 0)
            {
                IDictionary<string, object> user = (dynamic)resultList[0];
                if ((int)user["banned"] == 1)
                {
                    return true;
                }
                User newUser = new User(identifier, user["group"].ToString(),(int)user["warnings"], license);
                if (_users.ContainsKey(identifier))
                {
                    _users[identifier] = newUser;
                }
                else
                {
                    _users.Add(identifier,newUser);
                }
                
                return false;
            }
            else
            {
                //New User
                await Exports["ghmattimysql"].executeSync("INSERT INTO users VALUES(?,'user',0,0)", new[] {identifier});
                User newUser = new User(identifier, "user", 0, license);
                if (_users.ContainsKey(identifier))
                {
                    _users[identifier] = newUser;
                }
                else
                {
                    _users.Add(identifier,newUser);
                }
                return false;
            }
        }
        
        private async void OnPlayerConnecting([FromSource]Player source, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            bool _userEntering = false;
            bool banned = false;

            await Delay(0);

            if (!LoadConfig.isConfigLoaded)
            {
                deferrals.done("Servers is loading, Please wait a minute.");
                setKickReason("Servers is loading, Please wait a minute.");
                return;
            }

            var steamIdentifier = "steam:"+source.Identifiers["steam"];
            string license = "license:" + source.Identifiers["license"];
            deferrals.update(LoadConfig.Langs["CheckingIdentifier"]);
            if (steamIdentifier == null)
            {
                deferrals.done(LoadConfig.Langs["NoSteam"]);
                setKickReason(LoadConfig.Langs["NoSteam"]);
            }
            if (_usingWhitelist)
            {
                if (_whitelist.Contains(steamIdentifier))
                {
                    //deferrals.done();
                    _userEntering = true;
                }
                else
                {
                    deferrals.done(LoadConfig.Langs["NoInWhitelist"]);
                    setKickReason(LoadConfig.Langs["NoInWhitelist"]);
                }
            }
            else
            {
                _userEntering = true;
            }

            

            if (_userEntering)
            {
                deferrals.update(LoadConfig.Langs["LoadingUser"]);

                if (_users.ContainsKey(steamIdentifier) || CheckConnectedLicenses(license)) // Fix Duplicate Connections
                {
                    deferrals.done(LoadConfig.Langs["IsConnected"]);
                    setKickReason(LoadConfig.Langs["IsConnected"]);
                }

                banned =  await LoadUser(source);
                if (banned)
                {
                    deferrals.done(LoadConfig.Langs["BannedUser"]);
                    setKickReason(LoadConfig.Langs["BannedUser"]);
                }
                deferrals.done();
            }
        }

        private bool CheckConnectedLicenses(string license)
        {
            foreach (var user in _users)
            {
                if (user.Value.License == license)
                {
                    return true;
                }
            }
            return false;
        }

        private void PlayerSpawnFunction([FromSource] Player source)
        {
            string steam = "steam:" + source.Identifiers["steam"];
            if (_users.ContainsKey(steam))
            {
                Debug.WriteLine("Characters loaded "+_users[steam].Numofcharacters);
                _users[steam].Source = int.Parse(source.Handle);
                if (_users[steam].Numofcharacters <= 0)
                {
                    TriggerEvent("vorp_CreateNewCharacter",int.Parse(source.Handle));
                }
                else
                {
                    if (LoadConfig.Config["MaxCharacters"].ToObject<int>() == 1 && _users[steam].Numofcharacters <= 1)
                    {
                        TriggerEvent("vorp_SpawnUniqueCharacter", int.Parse(source.Handle));
                    }
                    else
                    {
                        TriggerEvent("vorp_GoToSelectionMenu", int.Parse(source.Handle));
                    }    
                }
            }
        }
    }
}