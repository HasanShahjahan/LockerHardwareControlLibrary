﻿using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;

namespace LSS.BE.Core.Security.Handlers
{
    public class HttpHandler : IHttpHandler
    {
        private readonly static HttpClient client = new HttpClient();
        public HttpHandler(string uriString)
        {
            Uri baseUri = new Uri(uriString);
            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpResponseMessage GetTokenAsync(string version, string uriPath, string clientId, string clientSecret)
        {
            
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            var content = new FormUrlEncodedContent(values);

            var authenticationString = $"{clientId}:{clientSecret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/" + version + "/" + uriPath);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            requestMessage.Content = content;

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            return response;
        }
        
        public HttpResponseMessage PostAsync(string request, HttpMethod httpMethod, string version, string uriPath, string type, string token)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(httpMethod, "/" + version + "/" + uriPath);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(type, token);
            requestMessage.Content = content;

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            return response;
        }

        public HttpResponseMessage GetAsync(Dictionary<string, string> queryParams, string version, string uriPath, string type, string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/" + version + "/" + uriPath, queryParams));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(type, token);

            var task = client.SendAsync(requestMessage);
            var response = task.Result;
            return response;
        }
    }
}
