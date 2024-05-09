using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenResiduosService
    {
        List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientes();
        MaterialDto[] ObtenerMateriales();
        ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicio, string fechaFin);
        LocalidadDto[] ObtenerLocalidades();
        PatentesClienteDto ObtenerPatentes(int clienteId);
        List<SustitucionMOAModel.Dto.OrdenDeCarga.PlantaDto> ObtenerPlantas(string cuit);
        List<SustitucionMOAModel.Dto.OrdenDeCarga.DomicilioDto> ObtenerDomicilios(string cuit);
        TransportesIds ObtenerIdsTransportes(int clienteId, string patenteAcoplado);
        bool EsCuilCuitValido(string cuilCuit);
        GrabarOrdenResponse CrearNuevaOrden(OrdenResiduosDto ordenDto, string mailUsuario);
        OrdenResiduosDto ObtenerOrden(int idOrden);
        OrdenResiduosDto AnularOrden(int ordenId, string mailUsuario);
        OrdenResiduosDto ActualizarSolicitudAnulacion(int ordenId, string mailUsuario, bool aprobarSolicitud);
        OrdenResiduosDto SolicitarAnulacion(int ordenId, string mailUsuario);
        OrdenResiduosDto ActualizarSolicitudEdicion(int ordenId, string mailUsuario, bool aprobarSolicitud);
        GrabarOrdenResponse EditarOrden(OrdenResiduosDto ordenDto, string mailUsuario);
        OrdenResiduosDto VerificarTransporte(int ordenId);
        SustitucionMOAModel.Dto.ProveedorDto ObtenerProveedor(int idProveedor);
        void VerificarVencimientoOrdenesResiduos();
    }
}
