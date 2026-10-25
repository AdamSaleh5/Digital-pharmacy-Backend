using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ImportResponse
    {
        public ImportResponse() { }

        public ImportResponse(string logFileName)
        {
            this.Status = false;
            this.LogFileName = logFileName;
        }

        public bool Status { get; set; } = true;

        public string LogFileName { get; set; } = string.Empty;

        public int ImportedCount { get; set; }

        public List<ImportError> Errors { get; set; } = new();
    }

    public class ImportError
    {
        public int? Row { get; set; }
        public string? Column { get; set; }
        public string? Value { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
