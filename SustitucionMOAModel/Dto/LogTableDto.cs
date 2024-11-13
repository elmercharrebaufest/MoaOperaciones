using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class LogTableDto
    {
        public LogTableDto()
        {
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

        public long Id { get; set; }
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
        public string LoggerLevel { get; set; }
        public int Count { get; set; }
        public IEnumerable<LogTableDto> Errors { get; set; }

    }

    public class LogRequest
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public List<string> Levels { get; set; } = new List<string>();
        public List<string> Loggers { get; set; } = new List<string>();

    }
}
