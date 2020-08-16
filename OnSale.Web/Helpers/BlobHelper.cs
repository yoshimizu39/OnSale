using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName) //pasamos el IFormFile y el nombre del contendroe creado en el blobstorage
        {
            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}"); //($"{name}"), para que no se repita las imègenes
            await blockBlob.UploadFromStreamAsync(stream); //nos sube la info al blob
            return name;
        }
    }

}
