using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.ViewModels
{
    public partial class RecognizerAnalyzeResponseViewModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("pages")]
        public Page[] Pages { get; set; }

        [JsonProperty("errors")]
        public Error[] Errors { get; set; }
    }

    public partial class Error
    {
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("number")]
        public long? Number { get; set; }

        [JsonProperty("height")]
        public long? Height { get; set; }

        [JsonProperty("width")]
        public long? Width { get; set; }

        [JsonProperty("clusterId")]
        public long? ClusterId { get; set; }

        [JsonProperty("keyValuePairs")]
        public KeyValuePair[] KeyValuePairs { get; set; }

        [JsonProperty("tables")]
        public Table[] Tables { get; set; }
    }

    public partial class KeyValuePair
    {
        [JsonProperty("key")]
        public Key[] Key { get; set; }

        [JsonProperty("value")]
        public Key[] Value { get; set; }
    }

    public partial class Key
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("boundingBox")]
        public long[] BoundingBox { get; set; }

        [JsonProperty("confidence")]
        public long Confidence { get; set; }
    }

    public partial class Table
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("columns")]
        public Column[] Columns { get; set; }
    }

    public partial class Column
    {
        [JsonProperty("header")]
        public Key[] Header { get; set; }

        [JsonProperty("entries")]
        public Key[][] Entries { get; set; }
    }

}
