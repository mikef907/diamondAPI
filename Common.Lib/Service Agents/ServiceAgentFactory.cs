using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Common.Lib.Service_Agents
{
    public class ServiceAgentFactory
    {
        private HttpClient _http;
        public ServiceAgentFactory(HttpClient client) => _http = client;
        public HttpClient CreateHttpClient() => _http ?? new HttpClient();
    }
}
