using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
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

        public ReporteContratoService(IReporteContratoConsumerMOA consumer)
        {
            this.consumer = consumer;

        }

        public ReporteContratoViewModel GetContratosReporte(string proveedor, string fechaInicio, string fechaFin)
        {
            List<FechaWS> fechas = new List<FechaWS>();

            var request = new ReporteContratoWSMOARequest()
            {
                //Cliente = proveedor,
                Pendiente = "X",
                Corredor = "",
                Fechas = new List<FechaWS> { new FechaWS { fechaInicio = Convert.ToDateTime(fechaInicio),
                                                           fechaFin = Convert.ToDateTime(fechaFin)} 
                }

            };
            ReporteContratoViewModel view = new ReporteContratoViewModel();
            view.data = consumer.ReporteContratoExecute(request);
            view.totales = new List<Totales>();
            validarRespuesta(view.data);
            view.filtroCliente = new DropdownContent(view.data.Resultados.GroupBy(i => i.NombreCliente).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
            view.filtroProducto = new DropdownContent(view.data.Resultados.GroupBy(i => i.DescripcionMaterial).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
            view.filtroTipoContrato = new DropdownContent(view.data.Resultados.GroupBy(i => i.TipoContrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
            view.filtroNroContrato = new DropdownContent(view.data.Resultados.GroupBy(i => i.Contrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + "(" + x.Count() + ")" }).ToList());
            var comparacion = view.data.Resultados.OrderByDescending(x => x.DescripcionMaterial).ToList();
            var totales = view.data.Resultados
                .GroupBy
                (x => x.DescripcionMaterial)
                .Select(x => new
                {
                    descripcion = x.Key,
                    kilosEnregados = x.Sum(i => i.KilosEntregados),
                    kilosPendientes = x.Sum(i => i.KilosPendienteEntrega),
                    kilosTotales = x.Sum(i => i.KilosTotales)

                }).ToList();


            foreach (var row in totales)
            {
                foreach (var row1 in comparacion)
                {
                    if (row.descripcion != row1.DescripcionMaterial || totales.Count == 1)
                    {
                        var res = new Result();
                        res.TipoContrato = "";
                        res.Contrato = "";
                        res.NombreCliente = "";
                        res.FechaDesde = "";
                        res.Corredor = "";
                        res.KilosTotalesStr = "";
                        res.KilosPendienteEntregaStr = "";
                        res.DescripcionMaterial = row.descripcion;
                        res.KilosEntregadosStr = SAPFormatter.FormatearCantidad(row.kilosEnregados, "KG");
                        res.KilosTotalesStr = SAPFormatter.FormatearCantidad(row.kilosTotales, "KG");
                        res.KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(row.kilosPendientes, "KG");
                        res.ColorProducto = consumer.SetearColorProducto((comparacion.FirstOrDefault(x => x.DescripcionMaterial == row.descripcion).Producto).Trim('0'));
                        view.data.Resultados.Add(res);
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

        //public List<Result> GetFiltroPorColumna(Result cabecera)
        //{

        //    return cabecera ;
        //}
    }
}
