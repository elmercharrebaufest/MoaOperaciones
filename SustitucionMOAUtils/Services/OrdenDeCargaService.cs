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
            ordenDeCarga.Estado = EstadoOrdenDeCarga.Pendiente;

            if (ordenDeCarga.CUITCliente != "")
                cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
            else
                cliente = usuario.ObtenerProveedor();

            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente_Id = cliente.Id;

            VerificarContrato(ordenDeCarga);

            VerificarCorredor(ordenDeCarga);

            VerificarTransporte(ordenDeCarga);

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

        private void VerificarContrato(OrdenDeCarga orden)
        {
            var contratosSAP = ObtenerContratos(orden.CUITCliente);

            if(contratosSAP.Count == 1)
            {
                orden.ContratoSAP = contratosSAP.First();
            }
            else
            {
                orden.ContratoSAP = "";
            }

            orden.ActualizarEstado();
        }

        private List<string> ObtenerContratos(string CUIT)
        {
            return new List<string> { "1231231", "515121"};
        }

        private List<string> ObtenerCorredores(string CUIT)
        {
            return new List<string> { "PEPE", "LUIS" };
        }

        private void VerificarCorredor(OrdenDeCarga orden)
        {
            var corredoresCliente = ObtenerCorredores(orden.CUITCliente);

            if (corredoresCliente.Count == 1)
            {
                orden.Corredor = corredoresCliente.First();
                orden.CorredorSeleccionado = true;
            }
            else
            {
                orden.CorredorSeleccionado = false;
            }

            orden.ActualizarEstado();
        }

        private void VerificarTransporte(OrdenDeCarga orden)
        {
            orden.TransporteExiste = TransporteExiste(orden.CUITTransporte);
        
            orden.ActualizarEstado();
        }

        private bool TransporteExiste (string CUIT)
        {
            return CUIT.Contains("7");
        }
    }
}
