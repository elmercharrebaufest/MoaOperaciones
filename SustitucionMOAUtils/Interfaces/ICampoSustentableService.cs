using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICampoSustentableService
    {
        Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz);
        List<CampoProveedorListadoDto> Listar(string mailUsuario);
        List<Cosecha> ObtenerCosechas();
    }
}
