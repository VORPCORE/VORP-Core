using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using vorpcore_sv.Scripts;

namespace vorpcore_sv.Utils
{
    public class LoadConfig : BaseScript
    {
        public static JObject Config = new JObject();
        public static string ConfigString;
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        public static string resourcePath = $"{API.GetResourcePath(API.GetCurrentResourceName())}";

        public static bool isConfigLoaded = false;

        public LoadConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:getConfig"] += new Action<Player>(getConfig);
            LoadConfigAndLang();
        }

        private void LoadConfigAndLang()
        {
            if (File.Exists($"{resourcePath}/Config.json"))
            {
                ConfigString = File.ReadAllText($"{resourcePath}/Config.json", Encoding.UTF8);
                Config = JObject.Parse(ConfigString);
                if (File.Exists($"{resourcePath}/{Config["defaultlang"]}.json"))
                {
                    string langstring = File.ReadAllText($"{resourcePath}/{Config["defaultlang"]}.json", Encoding.UTF8);
                    Langs = JsonConvert.DeserializeObject<Dictionary<string, string>>(langstring);
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: Language {Config["defaultlang"]}.json loaded!");
                }
                else
                {
                    Debug.WriteLine($"{API.GetCurrentResourceName()}: {Config["defaultlang"]}.json Not Found");
                }
            }
            else
            {
                Debug.WriteLine($"{API.GetCurrentResourceName()}: Config.json Not Found");
            }
            isConfigLoaded = true;
            if (Config["Whitelist"].ToObject<bool>() != null)
            {
                LoadUsers._usingWhitelist = Config["Whitelist"].ToObject<bool>();
                if (LoadUsers._usingWhitelist)
                {
                    LoadWhitelist();
                }
            }
        }

        private void LoadWhitelist()
        {
            Exports["ghmattimysql"].execute("SELECT * FROM whitelist", new[] { "" }, new Action<dynamic>((result) =>
            {
                if (result.Count > 0)
                {
                    foreach (var r in result)
                    {
                        LoadUsers._whitelist.Add(r.identifier);
                    }
                }
            }));
        }

        private void getConfig([FromSource] Player source)
        {
            source.TriggerEvent($"{API.GetCurrentResourceName()}:SendConfig", ConfigString, Langs);
        }
    }

}
