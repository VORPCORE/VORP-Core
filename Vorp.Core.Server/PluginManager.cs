global using CitizenFX.Core;
global using Newtonsoft.Json;
global using System;
global using Vorp.Core.Server.Attributes;
global using Vorp.Shared;
global using Vorp.Shared.Diagnostics;
global using static CitizenFX.Core.Native.API;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vorp.Core.Server.Commands;
using Vorp.Core.Server.Commands.Impl;
using Vorp.Core.Server.Events;
using Vorp.Core.Server.Managers;
using Vorp.Shared.Records;

namespace Vorp.Core.Server
{
    public class PluginManager : BaseScript
    {
        public static PluginManager Instance { get; private set; }
        public static PlayerList PlayersList { get; private set; }
        public EventHandlerDictionary EventRegistry => EventHandlers;
        public ExportDictionary ExportDictionary => Exports;
        public Dictionary<Type, object> Managers { get; } = new Dictionary<Type, object>();
        public Dictionary<Type, List<MethodInfo>> TickHandlers { get; set; } = new Dictionary<Type, List<MethodInfo>>();
        public List<Type> RegisteredTickHandlers { get; set; } = new List<Type>();
        public ServerGateway Events;
        public static ConcurrentDictionary<int, User> UserSessions = new();
        public static bool IsOneSyncEnabled => GetConvar("onesync", "off") != "off";
        public CommandFramework CommandFramework;
        public bool IsServerReady = false;

        public PluginManager()
        {
            Instance = this;
            Events = new ServerGateway(Instance);

            Load();
        }

        public void Hook(string eventName, Delegate @delegate)
        {
            Logger.Debug($"Registered Legacy Event Handler '{eventName}'");
            EventHandlers.Add(eventName, @delegate);
        }

        public static Player ToPlayer(int handle)
        {
            return Instance.Players[handle];
        }

        public static User ToUser(string handle)
        {
            if (int.TryParse(handle, out int iHandle))
                return ToUser(iHandle);
            return null;
        }

        public static User ToUser(int handle)
        {
            if (!UserSessions.ContainsKey(handle)) return null;
            return UserSessions[handle];
        }

        private async void Load()
        {
            Logger.Info($"INIT");

            PlayersList = Players;

            Assembly.GetExecutingAssembly().GetExportedTypes()
                .SelectMany(self => self.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(self => self.GetCustomAttribute(typeof(TickHandler), false) != null).ToList()
                .ForEach(self =>
                {
                    var type = self.DeclaringType;

                    if (type == null) return;

                    if (!TickHandlers.ContainsKey(type))
                    {
                        TickHandlers.Add(type, new List<MethodInfo>());
                    }

                    Logger.Debug($"[TickHandlers] {type.Name}::{self.Name}");

                    TickHandlers[type].Add(self);
                });

            var loaded = 0;

            foreach (var type in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (type.BaseType == null) continue;
                if (!type.BaseType.IsGenericType) continue;

                var generic = type.BaseType.GetGenericTypeDefinition();

                if (generic != typeof(Manager<>) || type == typeof(Manager<>)) continue;

                LoadManager(type);

                loaded++;
            }

            foreach (var manager in Managers)
            {
                var method = manager.Key.GetMethod("Begin", BindingFlags.Public | BindingFlags.Instance);

                method?.Invoke(manager.Value, null);
            }

            CommandFramework = new CommandFramework();
            CommandFramework.Bind(typeof(AdminCommands));

            Logger.Info($"[Managers] Successfully loaded in {loaded} manager(s)!");

            bool databaseTest = await Database.DapperDatabase<bool>.GetSingleAsync("select 1;");
            if (databaseTest)
            {
                Logger.Trace($"Database Connection Test Successful!");
            }
            else
            {
                Logger.Error($"Database Connection Test Failed!");
            }

            IsServerReady = true;
            Logger.Info($"VORP CORE - LOAD COMPLETED");

            BaseScript.TriggerEvent("vorp:server:activated");
            Logger.Info($"VORP CORE - Broadcasted event 'vorp:server:activated' to let other resources know the CORE is ready");
        }

        public object LoadManager(Type type)
        {
            if (GetManager(type) != null) return null;

            Logger.Debug($"Loading in manager of type `{type.FullName}`");

            Managers.Add(type, default(Type));

            var instance = Activator.CreateInstance(type);

            AttachTickHandlers(instance);
            Managers[type] = instance;

            return instance;
        }

        public bool IsLoadingManager<T>() where T : Manager<T>, new()
        {
            return !(Managers.FirstOrDefault(self => self.Key == typeof(T)).Value is bool);
        }

        public object GetManager(Type type)
        {
            return Managers.FirstOrDefault(self => self.Key == type).Value;
        }

        public T GetManager<T>() where T : Manager<T>, new()
        {
            return (T)Managers.FirstOrDefault(self => self.Key == typeof(T)).Value;
        }

        public void AttachTickHandlers(object instance)
        {
            TickHandlers.TryGetValue(instance.GetType(), out var methods);

            methods?.ForEach(self =>
            {
                var handler = (TickHandler)self.GetCustomAttribute(typeof(TickHandler));

                Tick += (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), instance, self);

                RegisteredTickHandlers.Add(instance.GetType());
            });
        }
        public static Player GetPlayer(int netID)
        {
            return PlayersList[netID];
        }

        public void AttachTickHandler(Func<Task> task)
        {
            Tick += task;
        }

        public void DetachTickHandler(Func<Task> task)
        {
            Tick -= task;
        }

        public bool IsUserActive(string steamIdentifier)
        {
            try
            {
                Dictionary<int, User > userDictionary = new Dictionary<int, User>(UserSessions);
                foreach (KeyValuePair<int, User> kvp in userDictionary)
                {
                    User user = kvp.Value;

                    if (user == null) return false;

                    if (user.SteamIdentifier == steamIdentifier)
                    {
                        // if the currently known user returns an endpoint with that server Id, then they are still connected.
                        // if it returns nothing, then the server should allow the connection and pass their current data back
                        // this should also skip any additional loading from the database
                        if (user.CFXServerID == 0)
                            return false;

                        return !string.IsNullOrEmpty(user.Endpoint);
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"IsUserActive: {steamIdentifier}");
                return false;
            }
        }
    }
}