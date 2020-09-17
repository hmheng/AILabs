using FormRecognizerApi.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Services
{
    public interface IFormRecognizerService
    {
        Task<RecognizerTrainResponseViewModel> TrainAsync(string blobSAS);

        Task<RecognizerAnalyzeResponseViewModel> AnalyzeAsync(string modelId, IFormFile file);
    }
}
