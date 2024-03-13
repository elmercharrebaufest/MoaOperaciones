using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string ToString()
        {
            return $"{Timestamp} -> Message={Message} - FileName={FileName} - LineNumber={LineNumber}";
        }
    }
}
