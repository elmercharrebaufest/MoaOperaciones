using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IReportesService
    {
        void EnviarReporteLiquidacionesInformadas();
        void EnviarReporteCamposSustentablesTSA();
        void EnviarReporteConflictosCamposSustentables();
        void EnviarReporteLogin();
    }
}
