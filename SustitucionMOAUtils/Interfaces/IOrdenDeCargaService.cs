using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaService
    {
        Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId);
        OrdenDeCargaEditarDto ObtenerEditar(string mailUsuario, int ordenId);
        List<OrdenDeCargaHistorialDto> ObtenerEditarHistorial(string mailUsuario, int ordenId);        
        string AnularOrden(int ordenId);
        string SolicitarAnulacionOrden(int ordenId, string mailUsuario);
        string RechazarSolicitudAnulacion(int ordenId, string mailUsuario);
        string EdicionFinalizada(int ordenId);
        string SolicitarEdicionOrden(int ordenId, string mailUsuario);
        string RechazarSolicitudEdicion(int ordenId, string mailUsuario);
        string NotificarTransporte(int ordenId);
        List<string> ObtenerContratos(int ordenId);
        Resultado SeleccionarContrato(int ordenId, string contratoSAP);
        List<string> ObtenerPedidos(int ordenId);
        string SeleccionarPedido(int ordenId, string pedido);
        Resultado VerificarSituacionCrediticia(int ordenId);
        string VerificarTransporte(int ordenId);
        void VerificarTransporteBulk();
        Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario);
        List<OrdenDeCarga> VerificarVencimientoOrdenDeCarga();
        string ForzarCreacionOrden(int ordenId);
        OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga orden, string mailUsuario);
        List<ProveedorDto> VisualizarCliente(string cliente, string contrato, string corredor, string fechaInicio, string fechaFin, string material, string pendiente, string tipoContrato);
        string NotificarVariosPedidos(int ordenDeCargaId);
        string NotificarVariosContratos(int ordenDeCargaId);
        string NotificarVencimientoOrdenCarga(int ordenId);

    }

}
