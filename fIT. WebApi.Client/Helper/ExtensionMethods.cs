using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Helper
{
    public static class ExtensionMethods
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            T result = default(T);
            var stringcontent = await content.ReadAsStringAsync();
            try
            {
                result = (T) JsonConvert.DeserializeObject(stringcontent, typeof (T));
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string url, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            return await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "text/json"));
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string url, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            return await client.PutAsync(url, new StringContent(json, Encoding.UTF8, "text/json"));
        }

    }
}
