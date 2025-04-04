using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class FrontLoggerRequestDto
    {
        public string Message { get; set; }
        public List<object> Additional { get; set; }
        public int Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string User { get; set; }

        public override string ToString()
        {
            return $"{Message} - {(Additional?.Any() ?? false ? (Additional[0] ?? "").ToString() : "")} - {User}";
        }
    }
}
