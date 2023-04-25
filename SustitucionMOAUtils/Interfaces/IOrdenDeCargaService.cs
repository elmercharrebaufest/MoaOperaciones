using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
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
        List<string> ObtenerPedidos(int ordenId);
        string SeleccionarPedido(int ordenId, string pedido, string mailUsuario);
        Resultado VerificarSituacionCrediticia(int ordenId);
        string VerificarTransporte(int ordenId);
        void VerificarTransporteBulk();
        void CrearOrdenEnSAPBulk();
        List<OrdenDeCarga> VerificarVencimientoOrdenDeCarga();
        string ForzarCreacionOrden(int ordenId, string mailUsuario);
        OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga orden, string mailUsuario);
        VisualizarClienteResponse VisualizarCliente(VisualizarClienteRequest request);
        VisualizarProductoResponse VisualizarProducto(VisualizarProductoRequest request);
        ValidarCorredorClienteContratoProductoResponse ValidarCorredorClienteContratoProducto(ValidarCorredorClienteContratoProductoRequest request);
        string NotificarVariosPedidos(int ordenDeCargaId);
        string NotificarVariosContratos(EmailSenderData emailSenderData);
        string NotificarVencimientoOrdenCarga(int ordenId, string mailUsuario);
        string ActivarOC(int ordenId, string mailUsuario);
        OrdenDeCargaDetalleDto ObtenerPorNroEntrega(string mailUsuario, string nroEntrega);
        List<OrdenDeCargaCambiosHistorialDto> ObtenerCambiosHistorial(OrdenDeCarga orden);
        void VerificarSituacionCrediticiaJob();
        ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req);
        string EnviarOrdenesASAP(List<int> ordenesIds, string mailUsuario);
        ValidarSisaCorredorClienteResponse ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo);
        bool ValidarSisaCuit(string cuit);
        bool EmailGestionarAlta(string cuit, string razonSocial);
        ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit);
        List<PlantaDto> ObtenerPlantasDestino(string destinoCuit);
        List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit);
    }
}
