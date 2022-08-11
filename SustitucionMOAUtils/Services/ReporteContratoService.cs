using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class ReporteContratoService : IReporteContratoService
    {
        protected readonly IReporteContratoConsumerMOA consumer;
        protected readonly IRepositorio repositorio;

        public ReporteContratoService(IReporteContratoConsumerMOA consumer, IRepositorio repositorio)
        {
            this.consumer = consumer;
            this.repositorio = repositorio;

        }

        public ReporteContratoViewModel GetContratosReporte(
            string proveedor, 
            string fechaInicio,
            string fechaFin, 
            string clienteFiltro, 
            string productoFiltro,
            string tipoContrato, 
            bool mostrarPendientes,
            bool esFiltro, 
            string mailUsuario)
        {
            List<FechaWS> fechas = new List<FechaWS>();
            var proveedorDB = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == proveedor);
            var request = new ReporteContratoWSMOARequest();
            var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(u => u.Mail == mailUsuario);
            var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");
            
            if (esFiltro)
            {
                request = new ReporteContratoWSMOARequest()
                {
                    Cliente = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? clienteFiltro :proveedor,
                    Pendiente = mostrarPendientes ? "X":"",
                    Corredor = "",
                    Material = productoFiltro == "" ? "" : productoFiltro,
                    TipoContrato = tipoContrato,
                    Fechas = new List<FechaWS> { new FechaWS
                      {
                        fechaInicio = Convert.ToDateTime(fechaInicio),
                        fechaFin = Convert.ToDateTime(fechaFin)

                      }
                    }

                };
            }
            else
            {

                request = new ReporteContratoWSMOARequest()
                {
                    Cliente = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? "" : proveedor,
                    Pendiente = mostrarPendientes ? "X" : "",
                    Corredor = (!esComercial && usuario.TipoUsuario.NombreCorto == "CORR") ? proveedor : "",
                    Material = productoFiltro == "" ? "" : productoFiltro,
                    TipoContrato = tipoContrato,
                    Fechas = new List<FechaWS> { new FechaWS { fechaInicio = Convert.ToDateTime(fechaInicio),
                                                               fechaFin = Convert.ToDateTime(fechaFin)}
                    }
                };

            }


            ReporteContratoViewModel view = new ReporteContratoViewModel();
            view.data = consumer.ReporteContratoExecute(request);
            validarRespuesta(view.data);
            if (!esFiltro)
            {
                view.filtroCliente = new DropdownContent(view.data.Resultados.GroupBy(i => i.NombreCliente).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                view.filtroProducto = new DropdownContent(view.data.Resultados.GroupBy(i => i.DescripcionMaterial).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
                view.filtroTipoContrato = new DropdownContent(view.data.Resultados.GroupBy(i => i.TipoContrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
                view.filtroNroContrato = new DropdownContent(view.data.Resultados.GroupBy(i => i.Contrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
            }
            var listaPorProducto = view.data.Resultados.OrderByDescending(x => x.DescripcionMaterial).ToList();
            var agrupadoProducto = view.data.Resultados.GroupBy(x => x.DescripcionMaterial).Select(x => new
            {
                descripcion = x.Key,
                kilosEnregados = x.Sum(i => i.KilosEntregados),
                kilosPendientes = x.Sum(i => i.KilosPendienteEntrega),
                kilosTotales = x.Sum(i => i.KilosTotales)

            }).ToList();

            foreach (var rowAgrupado in agrupadoProducto)
            {
                foreach (var rowProducto in listaPorProducto)
                {
                    if (rowAgrupado.descripcion != rowProducto.DescripcionMaterial)
                    {
                        var resultado = new Result();
                        resultado.TipoContrato = "";
                        resultado.Contrato = "";
                        resultado.NombreCliente = "";
                        resultado.FechaDesde = "";
                        resultado.Corredor = "";
                        resultado.KilosTotalesStr = "";
                        resultado.KilosPendienteEntregaStr = "";
                        resultado.DescripcionMaterial = rowAgrupado.descripcion;
                        resultado.KilosEntregadosStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosEnregados, "KG");
                        resultado.KilosTotalesStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosTotales, "KG");
                        resultado.KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosPendientes, "KG");
                        resultado.ColorProducto = consumer.SetearColorProducto((listaPorProducto.FirstOrDefault(x => x.DescripcionMaterial == rowAgrupado.descripcion).Producto).Trim('0'));
                        view.data.Resultados.Add(resultado);
                        break;


                    }
                }


            }

            view.data.Resultados = view.data.Resultados.OrderByDescending(x => x.FechaDesde).OrderByDescending(y => y.DescripcionMaterial).ToList();
            return view;
        }

        private void validarRespuesta(ReporteContratoWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.Resultados == null || data.Resultados.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Contratos"));
        }
    }
}
