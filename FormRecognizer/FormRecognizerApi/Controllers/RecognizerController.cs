using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer.Training;
using FormRecognizerApi.Constants;
using FormRecognizerApi.Services;
using FormRecognizerApi.SignalR;
using FormRecognizerApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FormRecognizerApi.Controllers
{
    [ApiController]
    public class RecognizerController : ControllerBase
    {
        private IHubContext<PushHub> _hubContext;
        private IConfiguration _configuration { get; }
        private IBlobService _blobService { get; set; }
        private IFormRecognizerService _formRecognizerService { get; set; }

        public RecognizerController(IConfiguration configuration,
            IBlobService blobService,
            IHubContext<PushHub> hubContext,
            IFormRecognizerService formRecognizerService)
        {
            _configuration = configuration;
            _blobService = blobService;
            _formRecognizerService = formRecognizerService;
            _hubContext = hubContext;
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> TrainDocument(IFormFile file)
        {
            if (file.Length > 0)
            {
                //Upload to blob
                using (var stream = file.OpenReadStream())
                {
                    var fileUrl = await _blobService.UploadFileAsync(stream, file.FileName, "training");

                    if (!string.IsNullOrWhiteSpace(fileUrl))
                    {

                        return new OkObjectResult(fileUrl);
                    }
                }

            }
            return new BadRequestObjectResult("");

        }


        [HttpPost("train")]
        public async Task<IActionResult> TrainDocument()
        {
            var trainResult = await _formRecognizerService.TrainAsync($"{_configuration["SAS:training"]}");

            if (trainResult != null)
            {

                return new OkObjectResult(trainResult);
            }
            return new BadRequestObjectResult("");

        }


        /// <summary>
        /// Analyze Document
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /upload-document-and-analyze
        ///     {
        ///        files: multiple files of multipart/form-data
        ///     }
        ///
        /// </remarks>
        /// <returns>A JSON object stating whether the chat is successfully sent or not.</returns>
        [HttpPost]
        [Route("analyze")]
        [RequestFormSizeLimit(104857600)] // 100 MB
        public async Task<IActionResult> AnalyzeDocument(IFormFile file, string modelId = "a6936931-e1f4-49d2-bc4d-74020a2ad3f3")
        {
            DocumentAnalyzeResultViewModel resultViewModel = new DocumentAnalyzeResultViewModel();
            resultViewModel.results = new List<Result>();

            //Upload to blob
            using (var stream = file.OpenReadStream())
            {
                var fileUrl = await _blobService.UploadFileAsync(stream, file.FileName, "test");
                if (!string.IsNullOrWhiteSpace(fileUrl))
                {
                    var tmpStream = file.OpenReadStream();
                    byte[] bytes = new byte[tmpStream.Length];
                    tmpStream.Read(bytes, 0, (int)tmpStream.Length);
                    var analyzeResult = await _formRecognizerService.AnalyzeAsync(modelId, file);

                    if (analyzeResult != null)
                    {
                        resultViewModel.results.Add(new Result { filename = file.FileName, body = analyzeResult, summaries = await GetSummaryAsync(analyzeResult), status = true });
                    }
                }

            }

            if (resultViewModel.results.Count > 0)
            {
                return new OkObjectResult(resultViewModel);
            }
            return new BadRequestObjectResult("");

        }

        [Obsolete("version 1 preview api. Please refer to v2")]
        [HttpPost("upload-simple-receipt-and-analyze")]
        public async Task<IActionResult> AnalyzeSimpleReceipt(IFormFile file)
        {
            var analyzeUrl = $"{_configuration["FormRecognizer:Endpoint"]}/formrecognizer/v1.0-preview/prebuilt/receipt/asyncBatchAnalyze";

            //Upload to blob
            using (var stream = file.OpenReadStream())
            {
                var fileUrl = await _blobService.UploadFileAsync(stream, file.FileName, "simple-receipts");
                if (!string.IsNullOrWhiteSpace(fileUrl))
                {
                    ReceiptRequestViewModel receiptRequestViewModel = new ReceiptRequestViewModel()
                    {
                        Url = $"{fileUrl}"
                    };

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_configuration["FormRecognizer:Key"]}");
                    var response = await client.PostAsync(analyzeUrl, new StringContent(JsonConvert.SerializeObject(receiptRequestViewModel), Encoding.UTF8, "application/json"));
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        var a = response.Headers.GetValues("Operation-Location")?.FirstOrDefault();
                        var operationId = a.Substring(a.Length - 36);

                        var result = await this.GetResultAsync(operationId, fileUrl);
                        return new OkObjectResult(new { operationId, fileUrl });
                    }

                }
            }
            return new BadRequestObjectResult("");

        }


        [HttpGet("get-result")]
        public async Task<IActionResult> GetResultAsync(string operationId, string fileUrl)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{_configuration["FormRecognizer:Key"]}");
            var getResultUrl = $"{_configuration["FormRecognizer:Endpoint"]}/formrecognizer/v1.0-preview/prebuilt/receipt/operations/{operationId}";

            bool isRunning = false;
            do
            {
                var resultResponse = await client.GetAsync(getResultUrl);
                if (resultResponse.IsSuccessStatusCode)
                {
                    var resultString = await resultResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(resultString))
                    {
                        var frResult = JsonConvert.DeserializeObject<ReceiptResultViewModel>(resultString);
                        if (frResult.Status.ToUpper() != "RUNNING")
                        {
                            isRunning = false;
                            if (frResult.UnderstandingResults.Count() > 0)
                            {
                                SimpleReceiptViewModel vm = new SimpleReceiptViewModel()
                                {
                                    MerchantName = frResult.UnderstandingResults[0].Fields.MerchantName?.Text,
                                    MerchantAddress = frResult.UnderstandingResults[0].Fields.MerchantAddress?.Text,
                                    TotalAmount = frResult.UnderstandingResults[0].Fields.Total?.Text,
                                    SubTotalAmount = frResult.UnderstandingResults[0].Fields.Subtotal?.Text,
                                    BlobUrl = fileUrl
                                };

                                await _hubContext.Clients.All.SendAsync("test", "api", vm);
                                return new OkObjectResult(frResult);
                            }
                        }
                        else
                        {
                            isRunning = true;
                        }
                    }
                }
            } while (isRunning);
            return new BadRequestObjectResult("");

        }


        private async Task<List<Summary>> GetSummaryAsync(RecognizerAnalyzeResponseViewModel analyzeResult)
        {
            List<Summary> summaries = new List<Summary>();
            if (analyzeResult != null)
            {
                var pages = analyzeResult.Pages;
                if (pages != null && pages.Length > 0)
                {
                    foreach (var page in pages)
                    {
                        foreach (var pair in page.KeyValuePairs)
                        {
                            string key = pair.Key.Select(x => x.Text).Aggregate((i, j) => i + " " + j);
                            string value = pair.Value.Select(x => x.Text).Aggregate((i, j) => i + " " + j);
                            summaries.Add(new Summary()
                            {
                                key = key,
                                value = value
                            });
                        }
                    }
                }
            }
            return summaries;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAsync()
        {
            return new OkObjectResult(new[] { "value1", "value2" });
        }


        //[MapToApiVersion("2")]
        //[HttpGet("get-all")]
        //public async Task<IActionResult> GetV2Async()
        //{
        //    return new OkObjectResult(new[] { "value2", "value2" });
        //}
        #region V2.1 preview
        /// <summary>
        /// Analyze Layout
        /// Sample layout: https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/simple-invoice.png
        /// </summary>
        /// <returns></returns>
        [HttpGet("v2.1-pre/analyze-layout")]
        public async Task<IActionResult> AnalyzeLayout(string layoutUri)
        {
            try
            {
                FormRecognizerClient recognizerClient = AuthenticateClient();
                FormPageCollection formPages = await recognizerClient
                    .StartRecognizeContentFromUri(new Uri(layoutUri))
                    .WaitForCompletionAsync();

                if (formPages != null)
                {
                    return new OkObjectResult(formPages);
                }
                return new BadRequestObjectResult("No result");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
        }

        /// <summary>
        /// Analyze Receipt
        /// sample receipt: https://docs.microsoft.com/azure/cognitive-services/form-recognizer/media/contoso-allinone.jpg
        /// </summary>
        /// <param name="receiptUri"></param>
        /// <returns></returns>
        [HttpGet("v2.1-pre/analyze-receipt")]
        public async Task<IActionResult> AnalyzeReceiptV2(string receiptUri)
        {
            try
            {
                FormRecognizerClient recognizerClient = AuthenticateClient();
                RecognizedFormCollection receipts = await recognizerClient.StartRecognizeReceiptsFromUri(new Uri(receiptUri)).WaitForCompletionAsync();

                if (receipts != null)
                {
                    return new OkObjectResult(receipts);
                }
                return new BadRequestObjectResult("No result");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }

        }

        /// <summary>
        /// Analyze Business Card
        /// sample business card: https://raw.githubusercontent.com/Azure/azure-sdk-for-python/master/sdk/formrecognizer/azure-ai-formrecognizer/samples/sample_forms/business_cards/business-card-english.jpg
        /// </summary>
        /// <param name="receiptUri"></param>
        /// <returns></returns>
        [HttpGet("v2.1-pre/analyze-business-card")]
        public async Task<IActionResult> AnalyzeBusinessCardV2(string cardUri)
        {
            try
            {
                FormRecognizerClient recognizerClient = AuthenticateClient();
                RecognizedFormCollection businessCards = await recognizerClient.StartRecognizeBusinessCardsFromUriAsync(new Uri(cardUri))
                                                                .WaitForCompletionAsync();

                if (businessCards != null)
                {
                    return new OkObjectResult(businessCards);
                }
                return new BadRequestObjectResult("No result");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }

        }
        #endregion

        #region private
        private FormRecognizerClient AuthenticateClient()
        {
            var apiKey = $"{_configuration["FormRecognizer:Key"]}" ?? "";
            var endpoint = $"{_configuration["FormRecognizer:Endpoint"]}" ?? "";
            var credential = new AzureKeyCredential(apiKey);
            var client = new FormRecognizerClient(new Uri(endpoint), credential);
            return client;
        }

        private FormTrainingClient AuthenticateTrainingClient()
        {
            var apiKey = $"{_configuration["FormRecognizer:Key"]}" ?? "";
            var endpoint = $"{_configuration["FormRecognizer:Endpoint"]}" ?? "";
            var credential = new AzureKeyCredential(apiKey);
            var client = new FormTrainingClient(new Uri(endpoint), credential);
            return client;
        }
        #endregion
    }

}
