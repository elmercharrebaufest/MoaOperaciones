using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
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
    public class PesificacionService
    {
        FeriadoService _feriadoService = new FeriadoService();

        public Fecha GetFechaPesificacion(string formatoFecha)
        {
            try
            {
                var feriados = _feriadoService.ObtenerFeriados();
                string horaDeCorte = ConfigurationManager.AppSettings["HoraCortePesificaciones"];

                //Si se paso la hora de corte la fecha minima es manana, caso contrario es hoy
                TimeSpan ts = TimeSpan.Parse(horaDeCorte);
                DateTime dateTimeCorte = DateTime.Today.Add(ts);
                DateTime dateTimePesificacion = dateTimeCorte < DateTime.Now ? DateTime.Today.AddDays(1) : DateTime.Today;
                dateTimePesificacion = ObtenerProximoDiaHabil(dateTimePesificacion, feriados);
                Fecha fecha = new Fecha()
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

                Contrato contratoEncontrado = responseGet.Contratos.Find(x => x.NroContrato.TrimStart(new Char[] { '0' }) == contrato && x.Fijacion.TrimStart(new Char[] { '0' }) == fijacion && x.CantidadPendiente > 0);
                if (contratoEncontrado == null)
                {
                    throw new InfoCustomException("El contrato que desea pesificar no se encuentra o bien ya fue pesificado completamente");
                }

                if (contratoEncontrado.CantidadPendiente < cantidad)
                {
                    throw new InfoCustomException("El Contrato " + contrato + " dispone de " + contratoEncontrado.CantidadPendiente + " " + contratoEncontrado.Unidad + " por pesificar. Por favor ingrese una cantidad igual o menor a la pendiente");
                }

                List<ZMPES5480> comprobantes = new List<ZMPES5480>
                {
                    new ZMPES5480()
                    {
                        CONTRATO = contratoEncontrado.NroContrato,
                        FIJACION = contratoEncontrado.Fijacion,
                        CANTIDAD = cantidad,
                        FECHA = GetFechaPesificacion("yyyyMMdd").FechaPesificacion,
                        IMPORTE = contratoEncontrado.Precio,
                        MONEDA = contratoEncontrado.Moneda,
                        UNIDAD = contratoEncontrado.Unidad,
                        CANTIDADSpecified = true,
                        IMPORTESpecified = true
                    }
                };

                PesificacionSetContratosWSMOAResponse responseSet = (PesificacionSetContratosWSMOAResponse)new PesificacionGuardarConsumerMOA().request(comprobantes.ToArray());
                if (responseSet == null)
                {
                    throw new Exception(ErrorMsg.ErrorWS);
                }
                if (responseSet != null && responseSet.Log.Count > 0 && responseSet.Log[0].Mensaje != "")
                {
                    //throw new InfoCustomException(responseSet.Log[0].Mensaje);
                    throw new InfoCustomException(ErrorMsg.Error);
                }

                return responseSet;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
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
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
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

                return responseGet.Contratos.Where(a => a.CantidadPendiente > 0).ToList();
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
