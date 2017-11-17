using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            IcmsClient icmsClient = new IcmsClient();
            HttpClient httpClient = auth.HttpClient;
            try
            {
                var responseText = icmsClient.Ping(httpClient).Result;
                File.WriteAllText(@"Files\Output\PingOutput.txt", responseText);

                var uploadResponse = icmsClient.UploadFile(httpClient).Result;
                var uploadResult = JsonConvert.SerializeObject(uploadResponse);

                File.WriteAllText(@"Files\Output\FileUploadOutput.txt", uploadResult);

                var handbackResponse = icmsClient.GetLocFilesWithContent(httpClient, true, 4001072).Result;
                var hbJson = (JObject) JsonConvert.DeserializeObject(handbackResponse);
                //File.WriteAllBytes(@"Files\Output\HandbackOutput.txt",Convert.FromBase64String(hb_json["Content"].ToString()));

                var fileIds = new List<long?> {4001072};
                var resultOfUpdate = icmsClient.SetLocFileStatus(httpClient, "CheckedIn", fileIds);

                var locFile = icmsClient.GetLocFilesWithContent(httpClient,true,4001072);
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format(@"Exception: {0} has occurred",exception.Message));
            }                      
        }
    }
}
