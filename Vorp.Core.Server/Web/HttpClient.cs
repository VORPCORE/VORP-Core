using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Vorp.Core.Server.Web
{
    public struct RequestResponse
    {
        public HttpStatusCode status;
        public WebHeaderCollection headers;
        public string content;
    }

    public struct RequestDataInternal
    {
        public string url;
        public string method;
        public string data;
        public dynamic headers;
    }

    public class RequestInternal : BaseScript
    {
        public Dictionary<int, Dictionary<string, dynamic>> ResponseDictionary { get; } =
            new Dictionary<int, Dictionary<string, dynamic>>();

        public RequestInternal()
        {
            EventHandlers["__cfx_internal:httpResponse"] += new Action<int, int, string, dynamic>(Response);
            Exports.Add("HttpRequest",
                new Func<string, string, string, string, Task<Dictionary<string, dynamic>>>(Http));
        }

        public void Response(int token, int status, string text, dynamic header)
        {
            var response = new Dictionary<string, dynamic>
            {
                ["headers"] = header,
                ["status"] = status,
                ["content"] = text
            };

            ResponseDictionary[token] = response;
        }

        public async Task<Dictionary<string, dynamic>> Http(string url, string method, string data, dynamic headers)
        {
            var requestData = new RequestDataInternal { url = url, method = method, data = data, headers = headers };
            var json = JsonConvert.SerializeObject(requestData);
            var token = API.PerformHttpRequestInternal(json, json.Length);

            while (!ResponseDictionary.ContainsKey(token))
            {
                await Delay(0);
            }

            var response = ResponseDictionary[token];

            ResponseDictionary.Remove(token);

            return response;
        }
    }

    public class Request : BaseScript
    {
        public async Task<RequestResponse> Http(string url, string method = "GET", string data = "",
            Dictionary<string, string> headers = null)
        {
            headers = headers ?? new Dictionary<string, string>();

            return ParseRequestResponseInternal(
                await Exports[API.GetCurrentResourceName()].HttpRequest(url, method, data, headers)
            );
        }

        private WebHeaderCollection ParseHeadersInternal(dynamic headerDynamic)
        {
            var headers = new WebHeaderCollection();
            var headerDictionary = (IDictionary<string, object>)headerDynamic;

            foreach (var entry in headerDictionary)
            {
                headers.Add(entry.Key, entry.Value.ToString());
            }

            return headers;
        }

        private HttpStatusCode ParseStatusInternal(int status)
        {
            return (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), status);
        }

        private RequestResponse ParseRequestResponseInternal(IDictionary<string, dynamic> response)
        {
            var result = new RequestResponse
            {
                status = ParseStatusInternal(response["status"]),
                headers = ParseHeadersInternal(response["headers"]),
                content = response["content"]
            };

            return result;
        }
    }
}