﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace CustomVoice_API.API
{
    class APIHelper
    {
        public static T Get<T>(string subscriptionKey, string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                var response = client.GetAsync(url, CancellationToken.None).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    PrintErrorMessage(response);
                    var items = JsonConvert.DeserializeObject<T>(" ");
                    return items;
                }

                using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                using (var streamReader = new StreamReader(responseStream))
                {
                    string responseJson = streamReader.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<T>(responseJson);
                    return items;
                }
            }
        }

        public static HttpResponseMessage Submit<T>(string subscriptionKey, string url, T definition)
        {
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                return client.PostAsJsonAsync(url, definition, CancellationToken.None).Result;
            }
        }

        public static HttpResponseMessage Delete(string subscriptionKey, string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                return client.DeleteAsync(url, CancellationToken.None).Result;
            }
        }

        public static string GetToken(string issueTokenUrl, string subscriptionKey)
        {
            var ibizaStsUrl = new Uri(issueTokenUrl);
            var authentication = new Authentication(ibizaStsUrl, subscriptionKey);
            return authentication.RetrieveNewTokenAsync();
        }

        public static void PrintErrorMessage(HttpResponseMessage response)
        {
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Status ReasonPhrase: {response.ReasonPhrase}");
        }
    }
}
