using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAltaEmpresaNoGranosService
    {
        Resultado GrabarNuevoProveedorNoGranos(string razonSocial, string cuit, string email, string telefono, bool realizarAnalisisNOSIS, int IdRubro, string CondicionDePago
            , string ServicioPrestado, string OrganizacionDeCompra, string RazonDeEleccion, int FacturacionAnual, string SolicitanteInterno, string usuarioMail, 
            int? idProveedor, string observacionesParaElProveedor,bool requiereVerificacionCompras, bool ingresoAPlanta, bool altaInterna, bool siperObligatorio, string observacionInterna);
        List<RubroDto> GetRubros();
        string RechazarProveedorNoGranos(int idProveedor, string usuarioMail, string observacionesParaElProveedor);

        InfoProveedorNoGranosDto ObtenerInfoProveedorNoGranos(string mailUsuario, int proveedorId);

        string EditarAltaEmpresaNoGranos(int proveedorId,
                                         string razonSocial,
                                         string cuit,
                                         string email,
                                         string telefono,
                                         bool realizarAnalisisNOSIS,
                                         int? IdRubro,
                                         string CondicionDePago,
                                         string ServicioPrestado,
                                         string OrganizacionDeCompra,
                                         string RazonDeEleccion,
                                         int? FacturacionAnual,
                                         bool requiereVerificacionCompras,
                                         bool ingresoAPlanta,
                                         bool altaInterna,
                                         bool siperObligatorio);

        string GetRazonSocial(string CUIT);
        byte[] DescargarFormularioNG(ProveedorAltaDto proveedorDto);

        string HabilitarProveedorOperando(int proveedorId, string razonSocial);

        bool RegistrarDocumentacionFisica(int proveedorId, bool contieneDocumentacionFisica, string mailUsuarioAlta);
    }
}
