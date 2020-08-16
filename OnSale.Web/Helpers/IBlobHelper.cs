using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName); //IFormFile, devuelve el archivo de memoria

        Task<Guid> UploadBlobAsync(byte[] file, string containerName); //byte[], al tomarse foto del fono

        Task<Guid> UploadBlobAsync(string image, string containerName);
    }

}
