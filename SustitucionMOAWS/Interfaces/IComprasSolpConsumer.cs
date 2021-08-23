using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IObtenerCecoSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerCuentasSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerOrdenSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerServiciosSolpConsumerMOA
    {
        object request();
    }
}
