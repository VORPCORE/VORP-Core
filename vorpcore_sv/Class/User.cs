using System;
using System.Collections.Generic;
using System.Data.Common;
using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using vorpcore_sv.Utils;

namespace vorpcore_sv.Class
{
    //class for users that contains their characters
    public class User:BaseScript
    {
        private string _identifier; //User steamid
        private string _license; //User rockstar    
        private string _group;//User admin group
        private int _playerwarnings;//Used for admins to know how many warnings a user has
        private Dictionary<int,Character> _usercharacters;
        private int _numofcharacters;
        private int usedCharacterId = -1;
        private int source = -1;

        public int UsedCharacterId
        {
            get => usedCharacterId;
            set
            {
                usedCharacterId = value;
                PlayerList pl = new PlayerList();
                foreach (Player player in pl)
                {
                    string steamid = "steam:" + player.Identifiers["steam"];
                    if (steamid == Identifier)
                    {
                        source = int.Parse(player.Handle);
                        _usercharacters[value].Source = source;
                        player.TriggerEvent("vorp:SelectedCharacter",usedCharacterId);
                        JObject postUi = new JObject();
                        postUi.Add("type", "ui");
                        postUi.Add("action", "update");
                        postUi.Add("moneyquanty", _usercharacters[usedCharacterId].Money);
                        postUi.Add("goldquanty", _usercharacters[usedCharacterId].Gold);
                        postUi.Add("rolquanty", _usercharacters[usedCharacterId].Rol);
                        postUi.Add("serverId", player.Handle);
                        postUi.Add("xp", _usercharacters[usedCharacterId].Xp);


                        player.TriggerEvent("vorp:updateUi", postUi.ToString());
                        break;
                    }
                }
                
                TriggerEvent("vorp:SelectedCharacter", source, _usercharacters[usedCharacterId].getCharacter());

            }
        }

        public int Source
        {
            get => source;
            set => source = value;
        }
        public int Numofcharacters
        {
            get => _numofcharacters;
            set => _numofcharacters = value;
        }

        public string Identifier
        {
            get => _identifier;
        }

        public string License
        {
            get => _license;
            set
            {
                _license = value;
            }
        }

        public string Group
        {
            get => _group;
            set
            {
                _group = value;
                Exports["ghmattimysql"].execute("UPDATE users SET `group` = ? WHERE `identifier` = ?", new object[] { _group, Identifier });
            }
        }

        public int Playerwarnings
        {
            get => _playerwarnings;
            set
            {
                _playerwarnings = value;
                Exports["ghmattimysql"].execute("UPDATE users SET `warnings` = ? WHERE `identifier` = ?", new object[] { _playerwarnings, Identifier });
            }
        }
        

        public User(string identifier, string group, int playerwarnings, string license)
        {
            _identifier = identifier;
            _group = group;
            _playerwarnings = playerwarnings;
            _usercharacters = new Dictionary<int, Character>();
            _license = license;
            LoadCharacters(identifier);
            //Cargarmos todos sus characters de la base de datos si al cargarlos no tiene entonces cuando se llame a spawnpalyer habrá que crear 1
        }

