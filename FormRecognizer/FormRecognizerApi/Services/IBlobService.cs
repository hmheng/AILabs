using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName, string containerReference = "receipts");
    }
}
