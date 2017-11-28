using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ICMSConnector.Interfaces
{
    interface IIcmsClient
    {
        Task<string> UploadFile(HttpClient httpClient);
        Task<string> GetLocFilesWithContent(HttpClient httpClient, bool locAction, int fileId);
        Task<string> Ping(HttpClient httpClient);
        Task<string> GetFileWithStatus(HttpClient httpClient, string status);
        Task<string> GetLocFilesWithStatus(HttpClient httpClient, string status);
        Task<string> SetLocFileStatus(HttpClient httpClient, string status, IList<long?> fileIds, bool propagateToChildren);
    }
}
