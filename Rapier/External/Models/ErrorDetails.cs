using System.Collections.Generic;
using System.Text.Json;

namespace Rapier.External.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Details { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
