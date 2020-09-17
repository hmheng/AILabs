using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Services
{
    public class BlobService : IBlobService
    {
        private IConfiguration _configuration { get; }
        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(Stream stream,string fileName, string containerReference = "files")
        {
            //var name = file.FileName;
            var name = Path.GetFileNameWithoutExtension(fileName).Replace(' ','_') + Path.GetExtension(fileName);

            if (stream != null && !string.IsNullOrWhiteSpace(name))
            {
               return await UploadFileAsBlobAsync(stream, name, containerReference);
            }


            return ""; //null just to make error free
        }


        /// <summary>
        /// Upload file in azure storage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private async Task<string> UploadFileAsBlobAsync(Stream stream, string filename, string containerReference = "receipts")
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("BlobConnection"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerReference);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

            await blockBlob.UploadFromStreamAsync(stream);

            stream.Dispose();
            return blockBlob?.Uri.ToString();
        }
    }
}
