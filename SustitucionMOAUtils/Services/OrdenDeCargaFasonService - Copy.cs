using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class OrdenesCargaApi : IOrdenesCargaApi
    {
        private readonly IRepositorio _repositorio;
        public OrdenesCargaApi(IRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public List<OrdendesDeCargaApiDto> ObtenerOrdenes()
        {
            var ordenesFas = _repositorio.Listar<OrdenDeCarga>(x => x.Estado == EstadoOrdenDeCarga.Pendiente).ToList();
            var ordenesFason = _repositorio.Listar<OrdenDeCargaFason>(x => x.Estado == 0).ToList();
            List<OrdendesDeCargaApiDto> listaOrdenes = new List<OrdendesDeCargaApiDto>();

            foreach (var ordenFas in ordenesFas)
            {
                var ordenFasDto = new OrdendesDeCargaApiDto();
                ordenFasDto.Id = ordenFas.Id;
                ordenFasDto.NombreChofer = ordenFas.NombreChofer;
                ordenFasDto.FechaCreacion = ordenFas.FechaCarga.ToString();
                ordenFasDto.CUITCliente = ordenFas.CUITCliente;
                ordenFasDto.CUILChofer = ordenFas.CUITChofer;
                ordenFasDto.CUITTransporte = ordenFas.CUITTransporte;
                ordenFasDto.RazonSocialTransporte = ordenFas.RazonSocialTransporte;
                ordenFasDto.Cantidad = ordenFas.Cantidad;
                ordenFasDto.Contrato = ordenFas.ContratoIngresado;
                ordenFasDto.Observacion = ordenFas.Observacion;
                ordenFasDto.PatenteAcoplado = ordenFas.PatenteAcoplado;
                ordenFasDto.PatenteChasis = ordenFas.ChasisAcoplado;
                ordenFasDto.Pedido = ordenFas.NumeroPedido;
                ordenFasDto.DescripcionProducto = ordenFas.Producto.Nombre;
                ordenFasDto.TipoOrden = "OrdenCargaFas";
                listaOrdenes.Add(ordenFasDto);

            }

            foreach (var ordenFason in ordenesFason)
            {
                var ordenFasonDto = new OrdendesDeCargaApiDto();
                ordenFasonDto.Id = ordenFason.Id;
                ordenFasonDto.FechaCreacion = ordenFason.FechaCreacion.ToString();
                ordenFasonDto.FechaRetiro = ordenFason.FechaRetiro.ToString();
                ordenFasonDto.Cantidad = ordenFason.Cantidad;
                ordenFasonDto.PatenteAcoplado = ordenFason.PatenteAcoplado;
                ordenFasonDto.PatenteChasis = ordenFason.PatenteChasis;
                ordenFasonDto.NombreChofer = ordenFason.NombreChofer;
                ordenFasonDto.CUILChofer = ordenFason.CUILChofer;
                ordenFasonDto.RazonSocialTransporte = ordenFason.RazonSocialTransporte;
                ordenFasonDto.CUITTransporte = ordenFason.CUITTransporte;
                ordenFasonDto.Destino = ordenFason.Destino;
                ordenFasonDto.CantidadDeViajesEsperados = ordenFason.CantidadDeViajesEsperados;
                ordenFasonDto.CantidadDeViajesRealizados = ordenFason.CantidadDeViajesRealizados;
                ordenFasonDto.Observacion = ordenFason.Observacion;
                ordenFasonDto.Cliente = ordenFason.Cliente.RazonSocial;
                ordenFasonDto.DescripcionProducto = ordenFason.Producto.Nombre;
                ordenFasonDto.TipoOrden = "OrdenCargaFason";
                listaOrdenes.Add(ordenFasonDto);

            }

            return listaOrdenes;
        }
    }
}
