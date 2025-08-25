using SustitucionMOAModel.Dto;
using ScatoWS = SustitucionMOAWS.ScatoWebService;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaFasonService : IOrdenDeCargaServiceBase
    {
        ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request);
        DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario);
        OrdenDeCargaFasonDto VerificarTransporte(int ordenId, string mailUsuario);
        void VerificarTransporteJob();
        List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason();
        List<ScatoWS.KmPorProveedorDto> ObtenerDestinos(int clienteId);
        Resultado Crear(CrearOrdenDeCargaFasonRequest request, string mailUsuario);
        Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario);
        List<ProveedorDto> GetCorredores();
        List<ProveedorDto> GetClientesDeCorredor(string codigoCorredor);
        OrdenDeCargaFasonDto AnularOrden(int ordenId, string mailUsuario);
        //List<AutoCompleteDropdownElement> ObtenerCuilsChofer(OrdenDeCargaFasonRequest ordenDeCarga, string mailUsuario);
        List<AutoCompleteDropdownElement> ObtenerCuilsChofer(int clienteId, string patenteAcoplado, string mailUsuario);
        List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(int clienteId, string patenteAcoplado, string mailUsuario);
        OrdenDeCargaDto ObtenerPatentes(OrdenDeCargaFasonRequest orden, string mailUsuario);
        bool EnviarMailAltaCuitTerceros(bool gestionaFlete, bool gestionaDestino, bool gestionaDestinatario, string ordenId);
        OrdenDeCargaFasonDto VerificarCuitsTerceros(int ordenId, string mailUsuario);
        bool ValidarOrdenActivaScato(long ordenId);
        bool ValidarSisaCliente(string codigoCliente, string codigoMaterial);
        bool ValidarExistenciaPatente(string patenteChasis, string cuitCliente);
    }
}
