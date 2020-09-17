using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FormRecognizerApi.ViewModels
{
    public partial class RecognizerTrainResponseViewModel
    {
        [JsonProperty("modelId")]
        public string ModelId { get; set; }

        [JsonProperty("trainingDocuments")]
        public TrainingDocument[] TrainingDocuments { get; set; }

        [JsonProperty("errors")]
        public object[] Errors { get; set; }
    }

    public partial class TrainingDocument
    {
        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("pages")]
        public long Pages { get; set; }

        [JsonProperty("errors")]
        public object[] Errors { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
