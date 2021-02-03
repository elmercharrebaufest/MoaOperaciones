using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{

    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;

        public OrdenDeCargaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public string Agregar(OrdenDeCarga ordenDeCarga)
        {
            ordenDeCarga.Estado = EstadoOrdenDeCarga.Pendiente;

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAgregada;
        }

        public List<OrdenDeCargaDto> Listar(Usuario usuario)
        {
            var clientes = usuario.Proveedores.Select(c => c.Id);

            var listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.ClienteId)).Select(x => new OrdenDeCargaDto
            {
                Id = x.Id
            }).ToList();

            return listado;
        }
    }
}
