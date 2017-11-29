using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ICMSConnector.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TranslationService.Models;

namespace ICMSConnector.Controllers
{
    class IcmsClient: IIcmsClient
    {
        /// <summary>
        /// Method to upload file to iCMS 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public async Task<string> UploadFile(HttpClient httpClient)
        {
            string path = (@"Files\Source\test.txt"); //Path of the file to be picked up, folder in bin/debug
            var bytes = File.ReadAllBytes(path);
            var icmsFileIdentifier = new IcmsFileIdentifier //ICMS supplied model to store File ID data
            {
                AssetId = "SourceAsset", //AssetID to identify file in Vantage Point (ICMS UI)
                HostFileReference = @"\\Acom\Files\Source\test.txt", //Host File Reference + Host File Revision + Host Content Store is the unqiue key to identify a file in ICMS
                Locale = "en-us", //Source file locale
                HostFileRevision = 0, //ICMS supports storing different versions of files, should be mapped to the version of file in source location
                ContentType = "Article" //The type of file, can be all ICMS supported content types                                       
            };

            var icmsMetadata = new IcmsFileMetadataWithLocalizationInfo //Model for the metdata about the file
            {
                FileIdentifier = icmsFileIdentifier, //Set File ID data 
                Localizable = true, 
                ContentGroup = "GHM", //High level file grouping in ICMS
                Priority = "2",//Localization priority
                HostFileTags = "Acom; Test; Connector", 
                ReadyForLoc = true,
                IsLead = true,
                LocaleList = new List<string>{ "ko-KR" },
                ParentFileIdentifier = null,
                TranslationOptions = new TranslationOptions
                {
                    RecycleContent = true,
                    TranslationType = "MT",
                    RecycleContentAudience = "Production Pilot"
                },
                Partner = "C+E Content - Continuous Production"
            };

            var icmsFile = new IcmsFile //Model for the file to be uploaded to iCMS
            {
                Content = bytes,
                Metadata = icmsMetadata //Set metadata                
            };

            //List of files to be uploaded
            var icmsList = new List<IcmsFile> {icmsFile};

            var data = JsonConvert.SerializeObject(icmsList); //Serializing file into a JSON string
            var content = new StringContent(data, Encoding.UTF8); //Create formatted string data appropriate for http server/client communication
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); //Specifiy content type

            var response = await httpClient.PostAsync("api/FilesWithLocInfo/", content); //Call Files API 
            var stringResponse = response.Content.ReadAsStringAsync(); //Serialize the response content
            return stringResponse.Result; //Get string from awaitable Task
        }

        /// <summary>
        /// Ping the Translation Service API
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        public async Task<string> Ping(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("api/Ping"); //Call the ping API to check if the translation service is up
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }

        /// <summary>
        /// Get files with an input status
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="status">Only one status supported: ReIndex</param>
        /// <returns></returns>
        public async Task<string> GetFileWithStatus(HttpClient httpClient, string status)
        {
            var response = await httpClient.GetAsync(string.Format(@"api/Files/Filestatus?status={0}&count={1}", status,100)); //Get Files with status and specify count retrieved
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }

        /// <summary>
        /// Get Loc Files with an input status and a limit
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="status">input status</param>
        /// <returns></returns>
        public async Task<string> GetLocFilesWithStatus(HttpClient httpClient, string status)
        {
            //Get LocFiles with status and specify count retrieved
            var response = await httpClient.GetAsync(string.Format(@"api/LocFiles/Locstatus?status={0}&count={1}", status, 100));
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }


        /// <summary>
        /// Setting status for LocFiles based on a list of loc fileIds
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="status">Status to be updated (CheckedIn, HandedBack etc.)</param>
        /// <param name="fileIds">A max of 4000 id's can be updated in a single call</param>
        /// <param name="propagateToChildren">Whether loc action should be propagated to dependents</param>
        /// <returns></returns>
        public async Task<string> SetLocFileStatus(HttpClient httpClient,string status, IList<long?> fileIds, bool propagateToChildren)
        {
            var locFileStatus = new LocFilesStatus //Create a Loc Status object with all the fileIds that have to be updated, the status and message
            {
                LocFileIds = fileIds.ToArray(),
                LocStatus = status,
                Message = "Setting loc file status from a Test client" //Setting message
            };

            var content = new StringContent(JsonConvert.SerializeObject(locFileStatus));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.PutAsync($"api/LocFiles/LocStatus?propagateToChildren={propagateToChildren}", content); //Call LocStatus API
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }

        public async Task<string> GetFailedValidationFiles(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(@"api/FilesFailedValidation");
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }

        /// <summary>
        /// Get LocFile which has gone through the pipe from Handoff to Handback.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="locAction"> Aggregated locAction (loc action resolved on loc file then file then project then repo level)</param>
        /// <param name="fileId">Id of the loc file to be retrieved</param>
        /// <returns></returns>
        public Task<string> GetLocFilesWithContent(HttpClient httpClient, bool locAction, int fileId)
        {
            var response = httpClient.GetAsync($"api/LocFiles/{fileId}?getLocAction={locAction}");
            if (!response.Result.IsSuccessStatusCode)
            {
                return Task.FromResult(response.Result.ReasonPhrase);
            }
            var responseString = response.Result.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
