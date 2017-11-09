using System;
using System.Collections.Generic;
using System.IO;
using ICMSConnector.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICMSConnector
{
    class Program
    {
        public static void Main(string[] args)
        {
            AuthController auth = new AuthController();
            PingController ping = new PingController();
            HandoffController handoff = new HandoffController();
            HandbackController handback = new HandbackController();

            var responseText = ping.Ping(auth.HttpClient);
            var HandoffResponse = handoff.Handoff(auth.HttpClient).Result;

            var ho_json = JsonConvert.SerializeObject(HandoffResponse);
            File.WriteAllText(@"Files\Output\HandoffOutput.txt", ho_json);
            var HandbackResponse = handback.RunHandback(auth.HttpClient).Result;
            var hb_json = (JObject) JsonConvert.DeserializeObject(HandbackResponse);
            File.WriteAllBytes(@"Files\Output\HandbackOutput.txt",Convert.FromBase64String(hb_json["Content"].ToString()));
        }
    }
}
