using System.Net.Http;
using ICMSConnector.Interfaces;

namespace ICMSConnector.Controllers
{
    class PingController: IPing
    {
        public string Ping(HttpClient _httpClient)
        {
            var response = _httpClient.GetAsync("api/Ping");
            var responseString = response.Result.Content.ReadAsStringAsync();
            return responseString.Result;
        }
    }
}
