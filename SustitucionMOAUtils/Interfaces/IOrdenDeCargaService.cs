using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaService
    {
        Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        CrearOrdenEnSAPResponse CrearOrdenEnSAP(CrearOrdenEnSAPRequest request, bool puedeEnviarASAP = false);
        List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId);
        OrdenDeCargaEditarDto ObtenerEditar(string mailUsuario, int ordenId);
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
        OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga orden, string mailUsuario);
        VisualizarClienteResponse VisualizarCliente(VisualizarClienteRequest request);

        ProveedorDto ObtenerProveedor(int idProveedor);

        VisualizarProductoResponse VisualizarProducto(VisualizarProductoRequest request);
        ValidarCorredorClienteContratoProductoResponse ValidarCorredorClienteContratoProducto(ValidarCorredorClienteContratoProductoRequest request);
        string NotificarVencimientoOrdenCarga(int ordenId, string mailUsuario);
        string ActivarOC(int ordenId, string mailUsuario);
        OrdenDeCargaDetalleDto ObtenerPorNroEntrega(string mailUsuario, string nroEntrega);
        List<OrdenDeCargaCambiosHistorialDto> ObtenerCambiosHistorial(OrdenDeCarga orden);
        void VerificarSituacionCrediticiaJob();
        ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req, string mailUsuario);
        string EnviarOrdenesASAP(List<int> ordenesIds, string mailUsuario);
        ValidarSisaCorredorClienteResponse ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo);
        bool ValidarSisaCuit(string cuit, string campo);
        bool EmailGestionarAltaCuitCliente(List<GestionCuitDto> cuits);
        bool EmailGestionarAltaIntermediarioFlete(GestionCuitDto cuit);
        bool ValidarCuitRuca(string cuit);
        ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit);
        List<PlantaDto> ObtenerPlantasDestino(string destinoCuit);
        List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit);
        ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit);
        (bool, Chofer) ValidarCuilChofer(string cuilChofer);
        bool ValidarCuilChoferDigito(string cuilChofer);
        bool ValidarCuitTransporteDigito(string cuitTransporte);
        Resultado SeleccionarFactura(int ordenId, string numeroFacturaSeleccionada, string mailUsuario);
        void VerificarCompensacion(int ordenId);
        List<AutoCompleteDropdownElement> ObtenerCuilsChofer(OrdenDeCarga ordenDeCarga, string mailUsuario);
        List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(OrdenDeCarga ordenDeCarga, string mailUsuario);
        bool ValidarOrdenActivaScato(string nroEntrega);
        Resultado VerificarCuitsTerceros(int ordenId, string usuarioEmail);
        List<ClienteSAPResponse> GetClientesVigentesSAP(string fechaIni, string fechaFin);
        List<SustitucionMOAModel.Entities.Proveedor> FiltrarNoExistentesWeb(List<ClienteSAPResponse> clientes);
    }
}
