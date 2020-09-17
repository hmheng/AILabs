using FormRecognizerApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FormRecognizerApi.Services
{
    public class FormRecognizerService : IFormRecognizerService
    {
        private IConfiguration _configuration { get; }
        public FormRecognizerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<RecognizerTrainResponseViewModel> TrainAsync(string blobSAS)
        {

            var trainUrl = $"{_configuration["FormRecognizer:Endpoint"]}/formrecognizer/v1.0-preview/custom/train";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_configuration["FormRecognizer:Key"]}");

            RecognizerTrainRequestViewModel trainRequestViewModel = new RecognizerTrainRequestViewModel()
            {
                Source = blobSAS
            };
            var response = await client.PostAsync(trainUrl, new StringContent(JsonConvert.SerializeObject(trainRequestViewModel), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var trainResponse = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(trainResponse))
                {
                    return JsonConvert.DeserializeObject<RecognizerTrainResponseViewModel>(trainResponse);
                }
            }
            return null;
        }

        public async Task<RecognizerAnalyzeResponseViewModel> AnalyzeAsync(string modelId, IFormFile file)
        {
            var analyzeUrl = $"{_configuration["FormRecognizer:Endpoint"]}/formrecognizer/v1.0-preview/custom/models/{modelId}/analyze";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_configuration["FormRecognizer:Key"]}");

            //// Request body
            //byte[] byteData = file;

            //using (var content = new ByteArrayContent(file))
            //{
            //    //content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            //    MultipartFormDataContent multiContent = new MultipartFormDataContent();

            //    multiContent.Add(content, "file", fileName);
            //    var response = await client.PostAsync(analyzeUrl, multiContent);
            //    var result = await response.Content.ReadAsStringAsync();
            //}


            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(file.OpenReadStream())
                {
                    Headers =
                    {
                        ContentLength = file.Length,
                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                    }
                }, "File", file.FileName);

                var response = await client.PostAsync(analyzeUrl, content);
                var result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return JsonConvert.DeserializeObject<RecognizerAnalyzeResponseViewModel>(result);
                }
            }
            //var analyzeUrl = $"{_configuration["FormRecognizer:Endpoint"]}/formrecognizer/v1.0-preview/custom/models/{modelId}/analyze";

            //RecognizerTrainRequestViewModel trainRequestViewModel = new RecognizerTrainRequestViewModel()
            //{
            //    Source = blobSAS
            //};

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_configuration["FormRecognizer:Key"]}");
            //var response = await client.PostAsync(trainUrl, new StringContent(JsonConvert.SerializeObject(trainRequestViewModel), Encoding.UTF8, "application/json"));

            //if (response.IsSuccessStatusCode)
            //{
            //    var trainResponse = await response.Content.ReadAsAsync<RecognizerTrainResponseViewModel>();

            //    return trainResponse;
            //}
            return null;
        }
    }
}
