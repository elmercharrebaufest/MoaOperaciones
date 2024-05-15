using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
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
        List<ProveedorAltaDto> GetEmpresas(List<int> IdTiposProveedor, string fechaInicio, string fechaFin);

        string SetEstadoAprobacion(int proveedorId, EstadoAprobacion estado, string observacion, string usuarioMail, string observacionParaElProveedor, string estadoSIPER, bool enviarMail,
                                          string razonSocial, string codigoCliente);

        EstadoAprobacionDto GetEstadoAprobacion(string mail);

        string DeshabilitarUsuario(string usuarioMail, int proveedorID, string observacion, string observarcionProveedor);

        string HabilitarUsuario(string usuarioMail, int proveedorID, string observacion);

        string GuardarSIPER(int proveedorId, string estadoSIPER);

        string SolicitarInformacion(int proveedorId, string usuarioMail);

        string AgregarObservacion(int proveedorId, string observacion, string usuarioMail);

    }
}
