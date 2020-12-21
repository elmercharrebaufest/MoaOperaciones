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
        string GrabarNuevoProveedorNoGranos(string razonSocial, string cuit, string email, string telefono, bool realizarAnalisisNOSIS, int IdRubro, string CondicionDePago
            , string ServicioPrestado, string OrganizacionDeCompra, string RazonDeEleccion, int FacturacionAnual, string SolicitanteInterno, string usuarioMail, 
            int? idProveedor, string observacionesParaElProveedor,bool requiereVerificacionCompras, bool ingresoAPlanta, bool altaInterna);
        List<RubroDto> GetRubros();
        string RechazarProveedorNoGranos(int idProveedor, string usuarioMail, string observacionesParaElProveedor);

    }
}
