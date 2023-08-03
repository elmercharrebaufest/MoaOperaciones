using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaFasonService : IOrdenDeCargaServiceBase
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
        ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit);
        bool EmailGestionarAlta(string cuit, string razonSocial, bool esIntermediarioFlete);
        List<PlantaDto> ObtenerPlantasDestino(string destinoCuit);
        List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit);
    }
}
