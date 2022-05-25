using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
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
        string AnularOrden(int ordenId);
        string SolicitarAnulacionOrden(int ordenId);
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
        string NotificarVariosPedidos(int ordenDeCargaId);
        string NotificarVariosContratos(int ordenDeCargaId);

    }

}
