using SustitucionMOAModel.Entities;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ILocalidadService
    {
         List<LocalidadDto> SincronizarLocalidadesScato();  
         List<Localidad> GetLocalidades();
         List<ProvinciaDto> ObtenerProvinciasScato();
    }
}
