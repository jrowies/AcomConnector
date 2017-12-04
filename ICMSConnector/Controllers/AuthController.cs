using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ICMSConnector.Controllers
{
    class AuthController
    {
        public HttpClient HttpClient { get; private set; }
        /// <summary>
        /// Create a new authorized HttpClient
        /// </summary>
        public AuthController()

        {
            HttpClient = new HttpClient();
            //The URI to use when connecting to the Translation Service API
            var translationServiceURI = new Uri(@"https://translationservicedebug.cloudapp.net");
            var authContext = new AuthenticationContext("https://login.microsoftonline.com/microsoft.onmicrosoft.com");

            //Specify authentication information and user credentials to get usable token 
            var response = authContext.AcquireTokenAsync("https://icmsauthorizer.azurewebsites.net/", "a8678b53-df7b-4a0c-be1a-644c9bd2e6d9", new UserPasswordCredential("yourusername","yourpassword"));
            var token = response.Result.AccessToken;

            //Setting up HttpClient
            HttpClient.BaseAddress = translationServiceURI;
            HttpClient.DefaultRequestHeaders.Accept.Clear();

            //Content to be returned
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Request ID to keep track of the connector's requests
            HttpClient.DefaultRequestHeaders.Add("RequestId", "test-connector-acom");

            //Specify tenant to translation service
            HttpClient.DefaultRequestHeaders.Add("Tenant", "C&E");

            //Specify Host Content Store 
            HttpClient.DefaultRequestHeaders.Add("HostContentStore", "TestInProduction");          

            //Set up appropriate authorization header
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
