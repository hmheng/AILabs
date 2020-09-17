using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.ViewModels
{
    public class Result
    {
        public string filename { get; set; }

        public RecognizerAnalyzeResponseViewModel body { get; set; }
        public List<Summary> summaries { get; set; }
        public bool status { get; set; }
    }

    public class Summary
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class DocumentAnalyzeResultViewModel
    {
        public List<Result> results { get; set; }
    }
}