        public Dictionary<string, dynamic> GetUser()
        {
            Dictionary<string,dynamic> character = new Dictionary<string, dynamic>();
            if (_usercharacters.ContainsKey(usedCharacterId))
            {
                character = _usercharacters[usedCharacterId].getCharacter();
            }
            List<Dictionary<string,dynamic>> userCharacters = new List<Dictionary<string,dynamic>>();
            foreach (KeyValuePair<int,Character> chara in _usercharacters)
            {
                userCharacters.Add(chara.Value.getCharacter());
            }
            Dictionary<string, dynamic> auxdic = new Dictionary<string, dynamic>
            {
                ["getIdentifier"] = Identifier,
                ["getGroup"] = Group,
                ["getPlayerwarnings"] = Playerwarnings,
                ["source"] = source,
                ["setGroup"] = new Action<string>((group) =>
                {
                    try
                    {
                        Group = group;
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                   
                }),
                ["setPlayerWarnings"] = new Action<int>((warnings) =>
                {
                    try
                    {
                        Playerwarnings = warnings;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }),
                ["getUsedCharacter"] = character,
                ["getUserCharacters"] = userCharacters,
                ["getNumOfCharacters"] = _numofcharacters,
                ["addCharacter"] = new Action<string, string, string, string>((firstname, lastname, skin, comps) => {
                    Numofcharacters++;
                    try
                    {
                        addCharacter(firstname, lastname, skin, comps);
                    }catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }),
                ["removeCharacter"] = new Action<int>((charid) => {
                    try
                    {
                        if (_usercharacters.ContainsKey(charid))
                        {
                            delCharacter(charid);
                        }
                    }catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    
                }),
                ["setUsedCharacter"] = new Action<int>((charid) => {
                    try
                    {
                        SetUsedCharacter(charid);
                    }catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                })
            };
            return auxdic;
        }

        private async void LoadCharacters(string identifier)
        {
            List<object> usercharacters = await Exports["ghmattimysql"].executeSync("SELECT * FROM characters WHERE identifier =?", new[] {identifier});
            Numofcharacters = usercharacters.Count;
            if (Numofcharacters > 0)
            {
                //Metemos todos los characters en el diccionario
                foreach (object icharacter in usercharacters)
                {
                    IDictionary<string, object> character = (dynamic)icharacter;
                    if (character.ContainsKey("identifier"))
                    {
                        Character newCharacter = new Character(identifier,(int) character["charidentifier"],(string)character["group"],
                            (string) character["job"],int.Parse(character["jobgrade"].ToString()),(string) character["firstname"],(string) character["lastname"]
                            ,(string) character["inventory"],
                            (string) character["status"],(string) character["coords"],double.Parse(character["money"].ToString())
                            ,double.Parse(character["gold"].ToString()),double.Parse(character["rol"].ToString()),int.Parse(character["xp"].ToString()), (bool)character["isdead"],(string)character["skinPlayer"],
                            (string)character["compPlayer"]);
                        if (_usercharacters.ContainsKey(newCharacter.CharIdentifier))
                        {
                            _usercharacters[newCharacter.CharIdentifier] = newCharacter;
                        }
                        else
                        {
                            _usercharacters.Add(newCharacter.CharIdentifier,newCharacter);
                        }
                    }
                }
            }
        }

        public async void addCharacter(string firstname, string lastname, string skin, string comps)
        {
            Character newChar = new Character(Identifier,"user", "none", 0, firstname, lastname, "{}", "{}", "{}", LoadConfig.Config["initMoney"].ToObject<double>(), LoadConfig.Config["initGold"].ToObject<double>(), LoadConfig.Config["initRol"].ToObject<double>(), LoadConfig.Config["initXp"].ToObject<int>(), false, skin, comps);
            int charidentifier = await newChar.SaveNewCharacterInDb();
            _usercharacters.Add(charidentifier, newChar);
            Debug.WriteLine("Added new character with identifier " + _usercharacters[charidentifier].PlayerVar.Identifiers["steam"]);
            UsedCharacterId = charidentifier;
        }

        public void delCharacter(int charIdentifier)
        {
            if (_usercharacters.ContainsKey(charIdentifier))
            {
                _usercharacters[charIdentifier].DeleteCharacter();
                _usercharacters.Remove(charIdentifier);
                Debug.WriteLine($"Character with charid {charIdentifier} deleted from user {Identifier} successfully");
            }
        }

        public Character GetUsedCharacter()
        {
            if (_usercharacters.ContainsKey(UsedCharacterId))//Comprobante para asegurarnos de que existe aunque lo demos por hecho
            {
                return _usercharacters[UsedCharacterId];
            }
            else
            {
                return null;
            }
        }

        public void SetUsedCharacter(int charid)
        {
            if (_usercharacters.ContainsKey(charid))
            {
                UsedCharacterId = charid;
            }
        }

        public void SaveUser()
        {
            foreach (KeyValuePair<int,Character> character in _usercharacters)
            {
                character.Value.SaveCharacterInDb();
            }
        }
    }
}