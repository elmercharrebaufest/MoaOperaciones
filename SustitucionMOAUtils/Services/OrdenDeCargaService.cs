using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
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

        public string Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Proveedor cliente;

            if (ordenDeCarga.CUITCliente != "")
                 cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
            else
                 cliente = usuario.ObtenerProveedor();

            ordenDeCarga.Estado = EstadoOrdenDeCarga.Pendiente;
            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente_Id = cliente.Id;

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAgregada;
        }

        public List<OrdenDeCargaDto> Listar(string mailUsuario)
        {

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var clientes = usuario.Proveedores.Select(c => c.Id);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (usuario.EsAdmin())
            {
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id)).Select(x => new OrdenDeCargaDto
                {
                    Id = x.Id,
                    CUITCliente = x.CUITCliente,
                    DescripcionEstado = x.Estado.ToString(),
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),

                }).ToList();
            }
            else
            {
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id)).Select(x => new OrdenDeCargaDto
                {
                    Id = x.Id,
                    CUITCliente = x.CUITCliente,
                    DescripcionEstado = x.Estado.ToString(),
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),
                }).ToList();
            }

            return listado;
        }
    }
}
