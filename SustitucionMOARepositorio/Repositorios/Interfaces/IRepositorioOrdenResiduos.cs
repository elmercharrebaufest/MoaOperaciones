using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioOrdenResiduos : IRepositorio
    {
        MaterialDto[] ObtenerMateriales();
        List<OrdenResiduosFila> ObtenerListadoOrdenes(DateTime fechaInicio, DateTime fechaFin, bool esInterno);
        LocalidadDto[] ObtenerLocalidades();
        List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientesResiduos();
        PatentesClienteDto ObtenerPatentesDeOrdenes(int clienteId);
        TransportesIds ObtenerIdsTransportes(int clienteId, string patenteAcoplado);
        OrdenResiduos ObtenerOrdenResiduos(int idOrden);
        Usuario ObtenerUsuarioSegunMail(string mailUsuario);
        Proveedor ObtenerProveedor(int idProveedor);
    }
}
