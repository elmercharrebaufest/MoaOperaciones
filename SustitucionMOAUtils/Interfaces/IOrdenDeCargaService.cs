using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaService : IOrdenDeCargaServiceBase
    {
        Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        CrearOrdenEnSAPResponse CrearOrdenEnSAP(CrearOrdenEnSAPRequest request, bool puedeEnviarASAP = false);
        List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin, int? idProveedorSeleccionado = null);
        OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId);
        OrdenDeCargaEditarDto ObtenerEditar(int ordenId);
        List<OrdenDeCargaHistorialDto> ObtenerEditarHistorial(string mailUsuario, int ordenId);
        string AnularOrden(int ordenId, string mailUsuario);
        string SolicitarAnulacionOrden(int ordenId, string mailUsuario);
        string RechazarSolicitudAnulacion(int ordenId, string mailUsuario);
        string EdicionFinalizada(int ordenId, string mailUsuario);
        string SolicitarEdicionOrden(int ordenId, string mailUsuario);
        string RechazarSolicitudEdicion(int ordenId, string mailUsuario);
        string NotificarTransporte(int ordenId);
        List<string> ObtenerContratos(int ordenId);
        Resultado SeleccionarContrato(int ordenId, string contratoSAP, string mailUsuario);
        Resultado VerificarSituacionCrediticia(int ordenId);
        string VerificarTransporte(int ordenId, string mailUsuario);
        void VerificarTransporteBulk();
        void CrearOrdenEnSAPBulk();
        List<OrdenDeCarga> VerificarVencimientoOrdenDeCarga();
        string ForzarCreacionOrden(int ordenId, string mailUsuario);
        OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga orden);
        VisualizarClienteResponse VisualizarCliente(VisualizarClienteRequest request);

        VisualizarProductoResponse VisualizarProducto(VisualizarProductoRequest request);
        ValidarCorredorClienteContratoProductoResponse ValidarCorredorClienteContratoProducto(ValidarCorredorClienteContratoProductoRequest request);
        string NotificarVencimientoOrdenCarga(int ordenId, string mailUsuario);
        string ActivarOC(int ordenId, string mailUsuario);
        OrdenDeCargaDetalleDto ObtenerPorNroEntrega(string mailUsuario, string nroEntrega);
        void VerificarSituacionCrediticiaJob();
        ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req, string mailUsuario);
        string EnviarOrdenesASAP(List<int> ordenesIds, string mailUsuario);
        ValidarSisaCorredorClienteResponse ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo);
        bool ValidarSisaCuit(string cuit, string campo);
        bool EnviarMailAltaCuitTerceros(bool gestionaFlete, bool gestionaDestino, bool gestionaDestinatario, string ordenId);
        bool ValidarCuitRuca(string cuit);
        ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit);
        ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit);
        (bool, Chofer) ValidarCuilChofer(string cuilChofer);
        Resultado SeleccionarFactura(int ordenId, string numeroFacturaSeleccionada);
        void VerificarCompensacion(int ordenId);
        List<AutoCompleteDropdownElement> ObtenerCuilsChofer(OrdenDeCarga ordenDeCarga);
        List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(OrdenDeCarga ordenDeCarga, string mailUsuario);
        bool ValidarOrdenActivaScato(string ordenId);
        Resultado VerificarCuitsTerceros(int ordenId);
        List<ClienteSAPResponse> GetClientesVigentesSAP(string fechaIni, string fechaFin);
        List<SustitucionMOAModel.Entities.Proveedor> FiltrarNoExistentesWeb(List<ClienteSAPResponse> clientes);
        List<DestinatarioDto> ObtenerDestinatariosConsultaFas(int ordenId);
        void VerificarOrdenesFacturaCompensadaJob();
        ValidarChoferResponse ValidarChofer(string cuilChofer, string cuitCliente);
    }
}
