using ICMSConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ICMSConnector.Controllers
{
    class HandbackController: IHandbackManager
    {
        public Task<string> RunHandback(HttpClient HttpClient)
        {
            var response = HttpClient.GetAsync(string.Format("api/LocFiles/{0}","4001072"));
            var stringResponse = response.Result.Content.ReadAsStringAsync();

            return stringResponse;
        }
    }
}
