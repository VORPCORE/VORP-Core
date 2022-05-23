using Lusive.Events;
using System.Collections.Generic;
using System.Linq;
using Vorp.Shared.Records;

namespace Vorp.Core.Server.Events
{
    public class ClientId : ISource
    {
        public static readonly ClientId All = new(-1);

        public int Handle { get; set; }
        public string[] Identifiers { get; set; }
        public User User { get; private set; }

        public ClientId(int handle)
        {
            Handle = handle;

            var holder = new List<string>();

            for (var index = 0; index < GetNumPlayerIdentifiers(handle.ToString()); index++)
            {
                holder.Add(GetPlayerIdentifier(handle.ToString(), index));
            }

            if (PluginManager.UserSessions.ContainsKey(handle))
            {
                User = PluginManager.UserSessions[handle];
            }

            Identifiers = holder.ToArray();
        }

        public override string ToString()
        {
            var args = new List<string> { Handle.ToString() };

            return string.Join(", ", args);
        }

        public bool Compare(string[] identifiers)
        {
            return identifiers.Any(self => Identifiers.Contains(self));
        }

        public bool Compare(ClientId client)
        {
            return client.Handle == Handle;
        }

        public static explicit operator ClientId(string netId)
        {
            if (int.TryParse(netId.Replace("internal-", string.Empty).Replace("net:", string.Empty), out var handle))
            {
                return new ClientId(handle);
            }

            throw new Exception($"Could not parse net id: {netId}");
        }

        public static explicit operator ClientId(int handle) => new(handle);
        public static explicit operator string(ClientId client) => client.Handle.ToString();
    }
}
