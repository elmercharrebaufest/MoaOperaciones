using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaService
    {
        List<ProveedorDto> getEmpresas();

        string setEstadoAprobacion(int proveedorId, EstadoAprobacion estado,string observacion, string usuarioMail, string observacionParaElProveedor);

        EstadoAprobacionDto GetEstadoAprobacion(string mail);
    }
}
