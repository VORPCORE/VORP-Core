using CitizenFX.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vorpcore_sv.Class
{
    //Class for user characters
    public class Character : BaseScript
    {
        private string identifier;
        private int charIdentifier;
        private string group;
        private string job;
        private int jobgrade;
        private string firstname;
        private string lastname;
        private string inventory;
        private string status;
        private string coords;
        private string skin;
        private string comps;

        private double money;
        private double gold;
        private double rol;

        private int xp;

        private bool isdead;

        private Player userPlayer;
        private int source;

        public int Source { set => source = value; }

        public Player PlayerVar
        {
            get
            {
                PlayerList pl = vorpcore_sv.PlayerList;
                return pl[source];
            }
        }

        public string Identifier { get => identifier; }
        public int CharIdentifier { get => charIdentifier; set => charIdentifier = value; }
        public string Group { get => group; }
        public string Job { get => job; }
        public int Jobgrade { get => jobgrade; set => jobgrade = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        public string Inventory { get => inventory; set => inventory = value; }
        public string Status { get => status; set => status = value; }
        public string Coords { get => coords; set => coords = value; }
        public double Money { get => money; }
        public double Gold { get => gold; }
        public double Rol { get => rol; }
        public int Xp { get => xp; }
        public bool IsDead { get => isdead; }

        public string Skin
        {
            get => skin;
            set
            {
                skin = value;
                Exports["ghmattimysql"].execute("UPDATE characters SET `skinPlayer` = ? WHERE `identifier` = ? AND `charidentifier` = ?", new object[] { value, Identifier, CharIdentifier });
            }
        }

        public string Comps
        {
            get => comps;
            set
            {
                comps = value;
                Exports["ghmattimysql"].execute("UPDATE characters SET `compPlayer` = ? WHERE `identifier` = ? AND `charidentifier` = ?", new object[] { value, Identifier, CharIdentifier });
            }
        }

        public Character(string identifier)
        {
            this.identifier = identifier;
            this.job = "unemployed";
            this.group = "user";
            this.inventory = "{}";
        }

        public Character(string identifier, int charIdentifier, string group, string job, int jobgrade, string firstname, string lastname, string inventory, string status, string coords, double money, double gold, double rol, int xp, bool isdead, string skin, string comps)
        {
            this.identifier = identifier;
            this.charIdentifier = charIdentifier;
            this.group = group;
            this.job = job;
            this.jobgrade = jobgrade;
            this.firstname = firstname;
            this.lastname = lastname;
            this.inventory = inventory;
            this.status = status;
            this.coords = coords;
            this.money = money;
            this.gold = gold;
            this.rol = rol;
            this.xp = xp;
            this.isdead = isdead;
            this.skin = skin;
            this.comps = comps;
        }

        public Character(string identifier, string group, string job, int jobgrade, string firstname, string lastname, string inventory, string status, string coords, double money, double gold, double rol, int xp, bool isdead, string skin, string comps)
        {
            this.identifier = identifier;
            this.group = group;
            this.job = job;
            this.jobgrade = jobgrade;
            this.firstname = firstname;
            this.lastname = lastname;
            this.inventory = inventory;
            this.status = status;
            this.coords = coords;
            this.money = money;
            this.gold = gold;
            this.rol = rol;
            this.xp = xp;
            this.isdead = isdead;
            this.skin = skin;
            this.comps = comps;
        }

        public Dictionary<string, dynamic> getCharacter()
        {
            Dictionary<string, dynamic> userData = new Dictionary<string, dynamic>();
            userData.Add("identifier", identifier);
            userData.Add("charIdentifier", charIdentifier);
            userData.Add("group", group);
            userData.Add("job", job);
            userData.Add("jobGrade", jobgrade);
            userData.Add("money", money);
            userData.Add("gold", gold);
            userData.Add("rol", rol);
            userData.Add("xp", xp);
            userData.Add("firstname", firstname);
            userData.Add("lastname", lastname);
            userData.Add("inventory", inventory);
            userData.Add("status", status);
            userData.Add("coords", coords);
            userData.Add("isdead", isdead);
            userData.Add("skin", skin);
            userData.Add("comps", comps);
            userData.Add("setStatus", new Action<string>((status) =>
            {
                try
                {
                    Status = status;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setJobGrade", new Action<int>((jobgrade) =>
            {
                try
                {
                    Jobgrade = jobgrade;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setGroup", new Action<string>((g) =>
            {
                try
                {
                    group = g;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setJob", new Action<string>((j) =>
            {
                try
                {
                    job = j;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setMoney", new Action<double>((m) =>
            {
                try
                {
                    money = m;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setGold", new Action<double>((g) =>
            {
                try
                {
                    gold = g;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setRol", new Action<double>((r) =>
            {
                try
                {
                    rol = r;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }));
            userData.Add("setXp", new Action<int>((x) =>
            {
                try
                {
                    xp = x;
                    updateCharUi();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setFirstname", new Action<string>((f) =>
            {
                try
                {
                    firstname = f;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("setLastname", new Action<string>((l) =>
            {
                try
                {
                    lastname = l;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("updateSkin", new Action<string>((nskin) =>
             {
                 try
                 {
                     Skin = nskin;
                 }
                 catch (Exception e)
                 {
                     Debug.WriteLine(e.Message);
                 }
             }));
            userData.Add("updateComps", new Action<string>((ncomps) =>
             {
                 try
                 {
                     Comps = ncomps;
                 }
                 catch (Exception e)
                 {
                     Debug.WriteLine(e.Message);
                 }
             }));
            userData.Add("addCurrency", new Action<int, double>((currency, quantity) =>
            {
                try
                {
                    addCurrency(currency, quantity);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("removeCurrency", new Action<int, double>((currency, quantity) =>
            {
                try
                {
                    removeCurrency(currency, quantity);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("addXp", new Action<int>((xp) =>
            {
                try
                {
                    addXp(xp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            userData.Add("removeXp", new Action<int>((xp) =>
            {
                try
                {
                    removeXp(xp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }));
            return userData;
        }

        public void updateCharUi()
        {
            JObject nuipost = new JObject();
            nuipost.Add("type", "ui");
            nuipost.Add("action", "update");
            nuipost.Add("moneyquanty", Money);
            nuipost.Add("goldquanty", Gold);
            nuipost.Add("rolquanty", Rol);
            nuipost.Add("xp", Xp);
            nuipost.Add("serverId", source.ToString());

            PlayerVar.TriggerEvent("vorp:updateUi", nuipost.ToString());
        }

        public void addCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    money += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET money=money + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    gold += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    rol += quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol + ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
            }
            updateCharUi();
        }

        public void removeCurrency(int currency, double quantity)
        {
            switch (currency)
            {
                case 0:
                    money -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET money=money - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 1:
                    gold -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET gold=gold - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
                case 2:
                    rol -= quantity;
                    //Exports["ghmattimysql"].execute($"UPDATE characters SET rol=rol - ? WHERE identifier=?", new object[] { quantity, identifier });
                    break;
            }
            updateCharUi();
        }

        public void addXp(int quantity)
        {
            xp += quantity;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp + ? WHERE identifier=?", new object[] { quantity, identifier });
            updateCharUi();
        }

        public void removeXp(int quantity)
        {
            xp -= quantity;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET xp=xp - ? WHERE identifier=?", new object[] { quantity, identifier });
            updateCharUi();
        }

        public void setJob(string newjob)
        {
            job = newjob;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET job=? WHERE identifier=?", new string[] { newjob, identifier });
        }

        public void setGroup(string newgroup)
        {
            group = newgroup;
            //Exports["ghmattimysql"].execute($"UPDATE characters SET `group`=? WHERE identifier=?", new string[] { newgroup.ToString(), identifier });
        }

        public void setDead(bool dead)
        {
            isdead = dead;
            //int intdead = dead ? 1 : 0;
            //Exports["ghmattimysql"].execute("UPDATE characters SET `isdead` = ? WHERE `identifier` = ?", new object[] { intdead, identifier });
        }

        public async Task<int> SaveNewCharacterInDb()
        {
            dynamic character = await Exports["ghmattimysql"].executeSync("INSERT INTO characters(`identifier`,`group`,`money`,`gold`,`rol`,`xp`,`inventory`,`job`,`status`,`firstname`,`lastname`,`skinPlayer`,`compPlayer`,`jobgrade`,`coords`,`isdead`) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)", new object[] { identifier, group, money, gold, rol, xp, inventory, job, status, firstname, lastname, skin, comps, jobgrade, coords, isdead ? 1 : 0 });
            CharIdentifier = (int)character.insertId;
            return (int)character.insertId;
        }

        public void DeleteCharacter()
        {
            Exports["ghmattimysql"].execute("DELETE FROM characters WHERE `identifier` = ? AND `charidentifier` = ? ", new object[] { Identifier, CharIdentifier });
        }

        public void SaveCharacterCoords(string coords)
        {
            Coords = coords;
            Exports["ghmattimysql"].execute("UPDATE characters SET `coords` = ? WHERE `identifier` = ? AND `charidentifier` = ?", new object[] { coords, Identifier, CharIdentifier });
        }

        public void SaveCharacterInDb()
        {
            Exports["ghmattimysql"].execute("UPDATE characters SET `group` = ?,`money` = ?,`gold` = ?,`rol` = ?,`xp` = ?,`job` = ?, `status` = ?,`firstname` = ?, `lastname` = ?, `jobgrade` = ?,`coords` = ?,`isdead` = ? WHERE `identifier` = ? AND `charidentifier` = ?", new object[] { group, money, gold, rol, xp, job, status, firstname, lastname, jobgrade, coords, isdead ? 1 : 0, identifier, charIdentifier });
        }
    }
}
