using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using transcription_project.WebApp.Extensions;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using transcription_project.WebApp.Models;
using System.Text.RegularExpressions;

namespace transcription_project.WebApp.Services
{
    public class BlobServices : IBlobService
    {
        private BlobServiceClient _blobServiceClient;

        public BlobServices(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobAsync(string name, Container container)
        {
            //checking if Container name meets standard for accepted URL
            string pattern = "^((?!-)[A-Za-z0-9-]{1, 63}(?<!-)\\.)+[A-Za-z]{2, 6}$";
            string replacement = "";
            Regex rgx = new Regex(pattern);

            string result = rgx.Replace(container.containerUid.ToString(), replacement);
            result = result.ToLower();

            if (!_blobServiceClient.GetBlobContainerClient(result).Exists())
            {
                Log.Error("Container not found.");
                return Stream.Null;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(result);
            var blobClient = containerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();
            return blobDownloadInfo.Value.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public async Task UploadFileBlobAsync(Stream content, string fileName, Container container)
        {

            BlobContainerClient containerClient;
            //checking if Container name meets standard for accepted URL
            string pattern = "^((?!-)[A-Za-z0-9-]{1, 63}(?<!-)\\.)+[A-Za-z]{2, 6}$";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(container.containerUid.ToString(), replacement);
            result = result.ToLower();
            if (_blobServiceClient.GetBlobContainerClient(result).Exists())
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(result);
            }
            else
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(result);
            }
            var blobClient = containerClient.GetBlobClient(fileName);

            try
            {

                await blobClient.UploadAsync(content,
                new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = fileName.GetContentType() }
                );
            }

            catch (Exception e)
            {
                Log.Error($"Error during upload.");
            }
        }

    }
}