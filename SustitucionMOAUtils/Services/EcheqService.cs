using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class EcheqService : IEcheqService
    {
        private readonly IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA;
        private readonly IEcheqModificarContratoConsumerMOA echeqModificarContratoConsumerMOA;
        private readonly IEcheqModificarFijacionConsumerMOA echeqModificarFijacionConsumerMOA;
        private readonly IEcheqModificacionDocumentoChequeConsumerMOA echeqModificacionDocumentoChequeConsumerMOA;
        private readonly IEcheqAnularAperturaChequeConsumerMOA echeqAnularAperturaChequeConsumerMOA;
        private readonly IEcheqCargaAperturaChequeConsumerMOA echeqCargaAperturaChequeConsumerMOA;
        private readonly IRepositorio repositorio;

        public EcheqService(IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA,
                            IEcheqModificarContratoConsumerMOA echeqModificarContratoConsumerMOA,
                            IEcheqModificarFijacionConsumerMOA echeqModificarFijacionConsumerMOA,
                            IEcheqModificacionDocumentoChequeConsumerMOA echeqModificacionDocumentoChequeConsumerMOA,
                            IEcheqAnularAperturaChequeConsumerMOA echeqAnularAperturaChequeConsumerMOA,
                            IEcheqCargaAperturaChequeConsumerMOA echeqCargaAperturaChequeConsumerMOA,
                            IRepositorio repositorio)
        {
            this.echeqVisualizarPendientePagoConsumerMOA = echeqVisualizarPendientePagoConsumerMOA;
            this.echeqModificarContratoConsumerMOA = echeqModificarContratoConsumerMOA;
            this.echeqModificarFijacionConsumerMOA = echeqModificarFijacionConsumerMOA;
            this.echeqModificacionDocumentoChequeConsumerMOA = echeqModificacionDocumentoChequeConsumerMOA;
            this.echeqAnularAperturaChequeConsumerMOA = echeqAnularAperturaChequeConsumerMOA;
            this.echeqCargaAperturaChequeConsumerMOA = echeqCargaAperturaChequeConsumerMOA;
            this.repositorio = repositorio;

        }

        public List<EcheqNegocioDto> ObtenerPendientePago(string proveedor, string fechaInicio, string fechaFin, string contrato)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);

                List<EcheqNegocioDto> result = echeqVisualizarPendientePagoConsumerMOA.Request(proveedor, fechas, contrato);

                result.Where(x => x.Clasificacion == "PRODUCTOR" && x.MarcaCheque == true).ToList().ForEach(x => x.Documentos.ForEach(k => k.MarcaCheque = true));
                List<string> documentos = result.Where(a => a.MarcaCheque).SelectMany(a => a.Documentos.Where(b => b.MarcaCheque).Select(b => b.Documento)).ToList();
                var aperturas = repositorio.Listar<EcheqApertura>(x => x.Estado == true &&
                   documentos.Contains(x.EcheqLiquidacion.Documento));
                foreach (var apertura in aperturas)
                {
                    EcheqLiquidacionDto liquidacion = result
                        .Where(a => a.Contrato == apertura.EcheqLiquidacion.EcheqNegocio.Contrato && a.Pedido == apertura.EcheqLiquidacion.EcheqNegocio.Pedido)
                        .SelectMany(a => a.Documentos).Where(a => a.Documento == apertura.EcheqLiquidacion.Documento).SingleOrDefault();
                    liquidacion.Aperturas.Add(new EcheqAperturaDto { OrdenCheque = apertura.OrdenCheque, ImporteCheque = apertura.ImporteCheque });
                }
                return result;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string MarcarContrato(EcheqRequestModel request)
        {
            try
            {
                //1- Obtener contrato desde la RFC y setear echeq
                EcheqNegocioDto echeqNegocio = this.ObtieneNegocio(request);


                //2- validar si es productor o acopiador
                if (echeqNegocio.Clasificacion == "PRODUCTOR")
                {
                    //contrato es fijo
                    //pedido es fijacion

                    ResultadoGenerico modificarNegocio;

                    //3.1- Si es productor validar si es contrato o pedido (para saber si usamos la RFC de fijacion o de contrato)
                    if (echeqNegocio.Pedido == "")
                    {
                        modificarNegocio = echeqModificarContratoConsumerMOA.Request(request.Contrato, "", "=");
                    }
                    else
                    {
                        modificarNegocio = echeqModificarFijacionConsumerMOA.Request(request.Contrato, "", request.Pedido, "=");
                    }

                    if (modificarNegocio.HayError)
                    {
                        throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                    }
                    else
                    {
                        if (repositorio.Existe<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido))
                        {
                            this.UpdateEcheq(request, true);
                        }
                        else
                        {
                            this.AgregarEcheq(request, echeqNegocio);
                        }
                    }
                }
                else if (echeqNegocio.Clasificacion == "ACOPIADOR")//3.2- Si es acopiador por cada una de las liquidaciones llamar a la rfc de marcar documento
                {
                    ResultadoGenerico modificarNegocio = new ResultadoGenerico();

                    foreach (var liquidacion in echeqNegocio.Documentos)
                    {
                        modificarNegocio = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", "", "=");

                        if (modificarNegocio.HayError)
                        {
                            throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                        }
                    }

                    if (repositorio.Existe<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido))
                    {
                        this.UpdateEcheq(request, true);
                    }
                    else
                    {
                        this.AgregarEcheq(request, echeqNegocio);
                    }
                }
                else
                {
                    throw new ValidationCustomException("Clasificacion de contrato no valida");
                }

                return "El contrato se marco correctamente";
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DesmarcarContrato(EcheqRequestModel request)
        {
            try
            {
                //1- Obtener contrato desde la RFC y setear echeq
                EcheqNegocioDto echeqNegocio = this.ObtieneNegocio(request);

                //2- validar si es productor o acopiador
                if (echeqNegocio.Clasificacion == "PRODUCTOR")
                {
                    ResultadoGenerico modificarNegocio;

                    if (echeqNegocio.Pedido == "")
                    {
                        modificarNegocio = echeqModificarContratoConsumerMOA.Request(request.Contrato, "", "");
                    }
                    else
                    {
                        modificarNegocio = echeqModificarFijacionConsumerMOA.Request(request.Contrato, "", request.Pedido, "");
                    }

                    if (modificarNegocio.HayError)
                    {
                        throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                    }
                    else
                    {
                        this.UpdateEcheq(request, false);
                    }
                }
                else if (echeqNegocio.Clasificacion == "ACOPIADOR")//3.2- Si es acopiador por cada una de las liquidaciones llamar a la rfc de marcar documento
                {
                    ResultadoGenerico modificarNegocio = new ResultadoGenerico();

                    foreach (var liquidacion in echeqNegocio.Documentos)
                    {
                        modificarNegocio = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", "", "");

                        if (modificarNegocio.HayError)
                        {
                            throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                        }
                    }

                    this.UpdateEcheq(request, false);
                }
                else
                {
                    throw new ValidationCustomException("Clasificacion de contrato no valida");
                }

                return "El contrato se desmarco correctamente";
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string MarcarDocumento(EcheqRequestModel request)
        {
            try
            {
                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, "2022", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", "", "=");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                //1- Obtener contrato desde la RFC y setear echeq
                EcheqNegocioDto echeqNegocioSAP = this.ObtieneNegocio(request);

                EcheqNegocio echeqDB = repositorio.Obtener<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido);

                if (echeqDB != null)
                {
                    echeqDB.MarcaCheque = true;
                    echeqDB.FechaModificacion = DateTime.Now;
                    echeqDB.UsuarioModificacionId = request.UsuarioCreacionId;

                    var docDB = echeqDB.Documentos.Where(x => x.Documento == request.Documento).SingleOrDefault();

                    if (docDB != null)
                    {
                        docDB.MarcaCheque = true;
                        docDB.FechaModificacion = DateTime.Now;
                        docDB.UsuarioModificacionId = request.UsuarioCreacionId;
                    }
                    else
                    {
                        //generar nuevo documento y agregar nuevo documento
                        var documentoDto = echeqNegocioSAP.Documentos.Where(x => x.Documento == request.Documento).SingleOrDefault();
                        var echeqLiquidacion = new EcheqLiquidacion(documentoDto);

                        echeqLiquidacion.MarcaCheque = true;
                        echeqLiquidacion.FechaModificacion = DateTime.Now;
                        echeqLiquidacion.UsuarioModificacionId = request.UsuarioCreacionId;

                        echeqDB.Documentos.Add(echeqLiquidacion);


                    }
                }
                else
                {
                    //generar echeqNegocio
                    EcheqNegocio negocio = new EcheqNegocio(echeqNegocioSAP);
                    MaterialFason material = repositorio.Obtener<MaterialFason>(x => x.CodigoSap == echeqNegocioSAP.MaterialCodigo);

                    if (material == null)
                    {
                        negocio.Material = new MaterialFason { CodigoSap = echeqNegocioSAP.MaterialCodigo, Nombre = echeqNegocioSAP.DescripcionMaterial };
                    }
                    else
                    {
                        negocio.MaterialId = material.Id;
                    }

                    negocio.MarcaCheque = true;
                    negocio.FechaCreacion = DateTime.Now;
                    negocio.UsuarioCreacionId = request.UsuarioCreacionId;
                    negocio.ProveedorId = request.ProveedorId;
                    negocio.Documentos = negocio.Documentos.Where(x => x.Documento == request.Documento).ToList();

                    var echeqLiquidacion = negocio.Documentos.Where(x => x.Documento == request.Documento).SingleOrDefault();

                    echeqLiquidacion.MarcaCheque = true;
                    echeqLiquidacion.FechaCreacion = DateTime.Now;
                    echeqLiquidacion.UsuarioCreacionId = request.UsuarioCreacionId;

                    repositorio.Agregar(negocio);

                }
                repositorio.GuardarCambios();

                return "El documento se marco correctamente";
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DesmarcarDocumento(EcheqRequestModel request)
        {
            try
            {

                EcheqLiquidacion liquidacionExistente = ObtenerLiquidacionPorDocumento(request.Documento);

                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, liquidacionExistente.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", "", "");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                foreach (var apertura in liquidacionExistente.Aperturas.Where(a => a.Estado))
                {
                    result = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacionExistente.Documento, liquidacionExistente.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
                    apertura.Estado = false;
                    apertura.UsuarioModificacionId = request.UsuarioCreacionId;
                    apertura.FechaModificacion = DateTime.Now;
                }
                UpdateLiquidacion(request, false, liquidacionExistente);

                EcheqNegocioDto echeqNegocio = ObtieneNegocio(request);

                if (echeqNegocio.Documentos.Where(x => x.MarcaCheque).Count() == 0)
                {
                    DesmarcarContrato(request);
                }

                return "El documento se desmarco correctamente";
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private EcheqNegocioDto ObtieneNegocio(EcheqRequestModel request)
        {
            List<EcheqNegocioDto> contratosEcheq = ObtenerPendientePago(request.CodigoProveedor, DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), request.Contrato);
            EcheqNegocioDto echeqNegocio = null;

            if (string.IsNullOrEmpty(request.Pedido))
            {
                echeqNegocio = contratosEcheq.FirstOrDefault();
            }
            else
            {
                echeqNegocio = contratosEcheq.Where(x => x.Pedido == request.Pedido).FirstOrDefault();
            }

            if (echeqNegocio == null)
            {
                throw new ValidationCustomException("El contrato ya no esta disponible para Echeq");
            }

            return echeqNegocio;
        }

        private int AgregarEcheq(EcheqRequestModel request, EcheqNegocioDto echeqNegocio)
        {
            EcheqNegocio negocio = new EcheqNegocio(echeqNegocio);


            MaterialFason material = repositorio.Obtener<MaterialFason>(x => x.CodigoSap == echeqNegocio.MaterialCodigo);

            if (material == null)
            {
                negocio.Material = new MaterialFason { CodigoSap = echeqNegocio.MaterialCodigo, Nombre = echeqNegocio.DescripcionMaterial };
            }
            else
            {
                negocio.MaterialId = material.Id;
            }

            negocio.MarcaCheque = true;
            negocio.FechaCreacion = DateTime.Now;
            negocio.UsuarioCreacionId = request.UsuarioCreacionId;
            negocio.ProveedorId = request.ProveedorId;


            foreach (var liquidacion in negocio.Documentos)
            {
                liquidacion.MarcaCheque = true;
                liquidacion.FechaCreacion = DateTime.Now;
                liquidacion.UsuarioCreacionId = request.UsuarioCreacionId;
            }


            //4- Grabar en la base de datos
            repositorio.Agregar(negocio);
            return repositorio.GuardarCambios();
        }

        private int UpdateEcheq(EcheqRequestModel request, bool marcaCheck)
        {
            EcheqNegocio echeqExistente = repositorio.Obtener<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido);

            if (echeqExistente == null)
            {
                throw new ValidationCustomException("No se encontro el Echeq");
            }

            echeqExistente.FechaModificacion = DateTime.Now;
            echeqExistente.UsuarioModificacionId = request.UsuarioCreacionId;
            echeqExistente.MarcaCheque = marcaCheck;

            foreach (var liquidacion in echeqExistente.Documentos)
            {
                liquidacion.MarcaCheque = marcaCheck;
                liquidacion.FechaModificacion = DateTime.Now;
                liquidacion.UsuarioModificacionId = request.UsuarioCreacionId;
                if (marcaCheck == false)
                {
                    foreach (var apertura in liquidacion.Aperturas.Where(a => a.Estado))
                    {
                        var result = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
                        apertura.Estado = marcaCheck;
                        apertura.UsuarioModificacionId = request.UsuarioCreacionId;
                        apertura.FechaModificacion = DateTime.Now;
                    }
                }
            }

            return repositorio.GuardarCambios();
        }

        private int UpdateLiquidacion(EcheqRequestModel request, bool marcaCheck, EcheqLiquidacion liquidacionExistente)
        {

            liquidacionExistente.FechaModificacion = DateTime.Now;
            liquidacionExistente.UsuarioModificacionId = request.UsuarioCreacionId;
            liquidacionExistente.MarcaCheque = marcaCheck;

            return repositorio.GuardarCambios();

        }

        private EcheqLiquidacion ObtenerLiquidacionPorDocumento(string documento)
        {
            EcheqLiquidacion liquidacionExistente = repositorio.Obtener<EcheqLiquidacion>(x => x.Documento == documento);

            if (liquidacionExistente == null)
            {
                throw new ValidationCustomException("No se encontro la liquidación");
            }
            else
            {

            }

            return liquidacionExistente;
        }

        public string AgregarApertura(EcheqRequestModel request)
        {
            EcheqLiquidacion liquidacion = repositorio.Obtener<EcheqLiquidacion>(x =>
            x.Documento == request.Documento &&
            x.EcheqNegocio.Contrato == request.Contrato &&
            x.EcheqNegocio.Pedido == request.Pedido &&
            x.MarcaCheque);

            if (liquidacion == null)
                throw new ValidationCustomException("No se encontro la liquidacion");

            //ANULAR APETURAS ANTERIORES
            foreach (var apertura in liquidacion.Aperturas)
            {
                apertura.Estado = false;
                apertura.Estado = false;
                apertura.UsuarioModificacionId = request.UsuarioCreacionId;
                apertura.FechaModificacion = DateTime.Now;
                var result = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
            }

            //Agregar Aperturas Nuevas
            foreach (var apertura in request.Apertura)
            {
                var result = echeqCargaAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(),liquidacion.EcheqNegocio.Contrato, liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"),apertura.ImporteCheque,"ARP  ",liquidacion.EcheqNegocio.Pedido,liquidacion.EcheqNegocio.Proveedor.CUIT,"", "MOA", "");
                liquidacion.Aperturas.Add(new EcheqApertura
                {
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    ImporteCheque = apertura.ImporteCheque,
                    OrdenCheque = apertura.OrdenCheque,
                    UsuarioCreacionId = request.UsuarioCreacionId
                });
            }

            repositorio.GuardarCambios();
            return "La apertura se grabo correctamente.";
        }

        public List<ConfiguracionDto> ObtenerConfiguracion()
        {
            var configuracionEcheq = repositorio.Listar<Configuracion>(a => a.Code == "EcheqLimiteCantidadAperturas" || a.Code == "EcheqAforo")
                .Select(x => new ConfiguracionDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Value = x.Value
                }).ToList();

            return configuracionEcheq;
        }
    }

}
