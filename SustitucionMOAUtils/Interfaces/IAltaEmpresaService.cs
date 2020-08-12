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
        List<ProveedorDto> getEmpresas(EstadoAprobacion estado);

        string setEstadoAprobacion(int empresaId, EstadoAprobacion estado,string observacion);

    }
}
