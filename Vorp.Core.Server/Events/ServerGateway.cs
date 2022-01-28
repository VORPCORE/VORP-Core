using Lusive.Events;
using Lusive.Events.Diagnostics;
using Lusive.Events.Exceptions;
using Lusive.Events.Message;
using Lusive.Events.Serialization;
using Lusive.Events.Serialization.Implementations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Vorp.Core.Server.Events
{
    public class ServerGateway : BaseGateway
    {
        public const string SignaturePipeline = "moonlight_event_sig";

        protected sealed override IEventLogger Logger { get; }
        protected override ISerialization Serialization { get; }

        private readonly Dictionary<int, string> _signatures = new();

        public ServerGateway(PluginManager script)
        {
            Logger = new Logger();
            Serialization = new BinarySerialization(Logger);
            DelayDelegate = async delay => await BaseScript.Delay(delay);
            PushDelegate = Push;

            script.Hook(EventConstant.InboundPipeline, new Action<string, byte[]>(Inbound));
            script.Hook(EventConstant.OutboundPipeline, new Action<string, byte[]>(Outbound));
            script.Hook(SignaturePipeline, new Action<string>(GetSignature));
        }

        public void Push(string pipeline, ISource source, byte[] buffer)
        {
            if (source.Handle != ClientId.All.Handle)
                BaseScript.TriggerClientEvent(PluginManager.ToPlayer(source.Handle), pipeline, buffer);
            else
                BaseScript.TriggerClientEvent(pipeline, buffer);
        }

        private void GetSignature([FromSource] string source)
        {
            var client = (ClientId)source;

            if (_signatures.ContainsKey(client.Handle))
            {
                Logger.Info($"Client {client} tried retrieving event signature more than once.");

                return;
            }

            var holder = new byte[64];

            using (var service = new RNGCryptoServiceProvider())
            {
                service.GetBytes(holder);
            }

            var signature = BitConverter.ToString(holder);

            _signatures.Add(client.Handle, signature);
            BaseScript.TriggerClientEvent(PluginManager.ToPlayer(client.Handle), SignaturePipeline,
                signature);
        }

        private async void Inbound([FromSource] string source, byte[] buffer)
        {
            var client = (ClientId)source;

            if (!_signatures.TryGetValue(client.Handle, out var signature)) return;

            using var context =
                new SerializationContext(EventConstant.InboundPipeline, "(Gateway) Invoke", Serialization, buffer);
            var message = context.Deserialize<EventMessage>();

            if (!VerifySignature(client, message, signature)) return;

            try
            {
                await ProcessInboundAsync(message, client);
            }
            catch (EventTimeoutException)
            {
                DropPlayer(client.Handle.ToString(), $"Operation failed: {message.Endpoint} (Timed Out)");
            }
        }

        private void Outbound([FromSource] string source, byte[] buffer)
        {
            var client = (ClientId)source;

            if (!_signatures.TryGetValue(client.Handle, out var signature)) return;

            using var context =
                new SerializationContext(EventConstant.OutboundPipeline, "(Gateway) Reply", Serialization, buffer);
            var response = context.Deserialize<EventResponseMessage>();

            if (!VerifySignature(client, response, signature)) return;

            ProcessOutbound(response);
        }

        public bool VerifySignature(ISource source, IMessage message, string signature)
        {
            if (message.Signature == signature) return true;

            Logger.Info($"[{message.Endpoint}] Client {source} had invalid event signature, aborting:");
            Logger.Info($"[{message.Endpoint}] \tSupplied Signature: {message.Signature}");
            Logger.Info($"[{message.Endpoint}] \tActual Signature: {message.Signature}");

            return false;
        }

        public void Send(Player player, string endpoint, params object[] args) => Send(player.Handle, endpoint, args);

        public void Send(string target, string endpoint, params object[] args) =>
            Send((ClientId)target, endpoint, args);

        public void Send(ClientId client, string endpoint, params object[] args) => Send(client.Handle, endpoint, args);

        public void Send(int target, string endpoint, params object[] args)
        {
            SendInternal(EventFlowType.Straight, new ClientId(target), endpoint, args);
        }

        public Task<T> Get<T>(Player player, string endpoint, params object[] args) where T : class =>
            Get<T>(player.Handle, endpoint, args);

        public Task<T> Get<T>(string target, string endpoint, params object[] args) where T : class =>
            Get<T>((ClientId)target, endpoint, args);

        public Task<T> Get<T>(int target, string endpoint, params object[] args) where T : class =>
            Get<T>(new ClientId(target), endpoint, args);

        public async Task<T> Get<T>(ClientId client, string endpoint, params object[] args) where T : class
        {
            return await GetInternal<T>(client, endpoint, args);
        }
    }
}
