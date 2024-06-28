using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);

                List<EcheqNegocioDto> result = echeqVisualizarPendientePagoConsumerMOA.Request(proveedor, fechas, contrato);

                result.Where(x => x.Clasificacion == "PRODUCTOR" && x.MarcaCheque == true).ToList().ForEach(x => x.Documentos.ForEach(k => k.MarcaCheque = true));
                result.Where(x => x.Clasificacion != "PRODUCTOR" && x.Documentos.Any(e => e.MarcaCheque == true)).ToList().ForEach(k => k.MarcaCheque = true);

                List<string> cttosConCesionPago = this.GetContratosConCesionPago(result, proveedor);
                var resultFiltrado = result.Where(x => !cttosConCesionPago.Contains(x.Contrato)).ToList();

                List<string> documentos = result.Where(a => a.MarcaCheque).SelectMany(a => a.Documentos.Where(b => b.MarcaCheque).Select(b => b.Documento)).ToList();
                var aperturas = repositorio.Listar<EcheqApertura>(x => x.Estado == true &&
                   documentos.Contains(x.EcheqLiquidacion.Documento));

                foreach (var apertura in aperturas)
                {
                    EcheqLiquidacionDto liquidacion = resultFiltrado
                        .Where(a => a.Contrato == apertura.EcheqLiquidacion.EcheqNegocio.Contrato && a.Pedido == apertura.EcheqLiquidacion.EcheqNegocio.Pedido)
                        .SelectMany(a => a.Documentos).Where(a => a.Documento == apertura.EcheqLiquidacion.Documento).SingleOrDefault();
                    liquidacion.Aperturas.Add(new EcheqAperturaDto { OrdenCheque = apertura.OrdenCheque, ImporteCheque = apertura.ImporteCheque });
                }
                return resultFiltrado;
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

        private List<string> GetContratosConCesionPago(List<EcheqNegocioDto> result, string proveedor)
        {
            HashSet<string> cttos = new HashSet<string>(result.Select(c => c.Contrato));
            List<string> cttosConCesionPago = new List<string>();

            foreach (string ctto in cttos)
            {
               if(this.ContratoTieneCesionDePago(proveedor, ctto))
                {
                    cttosConCesionPago.Add(ctto);
                }
            }

            return cttosConCesionPago;
        }

        public bool ContratoTieneCesionDePago(string proveedor, string contrato)
        {
            ContratoDetalleWSMOAResponse response = (ContratoDetalleWSMOAResponse)new ContratoDetalleConsumerMOA().request(proveedor, contrato);
            //Se considera en este caso, como un contrato con cesion de pago.
            if (response == null || response.error == "06")
            {
                return false;
            }

            if (response.error != null && response.error != "" && response.error != "01")
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            return response.condicionesPago.Any(condicion => condicion.Trim().ToUpper().Contains("CESIÓN DE PAGOS"));
        }
        public string MarcarContrato(EcheqRequestModel request)
        {
            try
            {
                //1- Obtener contrato desde la RFC y setear echeq
                EcheqNegocioDto echeqNegocio = this.ObtieneNegocio(request);


                //2- validar si es productor o acopiador u otros
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
                        if (ObtenerNegocio(request)!= null)
                        {
                            this.UpdateEcheq(request, true, echeqNegocio);
                        }
                        else
                        {
                            this.AgregarEcheq(request, echeqNegocio);
                        }
                    }
                }
                else if (echeqNegocio.Clasificacion == "ACOPIADOR" || echeqNegocio.Clasificacion == "OTROS")//3.2- Si es acopiador/otros por cada una de las liquidaciones llamar a la rfc de marcar documento
                {
                    ResultadoGenerico modificarNegocio = new ResultadoGenerico();

                    foreach (var liquidacion in echeqNegocio.Documentos)
                    {
                        modificarNegocio = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, liquidacion.NumeroCOE, "MOA", "", "=");

                        if (modificarNegocio.HayError)
                        {
                            throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                        }
                    }

                    if (ObtenerNegocio(request) != null)
                    {
                        this.UpdateEcheq(request, true, echeqNegocio);
                    }
                    else
                    {
                        this.AgregarEcheq(request, echeqNegocio);
                    }
                }
                else
                {
                    throw new ValidationCustomException("Clasificacíon de contrato no valida");
                }

                return "El contrato se marcó correctamente";
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

                //2- validar si es productor o acopiador u otros
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
                        this.UpdateEcheq(request, false, echeqNegocio);
                    }
                }
                else if (echeqNegocio.Clasificacion == "ACOPIADOR" || echeqNegocio.Clasificacion == "OTROS")//3.2- Si es acopiador/otros por cada una de las liquidaciones llamar a la rfc de marcar documento
                {
                    ResultadoGenerico modificarNegocio = new ResultadoGenerico();

                    this.UpdateEcheq(request, false, echeqNegocio);

                    foreach (var liquidacion in echeqNegocio.Documentos)
                    {
                        modificarNegocio = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, liquidacion.NumeroCOE, "MOA", "", "");

                        if (modificarNegocio.HayError)
                        {
                            throw new ValidationCustomException(string.Join(", ", modificarNegocio.Errores.Select(x => x.Message).ToList()));
                        }
                    }
                }
                else
                {
                    throw new ValidationCustomException("Clasificacíon de contrato no valida");
                }

                return "El contrato se desmarcó correctamente";
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
                EcheqNegocioDto echeqNegocioSAP = this.ObtieneNegocio(request);

                var numeroCOE = echeqNegocioSAP.Documentos.Where(x => x.Documento == request.Documento).FirstOrDefault().NumeroCOE;

                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, request.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, numeroCOE, "MOA", "", "=");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                //1- Obtener contrato desde la RFC y setear echeq

                EcheqNegocio negocioDB = ObtenerNegocio(request);

                if (negocioDB != null)
                {
                    negocioDB.MarcaCheque = true;
                    negocioDB.FechaModificacion = DateTime.Now;
                    negocioDB.UsuarioModificacionId = request.UsuarioCreacionId;

                    var docDB = negocioDB.Documentos.Where(x => x.Documento == request.Documento).SingleOrDefault();

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

                        negocioDB.Documentos.Add(echeqLiquidacion);


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

                return "El documento se marcó correctamente";
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
            string mensajeErrorBloqueo = "bloqueado por";
            string mensajeErrorBloqueoReemplazo = "El Contrato esta siendo tratado, espere un momentos.";
            try
            {
                EcheqLiquidacion liquidacion = repositorio.Obtener<EcheqLiquidacion>(x => x.Documento == request.Documento && x.EcheqNegocio.Contrato == request.Contrato && x.EcheqNegocio.Pedido == request.Pedido && x.MarcaCheque);

                if (liquidacion != null)
                {
                    //ANULAR APERTURAS ANTERIORES
                    foreach (var apertura in liquidacion.Aperturas.Where(ap => ap.Estado))
                    {
                        apertura.Estado = false;
                        apertura.UsuarioModificacionId = request.UsuarioCreacionId;
                        apertura.FechaModificacion = DateTime.Now;

                        if(apertura.OrdenCheque > 0)
                        {
                            var resultadoAnularAperturaCheque = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
                            if (resultadoAnularAperturaCheque.HayError)
                            {
                                string mensaje = string.Join(", ", resultadoAnularAperturaCheque.Errores.Select(x => x.Message.Contains(mensajeErrorBloqueo) ? mensajeErrorBloqueoReemplazo : x.Message).ToList());
                                throw new ValidationCustomException(mensaje);
                            }
                        }

                    }
                }

                EcheqLiquidacion liquidacionExistente = ObtenerLiquidacionPorDocumento(request);
                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, liquidacionExistente.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, liquidacionExistente.NumeroCOE, "MOA", "", "");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }
                liquidacionExistente = ObtenerLiquidacionPorDocumento(request);
                UpdateLiquidacion(request, false, liquidacionExistente);

                EcheqNegocioDto echeqNegocio = ObtieneNegocio(request);

                if (echeqNegocio.Documentos.Where(x => x.MarcaCheque).Count() == 0)
                {
                    DesmarcarContrato(request);
                }

                return "El documento se desmarcó correctamente";
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
            List<EcheqNegocioDto> contratosEcheq = ObtenerPendientePago(request.CodigoProveedor, DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"), request.Contrato);
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
                throw new ValidationCustomException("El contrato ya no está disponible para Echeq");
            }

            return echeqNegocio;
        }

        private EcheqNegocio AgregarEcheq(EcheqRequestModel request, EcheqNegocioDto echeqNegocio)
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
            repositorio.GuardarCambios();
            return negocio;
        }

        private int UpdateEcheq(EcheqRequestModel request, bool marcaCheck, EcheqNegocioDto echeqNegocio)
        {

            EcheqNegocio echeqExistente = ObtenerNegocio(request);

            if (echeqExistente == null)
            {
                echeqExistente = AgregarEcheq(request, echeqNegocio);
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
                        if (apertura.OrdenCheque > 0)
                        {
                            var result = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
                            if (result.HayError)
                                throw new ValidationCustomException(string.Join(", ", result.Errores.Select(x => x.Message).ToList()));
                        }
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

        private EcheqLiquidacion ObtenerLiquidacionPorDocumento(EcheqRequestModel request)
        {
            EcheqLiquidacion liquidacionExistente = repositorio.Obtener<EcheqLiquidacion>(x => x.Documento == request.Documento);

            if (liquidacionExistente == null)
            {
                liquidacionExistente = CrearLiquidacion(request);
            }

            return liquidacionExistente;
        }

        public string AgregarApertura(EcheqRequestModel request)
        {
            string mensajeErrorBloqueo = "bloqueado por";
            string mensajeErrorBloqueoReemplazo = "El Contrato esta siendo tratado, espere un momentos.";
            string mensaje = string.Empty;
            EcheqLiquidacion liquidacion = repositorio.Obtener<EcheqLiquidacion>(x =>
            x.Documento == request.Documento &&
            x.EcheqNegocio.Contrato == request.Contrato &&
            x.EcheqNegocio.Pedido == request.Pedido &&
            x.MarcaCheque);

            if (liquidacion == null)
            {
                liquidacion = CrearLiquidacion(request);
            }

            //ANULAR APERTURAS ANTERIORES
            foreach (var apertura in liquidacion.Aperturas.Where(ap => ap.Estado))
            {
                apertura.Estado = false;
                apertura.UsuarioModificacionId = request.UsuarioCreacionId;
                apertura.FechaModificacion = DateTime.Now;

                if (apertura.OrdenCheque > 0)
                {
                    var resultadoAnularAperturaCheque = echeqAnularAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), "MOA", "");
                    if (resultadoAnularAperturaCheque.HayError)
                    {
                        mensaje = string.Join(", ", resultadoAnularAperturaCheque.Errores.Select(x => x.Message.Contains(mensajeErrorBloqueo)? mensajeErrorBloqueoReemplazo : x.Message).ToList());
                        throw new ValidationCustomException(mensaje);
                        //throw new ValidationCustomException(string.Join(", ", resultadoAnularAperturaCheque.Errores.Select(x => x.Message).ToList()));
                    }
                }
            }

            //Agregar Aperturas Nuevas
            foreach (var apertura in request.Apertura)
            {
                //string cuit = liquidacion.EcheqNegocio.Proveedor != null ? liquidacion.EcheqNegocio.Proveedor.CUIT : 
                //    repositorio.Obtener<Proveedor>(x => x.Id == liquidacion.EcheqNegocio.ProveedorId).CUIT;

                if (apertura.OrdenCheque > 0)
                {
                    var resultadoCargaAperturaCheque = echeqCargaAperturaChequeConsumerMOA.Request(apertura.OrdenCheque.ToString(), liquidacion.EcheqNegocio.Contrato,
                                                                                liquidacion.Documento, liquidacion.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"),
                                                                                apertura.ImporteCheque, "ARP  ", liquidacion.EcheqNegocio.Pedido, request.CodigoProveedor, liquidacion.NumeroCOE, "MOA", "");
                    if (resultadoCargaAperturaCheque.HayError)
                    {
                        mensaje = string.Join(", ", resultadoCargaAperturaCheque.Errores.Select(x => x.Message.Contains(mensajeErrorBloqueo) ? mensajeErrorBloqueoReemplazo : x.Message).ToList());
                        throw new ValidationCustomException(mensaje);
                        //throw new ValidationCustomException(string.Join(", ", resultadoCargaAperturaCheque.Errores.Select(x => x.Message).ToList()));
                    }
                }

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
            mensaje = "La apertura se grabó correctamente.";
            return mensaje;
        }

        private EcheqLiquidacion CrearLiquidacion(EcheqRequestModel request)
        {

            EcheqNegocio echeqNegocioDB = repositorio.Obtener<EcheqNegocio>(x => x.Contrato == request.Contrato);
            EcheqNegocioDto echeqNegocio = ObtieneNegocio(request);

            if (echeqNegocioDB == null)
            {
                echeqNegocioDB = AgregarEcheq(request, echeqNegocio);

                var liquidacion = echeqNegocioDB.Documentos.Where(x => x.Documento == request.Documento).FirstOrDefault();
                return liquidacion;
            }
            else
            {
                //generar nuevo documento y agregar nuevo documento

                var documentoDto = echeqNegocio.Documentos.Where(x => x.Documento == request.Documento).SingleOrDefault();
                var liquidacion = new EcheqLiquidacion(documentoDto);

                liquidacion.MarcaCheque = true;
                liquidacion.FechaCreacion = DateTime.Now;
                liquidacion.UsuarioCreacionId = request.UsuarioCreacionId;
                echeqNegocioDB.Documentos.Add(liquidacion);

                repositorio.GuardarCambios();
                return liquidacion;
            }
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

        public List<EcheqReporteDto> ObtenerDatosReporte(string fechaInicio, string fechaFin, string mailUsuario, string codigoProveedor)
        {
            try
            {
                DateTime fechaIncioDateTime, fechaFinDateTime;
                try
                {
                    fechaIncioDateTime = DateTime.Parse(fechaInicio);
                }
                catch
                {
                    try
                    {
                        fechaInicio = new string(fechaInicio.Where(c => c != '\u200E').ToArray());
                        fechaIncioDateTime = DateTime.Parse(fechaInicio);
                    }
                    catch (Exception e)
                    {
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "inicio"), e);
                    }
                }

                try
                {
                    fechaFinDateTime = DateTime.Parse(fechaFin);
                }
                catch
                {
                    try
                    {
                        fechaFin = new string(fechaFin.Where(c => c != '\u200E').ToArray());
                        fechaFinDateTime = DateTime.Parse(fechaFin);
                    }
                    catch (Exception e)
                    {
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "fin"), e);
                    }
                }


                var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(u => u.Mail == mailUsuario);

                var esAdmin = usuario.TienePermiso(SustitucionMOAModel.Enums.PermisoEnum.VerEcheqAdmin);


                List<EcheqReporteDto> result = repositorio.Listar<EcheqLiquidacion, EcheqReporteDto>(x => new EcheqReporteDto
                {
                    RazonSocial = x.EcheqNegocio.Proveedor.RazonSocial,
                    Mail = x.UsuarioModificacionId!=null? x.UsuarioModificacion.Mail : x.UsuarioCreacion.Mail,
                    CodigoProveedor = x.EcheqNegocio.Proveedor.CodigoProveedor,
                    Contrato = x.EcheqNegocio.Contrato,
                    LiquidacionMarcada = x.MarcaCheque,
                    FechaCreacion = x.FechaCreacion,
                    Liquidacion = x.Documento,
                    NumeroCOE = x.NumeroCOE
                }, x => x.MarcaCheque && (x.EcheqNegocio.Proveedor.CodigoProveedor == codigoProveedor || esAdmin) && fechaIncioDateTime <= DbFunctions.TruncateTime(x.FechaCreacion) && fechaFinDateTime >= DbFunctions.TruncateTime(x.FechaCreacion)); // falta filtrar por fechas


                List<string> documentos = result.Select(b => b.Liquidacion).ToList();
                var aperturas = repositorio.Listar<EcheqApertura>(x => x.Estado && documentos.Contains(x.EcheqLiquidacion.Documento));

                foreach (var apertura in aperturas)
                {
                    EcheqReporteDto liquidacion = result
                        .Where(a => a.Liquidacion == apertura.EcheqLiquidacion.Documento)
                        .SingleOrDefault();
                    liquidacion.MontosEcheqs.Add(apertura.ImporteCheque);
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
        private EcheqNegocio ObtenerNegocio(EcheqRequestModel request)
        {
            var proveedor = repositorio.Obtener<Proveedor>(request.ProveedorId);
            return repositorio.Obtener<EcheqNegocio>(
                x => x.Contrato == request.Contrato &&
                x.Proveedor.CodigoProveedor == proveedor.CodigoProveedor &&
                x.Pedido == request.Pedido);
        }
    }

}
