using SustitucionMOAModel.Dto.OrdenesCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IReporteESService
    {
        Task BuildReportES(List<ReporteDto> report, string blobReference);
    }
}
