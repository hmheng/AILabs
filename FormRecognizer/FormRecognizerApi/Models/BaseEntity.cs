using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Models
{
    public class BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public string CreatedBy { get; set; } = "";

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public string UpdatedBy { get; set; } = "";

        public bool Status { get; set; } = true;
    }
}
