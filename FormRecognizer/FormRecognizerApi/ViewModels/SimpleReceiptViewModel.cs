using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.ViewModels
{
    public class SimpleReceiptViewModel
    {
        public string TransactionDate { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddress { get; set; }
        public string TotalAmount { get; set; }
        public string SubTotalAmount { get; set; }

        public string BlobUrl { get; set; }
    }
}
