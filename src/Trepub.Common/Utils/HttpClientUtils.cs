using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Trepub.Common.Utils
{
    public static class HttpClientUtils
    {
        public static R Get<R>(this HttpClient client, string path)
        {
            var response = client.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<R>(responseString);
            return result;
        }
        public static R Post<T, R>(this HttpClient client, T t, string path)
        {
            var oiSerialized = JsonConvert.SerializeObject(t);
            var content = new StringContent(oiSerialized, Encoding.UTF8, "application/json");
            var response = client.PostAsync(path, content).Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<R>(responseString);
            return result;
        }


        public static R PostNoEncoding<T, R>(this HttpClient client, T t, string path)
        {
            var oiSerialized = JsonConvert.SerializeObject(t);
            var content = new StringContent(oiSerialized);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = client.PostAsync(path, content).Result;
            response.EnsureSuccessStatusCode();
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<R>(responseString);
            return result;
        }

    }
}
