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
        List<ProveedorAltaDto> GetEmpresas(int IdTipoProveedor);

        string SetEstadoAprobacion(int proveedorId, EstadoAprobacion estado, string observacion, string usuarioMail, string observacionParaElProveedor, string estadoSIPER, bool enviarMail);

        EstadoAprobacionDto GetEstadoAprobacion(string mail);

        string DeshabilitarUsuario(string usuarioMail, int proveedorID, string observacion, string observarcionProveedor);

        string HabilitarUsuario(string usuarioMail, int proveedorID, string observacion);

        string GuardarSIPER(int proveedorId, string estadoSIPER);
    }
}
