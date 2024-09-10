using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasArchivosService
    {
        byte[] GenerarExcelHistorialCotizaciones(List<CotizacionHistorialDto> historialCotizaciones);
        byte[] GenerarExcelRevisionTecnica(PeticionDeOferta peticion);
    }
}
