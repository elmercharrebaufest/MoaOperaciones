using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFeriadoService
    {
        List<DateTime> ObtenerFeriados();
    }
}
