using IdentityModel.Client;
using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Web.API.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Net.Http.Headers;
using System.Net;

namespace Trepub.Web.API.Test.Common
{
    public abstract class BaseAppClient
    {
        protected TokenResponse tokenResponse;
        protected IEnumerable<Claim> claims;
        private TimeSpan _timeOut;
        private HttpClient __client;
        protected string baseUrl;

        public BaseAppClient() : this(null, new TimeSpan()) { }

        public BaseAppClient(string baseUrl) : this(baseUrl, new TimeSpan()) { }

        public BaseAppClient(TimeSpan timeOut) : this(null, timeOut)
        {
            _timeOut = timeOut;
        }
        public BaseAppClient(string baseUrl, TimeSpan timeOut)
        {
            //baseUrl
            if (String.IsNullOrEmpty(baseUrl))
            {
                this.baseUrl = "http://localhost:56224";
            }
            else
            {
                this.baseUrl = baseUrl;
            }

            //setting default timeout value
            if (timeOut == null || timeOut.TotalSeconds < 10)
            {
                _timeOut = new TimeSpan(0, 5, 0);
            }
            else
            {
                this._timeOut = timeOut;
            }

            GetDiscoveryDocument();

        }


        public T Post<T>(T t, string path)
        {
            return Post<T, T>(t, path);
        }

        public R Post<T, R>(T t, string path)
        {
            var oiSerialized = JsonConvert.SerializeObject(t);
            var content = new StringContent(oiSerialized, Encoding.UTF8, "application/json");
            var response = Client.PostAsync(path, content).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errorResponseString = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponseView>(errorResponseString);
                throw new AppException(errorResponse.ErrorCode, errorResponse.Message);
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<R>(responseString);
            return result;
        }

        public R PostForm<R>(MultipartFormDataContent content, string path)
        {
            var response = Client.PostAsync(path, content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errorResponseString = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponseView>(errorResponseString);
                throw new AppException(errorResponse.ErrorCode, errorResponse.Message);
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
            var responseString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<R>(responseString);
            return result;
        }


        public void PostForm(string key, string value, string path)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(key, value),
            });
            var res = Client.PostAsync(path, formContent).Result;
        }

        public T Get<T>(string path)
        {
            var response = Client.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }

        public string GetFileString(string path)
        {
            var response = Client.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            return stringResponse;
        }


        public byte[] GetFileRange(string path, long from, long to)
        {
            HttpRequestMessage r = new HttpRequestMessage();
            r.Method = HttpMethod.Get;
            r.Headers.Range = new RangeHeaderValue(from, to);
            //r.Headers.Range.Ranges.Add(new RangeItemHeaderValue(from, to));
            r.RequestUri = new Uri(Client.BaseAddress.ToString() + path);
            var response = Client.SendAsync(r).Result;
            response.EnsureSuccessStatusCode();
            var bytes = new byte[to - from + 1];
            using (var stream = response.Content.ReadAsStreamAsync().Result)
            {
                var bytesread = stream.Read(bytes, 0, bytes.Length);
                stream.Close();
            }
            return bytes;
        }


        public void Delete(string path)
        {
            var result = Client.DeleteAsync(path).Result;
            result.EnsureSuccessStatusCode();
            Thread.Sleep(1000);
        }

        protected HttpClient Client
        {
            get
            {
                if (__client == null)
                {
                    __client = new HttpClient()
                    {
                        BaseAddress = new Uri(baseUrl),
                        Timeout = _timeOut
                    };
                }
                if (tokenResponse != null)
                {
                    __client.SetBearerToken(tokenResponse.AccessToken);
                }
                else
                {
                    __client.SetBearerToken(null);
                }

                return __client;
            }
        }

        public void Logout()
        {
            tokenResponse = null;
            claims = null;
        }

        protected void GetDiscoveryDocument()
        {
            //if (endpoint == null)
            //{
            //    try
            //    {
            //        endpoint = DiscoveryClient.GetAsync(baseUrl).Result;
            //        if (endpoint.IsError)
            //        {
            //            string errorMessage = "Failed to get discovery document. \n ERROR: " + endpoint.Error;
            //            throw new Exception(errorMessage);
            //        }
            //    }
            //    catch (Exception exc)
            //    {
            //        string errorMessage = "Failed to get discovery document. \n ERROR: " + exc.Message;
            //        throw new Exception(errorMessage, exc);
            //    }
            //}

        }

    }
}
