using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ILogTableService
    {
        List<LogTableCountErrors> ObtenerLogs(DateTime? desde, bool soloErrores);
        List<LogTableCountErrors> ObtenerLogs(LogRequest request);
    }
}
