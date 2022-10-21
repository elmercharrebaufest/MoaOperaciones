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
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class EcheqService : IEcheqService
    {
        private readonly IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA;
        private readonly IEcheqModificarContratoConsumerMOA echeqModificarContratoConsumerMOA;
        private readonly IEcheqModificarFijacionConsumerMOA echeqModificarFijacionConsumerMOA;
        private readonly IEcheqModificacionDocumentoChequeConsumerMOA echeqModificacionDocumentoChequeConsumerMOA;
        private readonly IRepositorio repositorio;

        public EcheqService(IEcheqVisualizarPendientePagoConsumerMOA echeqVisualizarPendientePagoConsumerMOA,
                            IEcheqModificarContratoConsumerMOA echeqModificarContratoConsumerMOA,
                            IEcheqModificarFijacionConsumerMOA echeqModificarFijacionConsumerMOA,
                            IEcheqModificacionDocumentoChequeConsumerMOA echeqModificacionDocumentoChequeConsumerMOA,
                            IRepositorio repositorio)
        {
            this.echeqVisualizarPendientePagoConsumerMOA = echeqVisualizarPendientePagoConsumerMOA;
            this.echeqModificarContratoConsumerMOA = echeqModificarContratoConsumerMOA;
            this.echeqModificarFijacionConsumerMOA = echeqModificarFijacionConsumerMOA;
            this.echeqModificacionDocumentoChequeConsumerMOA = echeqModificacionDocumentoChequeConsumerMOA;
            this.repositorio = repositorio;

        }

        public List<EcheqNegocioDto> ObtenerPendientePago(string proveedor, string fechaInicio, string fechaFin, string contrato)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);

                List<EcheqNegocioDto> result = echeqVisualizarPendientePagoConsumerMOA.Request(proveedor, fechas, contrato);

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
                EcheqNegocioDto echeqNegocio = this.SetTipoNegocio(request);


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
                        if(repositorio.Existe<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido))
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

        public ResultadoGenerico DesmarcarContrato(EcheqRequestModel request)
        {
            try
            {
                ResultadoGenerico result = echeqModificarContratoConsumerMOA.Request(request.Contrato, "", "");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                if (!repositorio.Existe<EcheqNegocio>(x => x.Contrato == request.Contrato && x.ProveedorId == request.ProveedorId && x.Pedido == request.Pedido)) 
                {
                    throw new ValidationCustomException("El contrato no se encuentra");
                }

                this.UpdateEcheq(request, false);            

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

        public ResultadoGenerico MarcarDocumento(EcheqRequestModel request)
        {
            try
            {
                //1- Obtener contrato desde la RFC y setear echeq
                EcheqNegocioDto echeqNegocio = this.SetTipoNegocio(request);
                bool checkLiquidacion = true;
                AgregarEcheq(request, echeqNegocio, checkLiquidacion);

                EcheqLiquidacion liquidacionExistente = this.ObtenerLiquidacionPorDocumento(request.Documento);

                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, liquidacionExistente.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", request.UsuarioCreacionId.ToString(), "=");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                this.UpdateLiquidacion(request, true, liquidacionExistente);

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

        public ResultadoGenerico DesmarcarDocumento(EcheqRequestModel request)
        {
            try
            {

                EcheqLiquidacion liquidacionExistente = ObtenerLiquidacionPorDocumento(request.Documento);

                ResultadoGenerico result = echeqModificacionDocumentoChequeConsumerMOA.Request(request.Contrato, request.Documento, liquidacionExistente.Ejercicio, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"), request.Pedido, request.CodigoProveedor, "", "MOA", request.UsuarioCreacionId.ToString(), "");

                if (result.HayError)
                {
                    throw new ValidationCustomException(result.Errores[0].Message);
                }

                UpdateLiquidacion(request, false, liquidacionExistente);

                EcheqNegocioDto echeqNegocio = SetTipoNegocio(request);

                if (echeqNegocio.Documentos.Where(x => x.MarcaCheque).Count() == 0)
                {
                    DesmarcarContrato(request);
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

        private EcheqNegocioDto SetTipoNegocio(EcheqRequestModel request)
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

        private int AgregarEcheq(EcheqRequestModel request, EcheqNegocioDto echeqNegocio, bool checkLiquidacion = false)
        {
            EcheqNegocio negocio = new EcheqNegocio(echeqNegocio);

            MaterialFason material = repositorio.Obtener<MaterialFason>(x => x.CodigoSap == echeqNegocio.MaterialId.ToString());

            if (material == null)
            {
                throw new ValidationCustomException("El material es una poronga");
            }

            negocio.MaterialId = material.Id;
            negocio.MarcaCheque = true;
            negocio.FechaCreacion = DateTime.Now;
            negocio.UsuarioCreacionId = request.UsuarioCreacionId;
            negocio.ProveedorId = request.ProveedorId;

            if (checkLiquidacion)
            {
                foreach (var liquidacion in negocio.Documentos)
                {
                    liquidacion.MarcaCheque = liquidacion.MarcaCheque;
                    liquidacion.FechaCreacion = DateTime.Now;
                    liquidacion.UsuarioCreacionId = request.UsuarioCreacionId;
                }
            }
            else
            {
                foreach (var liquidacion in negocio.Documentos)
                {
                    liquidacion.MarcaCheque = true;
                    liquidacion.FechaCreacion = DateTime.Now;
                    liquidacion.UsuarioCreacionId = request.UsuarioCreacionId;
                }
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

            if (echeqExistente.Clasificacion == "ACOPIADOR")
            {
                foreach (var liquidacion in echeqExistente.Documentos)
                {
                    liquidacion.FechaModificacion = DateTime.Now;
                    liquidacion.UsuarioModificacionId = request.UsuarioCreacionId;
                }
            }
            else 
            { 
                foreach (var liquidacion in echeqExistente.Documentos)
                {
                    liquidacion.MarcaCheque = marcaCheck;
                    liquidacion.FechaModificacion = DateTime.Now;
                    liquidacion.UsuarioModificacionId = request.UsuarioCreacionId;
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
    }

}
