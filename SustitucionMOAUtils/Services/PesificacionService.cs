using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.PesificacionGuadarWebServiceMOA;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class PesificacionService : IPesificacionService
    {
        readonly FeriadoService _feriadoService = new FeriadoService();
        readonly IListarPesificacionesConsumer pesificacionesConsumer;
        readonly IRepositorio repositorio;
        readonly string HORA_CORTE_PESIFICACIONES_CODE = "HoraCortePesificaciones";

        public PesificacionService(IListarPesificacionesConsumer pesificacionesConsumer, IRepositorio repositorio)
        {
            this.pesificacionesConsumer = pesificacionesConsumer;
            this.repositorio = repositorio;
        }

        public Fecha GetFechaPesificacion(string formatoFecha)
        {
            try
            {
                var feriados = _feriadoService.ObtenerFeriados();

                var horaCorteDb = repositorio.Obtener<Configuracion>(c => c.Code == HORA_CORTE_PESIFICACIONES_CODE);

                string horaDeCorte = horaCorteDb != null ? horaCorteDb.Value : ConfigurationManager.AppSettings[HORA_CORTE_PESIFICACIONES_CODE];

                //Si se paso la hora de corte la fecha minima es manana, caso contrario es hoy
                TimeSpan ts = TimeSpan.Parse(horaDeCorte);
                DateTime dateTimeCorte = DateTime.Today.Add(ts);
                DateTime dateTimePesificacion = dateTimeCorte < DateTime.Now ? DateTime.Today.AddDays(1) : DateTime.Today;
                dateTimePesificacion = ObtenerProximoDiaHabil(dateTimePesificacion, feriados);
                Fecha fecha = new Fecha
                {
                    HoraDeCorte = horaDeCorte,
                    FechaPesificacion = dateTimePesificacion.ToString(formatoFecha),
                    DateTimePesificacion = dateTimePesificacion,
                    DateTimeCorte = dateTimeCorte
                };

                return fecha;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private DateTime ObtenerProximoDiaHabil(DateTime fecha, List<DateTime> feriados)
        {
            if (feriados.Contains(fecha.Date) || fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                fecha = fecha.AddDays(1);
                return ObtenerProximoDiaHabil(fecha, feriados);
            }
            else
            {
                return fecha;
            }
        }

        public PesificacionSetContratosWSMOAResponse SetContrato(string proveedor, string contrato, string fijacion, decimal cantidad)
        {
            try
            {
                if (contrato == null || contrato == "")
                {
                    throw new ValidationCustomException("Debe ingresar un contrato");
                }

                if (cantidad <= 0)
                {
                    throw new ValidationCustomException("Debe ingresar una cantidad mayor a 0");
                }

                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException("Debe ingresar un proveedor");
                }

                PesificacionGetContratosWSMOAResponse responseGet = (PesificacionGetContratosWSMOAResponse)new PesificacionConsumerMOA().request(proveedor);
                if (responseGet == null || responseGet.Contratos.Count < 1)
                {
                    throw new InfoCustomException("No se encontraron contratos para pesificar");
                }

                Contrato contratoEncontrado =
                        responseGet.Contratos.
                        Find(x => x.NroContrato.TrimStart(new Char[] { '0' }) == contrato
                        && x.Fijacion.TrimStart(new Char[] { '0' }) == fijacion
                        && x.CantidadPendiente > 0);

                if (contratoEncontrado == null)
                {
                    throw new InfoCustomException("El contrato que desea pesificar no se encuentra o bien ya fue pesificado completamente");
                }

                if (contratoEncontrado.CantidadPendiente < cantidad)
                {
                    throw new InfoCustomException("El Contrato "
                                                  + contrato
                                                  + " dispone de "
                                                  + contratoEncontrado.CantidadPendiente
                                                  + " "
                                                  + contratoEncontrado.Unidad
                                                  + " por pesificar. Por favor ingrese una cantidad igual o menor a la pendiente");
                }

                string FechaPesificacion = GetFechaPesificacion("yyyyMMdd").FechaPesificacion;

                if (int.Parse(contratoEncontrado.NroContrato) >= 2500000 && int.Parse(contratoEncontrado.NroContrato) < 2700000)
                {
                    var soja200 = GetSoja200();
                    if (soja200.Desde <= DateTime.Now.Date && soja200.Hasta >= DateTime.Now.Date)
                    {
                        FechaPesificacion = soja200.FechaCotizacion.ToString("yyyy-MM-dd");
                    }
                }
                else if (int.Parse(contratoEncontrado.NroContrato) >= 2100000 && int.Parse(contratoEncontrado.NroContrato) < 2300000)
                {
                    var dolarGirasol = GetDolarGirasol();
                    if (dolarGirasol != null)
                    {
                        FechaPesificacion = dolarGirasol.FechaCotizacion.ToString("yyyy-MM-dd");
                    }
                }
                else if (int.Parse(contratoEncontrado.NroContrato) >= 2700000 && int.Parse(contratoEncontrado.NroContrato) <= 2899999)
                {
                    var dolarMaiz = GetDolarMaiz();
                    if (dolarMaiz != null)
                    {
                        FechaPesificacion = dolarMaiz.FechaCotizacion.ToString("yyyy-MM-dd");
                    }
                }
                List<ZMPES5480> comprobantes = new List<ZMPES5480>
                {
                    new ZMPES5480()
                    {
                        CONTRATO = contratoEncontrado.NroContrato,
                        FIJACION = contratoEncontrado.Fijacion,
                        CANTIDAD = cantidad,
                        FECHA = FechaPesificacion,
                        IMPORTE = contratoEncontrado.Precio,
                        MONEDA = contratoEncontrado.Moneda,
                        UNIDAD = contratoEncontrado.Unidad,
                        CANTIDADSpecified = true,
                        IMPORTESpecified = true
                    }
                };

                try { Logger.Log.Debug("PesificacionService", "SetContrato", comprobantes.ToJson()); } catch (Exception e) { }
                PesificacionSetContratosWSMOAResponse responseSet = (PesificacionSetContratosWSMOAResponse)new PesificacionGuardarConsumerMOA().request(comprobantes.ToArray());
                if (responseSet == null)
                {
                    throw new Exception(ErrorMsg.ErrorWS);
                }
                if (responseSet != null && responseSet.Log.Count > 0 && responseSet.Log[0].Mensaje != "")
                {
                    throw new InfoCustomException(ErrorMsg.Error);
                }

                return responseSet;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string SetContratos(string proveedor, HttpPostedFileBase file)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException("Debe ingresar un proveedor");
                }

                if (file == null || file.ContentLength == 0 || Path.GetExtension(file.FileName).ToLower() != ".csv")
                {
                    throw new ValidationCustomException("Debe seleccionar un archivo .csv valido");
                }

                string fileName = proveedor + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string targetFolder = HttpContext.Current.Server.MapPath("~/Pesificados");
                string targetPath = Path.Combine(targetFolder, fileName);
                file.SaveAs(targetPath);

                return SuccessMsg.EnvioMsjOk;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public List<Contrato> GetContratos(string proveedor)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException("Debe ingresar un proveedor");
                }

                PesificacionGetContratosWSMOAResponse responseGet = (PesificacionGetContratosWSMOAResponse)new PesificacionConsumerMOA().request(proveedor);
                if (responseGet == null || responseGet.Contratos.Where(a => a.CantidadPendiente > 0).ToList().Count < 1)
                {
                    throw new InfoCustomException("No se encontraron contratos para pesificar");
                }

                return responseGet.Contratos.Where(a => a.CantidadPendiente > 0)
                    .Select(x => new Contrato
                    {
                        NroContrato = x.NroContrato.TrimStart('0'),
                        Fijacion = x.Fijacion.TrimStart('0'),
                        CantidadPendiente = x.CantidadPendiente,
                        Moneda = x.Moneda,
                        MontoPendiente = x.MontoPendiente,
                        NombreVendedor = x.NombreVendedor,
                        Precio = x.Precio,
                        Unidad = x.Unidad,
                        Vendedor = x.Vendedor
                    }).ToList();
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


        public List<PesificacionSapDto> GetPesificacionesSAP(string proveedor)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException("Debe ingresar un proveedor");
                }

                var responseGet = pesificacionesConsumer.Request(proveedor);

                if (responseGet == null)
                {
                    throw new InfoCustomException("No se encontraron pesificaciones");
                }

                return responseGet.Pesificaciones;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public DolarMaterialDto GetSoja200()
        {
            try
            {
                DolarMaterialDto soja200 = new DolarMaterialDto
                {
                    Desde = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "Soja200Desde").Value, "yyyy-MM-dd", null),
                    Hasta = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "Soja200Hasta").Value, "yyyy-MM-dd", null),
                    FechaCotizacion = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "Soja200FechaCotizacion").Value, "yyyy-MM-dd", null),
                    Cotizacion = double.Parse(repositorio.Obtener<Configuracion>(a => a.Code == "Soja200Cotizacion").Value),
                };

                return soja200;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
        public DolarMaterialDto GetDolarGirasol()
        {
            try
            {
                var now = DateTime.Now.Date;
                var desde = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarGirasolDesde").Value, "yyyy-MM-dd", null);
                var hasta = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarGirasolHasta").Value, "yyyy-MM-dd", null);
                if (!(desde <= now && hasta >= now))
                    return null;

                DolarMaterialDto dolarGirasol = new DolarMaterialDto()
                {
                    Desde = desde,
                    Hasta = hasta,
                    FechaCotizacion = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarGirasolFechaCotizacion").Value, "yyyy-MM-dd", null),
                    Cotizacion = double.Parse(repositorio.Obtener<Configuracion>(a => a.Code == "DolarGirasolCotizacion").Value),
                };

                return dolarGirasol;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
        public DolarMaterialDto GetDolarMaiz()
        {
            try
            {
                var now = DateTime.Now.Date;
                var desde = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarMaizDesde").Value, "yyyy-MM-dd", null);
                var hasta = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarMaizHasta").Value, "yyyy-MM-dd", null);
                if (!(desde <= now && hasta >= now))
                    return null;

                DolarMaterialDto dolarMaiz = new DolarMaterialDto()
                {
                    Desde = desde,
                    Hasta = hasta,
                    FechaCotizacion = DateTime.ParseExact(repositorio.Obtener<Configuracion>(a => a.Code == "DolarMaizFechaCotizacion").Value, "yyyy-MM-dd", null),
                    Cotizacion = double.Parse(repositorio.Obtener<Configuracion>(a => a.Code == "DolarMaizCotizacion").Value),
                };

                return dolarMaiz;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
