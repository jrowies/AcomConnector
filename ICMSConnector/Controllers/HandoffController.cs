using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ICMSConnector.Interfaces;
using Newtonsoft.Json;
using TranslationService.Models;

namespace ICMSConnector.Controllers
{
    class HandoffController: IHandoffManager
    {
        public Task<string> Handoff(HttpClient httpClient)
        {
            string path = (@"Files\Source\test.txt");
            var bytes = File.ReadAllBytes(path);
            var icmsFileIdentifier = new IcmsFileIdentifier();
            icmsFileIdentifier.AssetId = "AcomTestConnectorID";
            icmsFileIdentifier.HostFileReference = @"\\Acom\Files\Test\test.txt";
            icmsFileIdentifier.Locale = "en-us";
            icmsFileIdentifier.HostFileRevision = 4;
            icmsFileIdentifier.ContentType = "Article";

            var icmsMetadata = new IcmsFileMetadata();
            icmsMetadata.FileIdentifier = icmsFileIdentifier;
            icmsMetadata.Localizable = true;

            var icmsFile = new IcmsFile();
            icmsFile.Content = bytes;
            icmsFile.Metadata = icmsMetadata;

            var data = JsonConvert.SerializeObject(icmsFile);
            var content = new StringContent(data,Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = httpClient.PostAsync("api/Files/", content);           
            var stringResponse = response.Result.Content.ReadAsStringAsync();
            return stringResponse;
        }
    }
}
