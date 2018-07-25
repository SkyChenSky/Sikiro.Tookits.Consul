using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;

namespace Sikiro.Tookits.Consul
{
    public static class ConsulKyExtensions
    {
        public static async Task<bool> KvPutAsync(this ConsulClient consulClient, string key, string value)
        {
            var kvPair = new KVPair(key)
            {
                Value = Encoding.UTF8.GetBytes(value)
            };
            var result = await consulClient.KV.Put(kvPair);

            if (result.StatusCode == HttpStatusCode.OK)
                return result.Response;

            return false;
        }

        public static bool KvPut(this ConsulClient consulClient, string key, string value)
        {
            var kvPair = new KVPair(key)
            {
                Value = Encoding.UTF8.GetBytes(value)
            };
            var result = consulClient.KV.Put(kvPair).ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.StatusCode == HttpStatusCode.OK)
                return result.Response;

            return false;
        }

        public static async Task<string> KvGetAsync(this ConsulClient consulClient, string key)
        {
            var result = await consulClient.KV.Get(key);

            return Encoding.UTF8.GetString(result.Response.Value);
        }

        public static string KvGet(this ConsulClient consulClient, string key)
        {
            var result = consulClient.KV.Get(key).ConfigureAwait(false).GetAwaiter().GetResult();

            return Encoding.UTF8.GetString(result.Response.Value);
        }

        public static async Task<bool> KvDeleteAsync(this ConsulClient consulClient, string key)
        {
            var result = await consulClient.KV.Delete(key);

            if (result.StatusCode == HttpStatusCode.OK)
                return result.Response;

            return false;
        }

        public static bool KvDelete(this ConsulClient consulClient, string key)
        {
            var result = consulClient.KV.Delete(key).ConfigureAwait(false).GetAwaiter().GetResult();

            if (result.StatusCode == HttpStatusCode.OK)
                return result.Response;

            return false;
        }
    }
}
