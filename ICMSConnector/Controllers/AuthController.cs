using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ICMSConnector.Controllers
{
    class AuthController
    {
        public HttpClient HttpClient { get; private set; }
        public AuthController()
        {
            HttpClient = new HttpClient();
            var translationServiceURI = new Uri(@"https://translationservicedebug.cloudapp.net");
            HttpClient.BaseAddress = translationServiceURI;
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.Add("RequestId", "test-connector-acom");
            HttpClient.DefaultRequestHeaders.Add("Tenant", "C&E");
            HttpClient.DefaultRequestHeaders.Add("HostContentStore", "TestNewOrg");
            var authContext = new AuthenticationContext("https://login.microsoftonline.com/microsoft.onmicrosoft.com");
            var response = authContext.AcquireTokenAsync("https://icmsauthorizer.azurewebsites.net/", "a8678b53-df7b-4a0c-be1a-644c9bd2e6d9", new UserPasswordCredential("awscfm@microsoft.com", "NewYearLongRun17#"));
            var token = response.Result.AccessToken;
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        }
    }
}
