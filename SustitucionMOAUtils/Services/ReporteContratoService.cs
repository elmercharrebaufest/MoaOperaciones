using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
            string mailUsuario,
            string proveedor,
            string fechaInicio,
            string fechaFin,
            bool mostrarPendientes,
            ReporteContratoWSMOAResponse dataFiltro)
        {
            if (string.IsNullOrEmpty(proveedor))
            {
                throw new ValidationCustomException("El Cliente/Corredor no puede estar vacío");
            }
            if ((Convert.ToDateTime(fechaFin) - Convert.ToDateTime(fechaInicio)).TotalDays > 240)
            {
                throw new ValidationCustomException("El rango de fecha no puede ser mayor a 240 días.");
            }

            var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(us => us.Mail == mailUsuario);

            if(usuario == null)
            {
                throw new ValidationCustomException("El usuario no existe. Reinicie su sesión.");
            }

            Proveedor proveedorDB;


            if (usuario.TieneRol(RolEnum.Multifirma))
            {
                proveedorDB = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == proveedor);
            }
            else
            {
                proveedorDB = usuario.ObtenerProveedorAsignado();
            }
            

            var request = new ReporteContratoWSMOARequest()
            {
                Cliente = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? "" : proveedor,
                Pendiente = mostrarPendientes  ? "X" : "",
                Corredor = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? proveedor : "",
                Material = "",
                TipoContrato = "",
                Contrato = "",
                Fechas = new List<FechaWS> { new FechaWS { fechaInicio = Convert.ToDateTime(fechaInicio),
                                                               fechaFin = Convert.ToDateTime(fechaFin)}
                    }
            };

            ReporteContratoViewModel view = new ReporteContratoViewModel();
            if (dataFiltro == null)
            {
                view.data = consumer.ReporteContratoExecute(request);
            }
            else
            {
                view.data = dataFiltro;
            }
            validarRespuesta(dataFiltro == null ? view.data : dataFiltro);
            var obtenerAgrupado = obtenerAgrupadoProducto(view);
            view.data.Resultados = obtenerAgrupado.data.Resultados.OrderBy(o => o.DescripcionMaterial).ThenBy(o => o.FechaDesde).ToList();
            return view;
        }

        public ReporteContratoViewModel obtenerAgrupadoProducto(ReporteContratoViewModel view)
        {
            view.filtroCliente = new DropdownContent(
                    view.data.Resultados
                        .GroupBy(i => i.NombreClienteCUIT)
                        .Select(x => new DropdownOption
                        {
                            value = x.Key,
                            label = x.Key + " (" + x.Count() + ")"
                        })
                        .OrderBy(opt => opt.label)
                        .ToList());

            view.filtroProducto = new DropdownContent(
                    view.data.Resultados
                        .GroupBy(i => i.DescripcionMaterial)
                        .Select(x => new DropdownOption
                        {
                            value = x.Key,
                            label = x.Key + "(" + x.Count() + ")"
                        }).ToList());

            view.filtroTipoContrato = new DropdownContent(
                    view.data.Resultados
                        .GroupBy(i => i.TipoContrato)
                        .Select(x => new DropdownOption
                        {
                            value = x.Key,
                            label = x.Key + "(" + x.Count() + ")"
                        }).ToList());

            view.filtroNroContrato = new DropdownContent(
                    view.data.Resultados
                        .GroupBy(i => i.Contrato)
                        .Select(x => new DropdownOption
                        {
                            value = x.Key,
                            label = x.Key + "(" + x.Count() + ")"
                        }).ToList());

            var listaPorProducto = view.data.Resultados.OrderByDescending(x => x.DescripcionMaterial).ToList();
            var agrupadoProducto = view.data.Resultados.GroupBy(x => x.DescripcionMaterial).Select(x => new
            {
                descripcion = x.Key,
                kilosEntregados = x.Sum(i => i.KilosEntregados),
                kilosPendientes = x.Sum(i => i.KilosPendienteEntrega),
                kilosTotales = x.Sum(i => i.KilosTotales)

            }).ToList();

            foreach (var rowAgrupado in agrupadoProducto)
            {
                var resultado = new Result();
                resultado.TipoContrato = "";
                resultado.Contrato = "";
                resultado.NombreCliente = "";
                resultado.NombreClienteCUIT = "";
                resultado.FechaDesde = "";
                resultado.Corredor = "TOTAL";
                resultado.KilosTotalesStr = "";
                resultado.KilosPendienteEntregaStr = "";
                resultado.DescripcionMaterial = rowAgrupado.descripcion;
                resultado.KilosEntregadosStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosEntregados, "KG");
                resultado.KilosTotalesStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosTotales, "KG");
                resultado.KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(rowAgrupado.kilosPendientes, "KG");
                resultado.KilosEntregados = rowAgrupado.kilosEntregados;
                resultado.KilosTotales = rowAgrupado.kilosTotales;
                resultado.KilosPendienteEntrega = rowAgrupado.kilosPendientes;
                resultado.ColorProducto = consumer.SetearColorProducto((listaPorProducto.FirstOrDefault(x => x.DescripcionMaterial == rowAgrupado.descripcion).Producto).Trim('0'));
                view.data.Resultados.Add(resultado);

            }

            return view;
        }

        private void validarRespuesta(ReporteContratoWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.Resultados == null || data.Resultados.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Contratos"));
        }

        public ReporteContratoViewModel GetContratosDetalle(string contrato, string proveedor, string fechaInicio, string fechaFin)
        {
            var proveedorDB = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == proveedor);
            var request = new ReporteContratoWSMOARequest();

            var dateInicio = DataFormatter.StringToDateTime(fechaInicio, "");
            var dateFin = DataFormatter.StringToDateTime(fechaFin, "");

            request = new ReporteContratoWSMOARequest()
            {
                //Se envia corredor o Cliente para efectos de mas rapidez en la consulta a la rfc
                Cliente = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? "" : proveedor,
                Corredor = proveedorDB.TipoProveedor.NombreCorto == "CORR" ? proveedor : "",
                Contrato = contrato,
                Fechas = new List<FechaWS> { new FechaWS { fechaInicio = dateInicio,fechaFin = dateFin}}
            };

            ReporteContratoViewModel view = new ReporteContratoViewModel();
            view.data = consumer.ReporteContratoExecute(request);
            return view;
        }
    }
}
