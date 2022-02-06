global using CitizenFX.Core;
global using CitizenFX.Core.Native;
global using Newtonsoft.Json;
global using Vorp.Core.Client.RedM;
global using Vorp.Diagnostics;
global using Vorp.Shared.Diagnostics;
global using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vorp.Core.Client.Environment.Entities;
using Vorp.Core.Client.Events;
using Vorp.Core.Client.Managers;

namespace Vorp.Core.Client
{
    public class PluginManager : BaseScript
    {
        public static PluginManager Instance { get; private set; }
        public ClientGateway ClientGateway;
        public VorpPlayer LocalPlayer;
        public EventHandlerDictionary EventRegistry => EventHandlers;
        public ExportDictionary ExportDictionary => Exports;
        public Dictionary<Type, object> Managers { get; } = new Dictionary<Type, object>();
        public Dictionary<Type, List<MethodInfo>> TickHandlers { get; set; } = new Dictionary<Type, List<MethodInfo>>();
        public List<Type> RegisteredTickHandlers { get; set; } = new List<Type>();

        public PluginManager()
        {
            Instance = this;
            ClientGateway = new ClientGateway(this);

            EventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);

            OnLoad();
        }

        async Task OnLoad()
        {
            try
            {
                Logger.Info("Loading managers, please wait...");

                List<MethodInfo> managers = Assembly.GetExecutingAssembly().GetExportedTypes()
                    .SelectMany(self => self.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                    .Where(self => self.GetCustomAttribute(typeof(TickHandler), false) != null).ToList();

                managers.ForEach(self =>
                {
                    try
                    {
                        var type = self.DeclaringType;
                        if (type == null) return;

                        Logger.Debug($"[TickHandlers] {type.Name}::{self.Name}");

                        if (!TickHandlers.ContainsKey(type))
                        {
                            TickHandlers.Add(type, new List<MethodInfo>());
                        }

                        TickHandlers[type].Add(self);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"{ex}");
                        BaseScript.TriggerServerEvent("user:log:exception", $"Error with Tick; {ex.Message}", ex);
                    }
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

                Logger.Info($"[Managers] Successfully loaded in {loaded} manager(s)!");

                AttachTickHandlers(this);

                LocalPlayer = new VorpPlayer(PlayerId(), PlayerPedId());

                Logger.Info("Load method has been completed.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Plugin Manager Load Error");
            }
        }

        public void Hook(string eventName, Delegate @delegate)
        {
            EventHandlers[eventName] += @delegate;
        }

        private void OnResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        private void OnResourceStop(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            // Tell server to save

            Vector3 position = LocalPlayer.Position;
            float heading = LocalPlayer.Heading;

            ClientGateway.Send("vorp:character:coords:save", position, heading);
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

        public void AttachTickHandlers(object instance)
        {
            TickHandlers.TryGetValue(instance.GetType(), out var methods);

            methods?.ForEach(async self =>
            {
                try
                {
                    var handler = (TickHandler)self.GetCustomAttribute(typeof(TickHandler));

                    if (handler.SessionWait)
                    {
                        await Session.Loading();
                    }

                    Logger.Debug($"AttachTickHandlers -> {self.Name}");

                    Tick += (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), instance, self);

                    RegisteredTickHandlers.Add(instance.GetType());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"AttachTickHandlers");
                }
            });
        }

        public bool IsLoadingManager<T>() where T : Manager<T>, new()
        {
            return Managers.FirstOrDefault(self => self.Key == typeof(T)).Value is bool == false;
        }

        public object GetManager(Type type)
        {
            return Managers.FirstOrDefault(self => self.Key == type).Value;
        }

        public T GetManager<T>() where T : Manager<T>, new()
        {
            return (T)Managers.FirstOrDefault(self => self.Key == typeof(T)).Value;
        }

        public void AttachTickHandler(Func<Task> task)
        {
            Tick += task;
        }

        public void DetachTickHandler(Func<Task> task)
        {
            Tick -= task;
        }
    }
}
