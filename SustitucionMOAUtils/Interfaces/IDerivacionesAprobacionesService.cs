using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IDerivacionesAprobacionesService
    {
        void ReturnAprobaciones(string mailFiscal, string mailSuplente);

    }
}
