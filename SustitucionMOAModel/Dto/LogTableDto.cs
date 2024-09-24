using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class LogTableDto
    {
        public LogTableDto() { 
        }

        public LogTableDto(LogTable l)
        {
            this.Id = l.Id;
            this.Date = l.Date;
            this.Level = l.Level;
            this.Logger = l.Logger;
            this.Message = l.Message;
            this.Exception = l.Exception;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

    public class LogTableCountErrors
    {
        public string Logger { get; set; }
        public string Level { get; set; }
        public int Count { get; set; }
        public List<LogTableDto> Errors { get; set; } = new List<LogTableDto>();
    }
}
