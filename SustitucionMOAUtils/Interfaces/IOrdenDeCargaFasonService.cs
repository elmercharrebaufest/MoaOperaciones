using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaFasonService
    {
        ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request);
        DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario);
        string VerificarTransporte(int ordenId);
        void VerificarTransporteJob();
        List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason();
        object ObtenerDestinos(int clienteId);
        Resultado Crear(CrearOrdenDeCargaFasonRequest request, string mailUsuario);
        Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario);
        List<ProveedorDto> GetCorredores();
        List<ProveedorDto> GetClientesDeCorredor(string codigoCorredor);
    }
}
