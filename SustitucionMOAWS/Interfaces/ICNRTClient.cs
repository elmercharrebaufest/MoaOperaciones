using SustitucionMOAModel.Models.WebApiMap.CNRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface ICNRTClient
    {
        EquiposResponse ObtenerEquipos(string patenteChasis, string patenteAcoplado);
    }
}
