
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FormRecognizerApi.ViewModels
{
    /// <summary>
    /// https://app.quicktype.io/?l=csharp
    /// </summary>
    public partial class ReceiptResultViewModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("recognitionResults")]
        public RecognitionResult[] RecognitionResults { get; set; }

        [JsonProperty("understandingResults")]
        public UnderstandingResult[] UnderstandingResults { get; set; }
    }

    public partial class RecognitionResult
    {
        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("clockwiseOrientation")]
        public double ClockwiseOrientation { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("lines")]
        public Line[] Lines { get; set; }
    }

    public partial class Line
    {
        [JsonProperty("boundingBox")]
        public long[] BoundingBox { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("words")]
        public Word[] Words { get; set; }
    }

    public partial class Word
    {
        [JsonProperty("boundingBox")]
        public long[] BoundingBox { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public partial class UnderstandingResult
    {
        [JsonProperty("pages")]
        public long[] Pages { get; set; }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }
    }

    public partial class Fields
    {
        [JsonProperty("Subtotal")]
        public MerchantAddress Subtotal { get; set; }

        [JsonProperty("Total")]
        public MerchantAddress Total { get; set; }

        [JsonProperty("Tax")]
        public MerchantAddress Tax { get; set; }

        [JsonProperty("MerchantAddress")]
        public MerchantAddress MerchantAddress { get; set; }

        [JsonProperty("MerchantName")]
        public MerchantAddress MerchantName { get; set; }

        [JsonProperty("MerchantPhoneNumber")]
        public MerchantAddress MerchantPhoneNumber { get; set; }

        [JsonProperty("TransactionDate")]
        public MerchantAddress TransactionDate { get; set; }

        [JsonProperty("TransactionTime")]
        public MerchantAddress TransactionTime { get; set; }
    }

    public partial class MerchantAddress
    {
        [JsonProperty("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("elements")]
        public Element[] Elements { get; set; }
    }

    public partial class Element
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }

}
