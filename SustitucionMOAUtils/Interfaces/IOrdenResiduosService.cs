using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Dto.Scato;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenResiduosService : IOrdenDeCargaServiceBase
    {
        List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientes();
        MaterialDto[] ObtenerMateriales();
        ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicio, string fechaFin, string mailUsuario);
        LocalidadDto[] ObtenerLocalidades();
        PatentesClienteDto ObtenerPatentes(int clienteId);
        List<SustitucionMOAModel.Dto.OrdenDeCarga.PlantaDto> ObtenerPlantas(string cuit);
        List<SustitucionMOAModel.Dto.OrdenDeCarga.DomicilioDto> ObtenerDomicilios(string cuit);
        TransportesIds ObtenerIdsTransportes(int clienteId, string patenteAcoplado);
        bool EsCuilCuitValido(string cuilCuit);
        GrabarOrdenResponse CrearNuevaOrden(OrdenResiduosDto ordenDto, string mailUsuario);
        OrdenResiduosDto ObtenerOrden(int idOrden);
        OrdenResiduosDto AnularOrden(int ordenId);
        GrabarOrdenResponse EditarOrden(OrdenResiduosDto ordenDto);
        OrdenResiduosDto VerificarTransporte(int ordenId);
        void VerificarVencimientoOrdenesResiduos();
        IList<DestinoScato> ObtenerDestinosMercaderia(string cuit);
    }
}
