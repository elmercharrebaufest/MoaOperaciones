using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;

namespace SustitucionMOAUtils.Services
{
    public abstract class OrdenDeCargaServiceBase : IOrdenDeCargaServiceBase
    {
        protected readonly IOrdenCargaConsumerMOA ordenCargaConsumer;
        protected readonly IScatoConsumer scatoConsumer;
        protected readonly IScatoRepositorioClient scatoRepositorioClient;
        private readonly IEmailOrdenesCargaServiceBase emailService;


        protected OrdenDeCargaServiceBase(
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IScatoRepositorioClient scatoRepositorioClient,
            IEmailOrdenesCargaServiceBase emailService)
        {
            this.scatoConsumer = scatoConsumer;
            this.ordenCargaConsumer = ordenCargaConsumer;
            this.scatoRepositorioClient = scatoRepositorioClient;
            this.emailService = emailService;
        }

        public ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit)
        {
            var clientes = scatoConsumer.ObtenerClientesPorCuit(cuit);
            return new ValidarCuitExisteScatoResponse
            {
                Existe = clientes.Length > 0,
                RazonSocial = clientes.Length > 0 ? clientes.First().Descripcion : ""
            };
        }

        public bool ValidarSisa(string cuitDestinatario, string cuitDestino, string codigoMaterial)
        {
            if (string.IsNullOrEmpty(cuitDestinatario) && string.IsNullOrEmpty(cuitDestino))
            {
                return false;
            }
            var controlarCargaReq = new ControlCargaRequest
            {
                SoloSisa = true,
                Material = codigoMaterial,
                CuitDestinatario = cuitDestinatario,
                CuitDestino = cuitDestino
            };
            var responseHandler = ordenCargaConsumer.ControlarCarga(controlarCargaReq);
            if (!string.IsNullOrEmpty(cuitDestinatario))
            {
                return !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinatarioInhabilitadoEnSisa);
            }
            else
            {
                return !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinoInhabilitadoEnSisa);
            }
        }

        public List<PlantaDto> ObtenerPlantasDestino(string destinoCuit)
        {
            var plantasRes = scatoRepositorioClient.ObtenerPlantas(destinoCuit);
            if (!plantasRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Scato con CUIT " + destinoCuit);
                foreach (var err in plantasRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                throw new ValidationCustomException("Error al obtener Plantas");
            }
            else
            {
                return plantasRes.Data
                    .Select(x =>
                        new PlantaDto
                        {
                            Actividad = x.Actividad,
                            Codigo = x.NroPlanta
                        })
                    .ToList();
            }
        }

        public List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit)
        {
            var domiciliosRes = scatoRepositorioClient.ObtenerDomicilios(destinoCuit);
            if (!domiciliosRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Domicilios con CUIT " + destinoCuit);
                foreach (var err in domiciliosRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                throw new ValidationCustomException("Error al obtener Domicilios");
            }
            else
            {
                return domiciliosRes.Data
                    .Select(x =>
                        new DomicilioDto
                        {
                            Descripcion = x.Descripcion,
                            Orden = x.Orden,
                            Tipo = x.Tipo
                        })
                    .ToList();
            }
        }

        public bool ValidarCuitRuca(string cuit)
        {
            var tienePlanta = ObtenerPlantasDestino(cuit).Any();
            if (tienePlanta)
            {
                var tieneDomicilio = ObtenerDomiciliosDestino(cuit).Any();
                return tieneDomicilio;
            }
            else
            {
                return false;
            }
        }

        public void EmailGestionarAlta(string cuit, string razonSocial, bool esIntermediarioFlete)
        {
            if (esIntermediarioFlete)
            {
                emailService.EnviarMailAltaIntermediarioFlete(cuit, razonSocial);
            }
            else
            {
                emailService.EnviarMailAltaTempranaCuit(cuit, razonSocial);
            }
        }

        public ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit)
        {
            var scatoRes = scatoRepositorioClient.ObtenerProveedorPorCuil(cuit);
            if (scatoRes.IsValid)
            {
                return new ValidarIntermediarioFleteResponse
                {
                    EsCuitValido = true,
                    ExisteIntermediario = true,
                    RazonSocial = scatoRes.Data.RazonSocial
                };
            }

            var response = new ValidarIntermediarioFleteResponse();
            if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.DigitoVerificadorNoValido))
            {
                response.EsCuitValido = false;
            }
            else
            {
                if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.ProveedorNoEncontrado))
                {
                    response.EsCuitValido = true;
                    response.ExisteIntermediario = false;
                }
                else
                {
                    throw new Exception("Error en ValidarIntermediarioFlete. Validación inesperada con cuit " + cuit);
                }
            }
            return response;
        }
        public (bool, ScatoRepo.Chofer) ValidarCuilChofer(string cuilChofer)
        {
            var choferRes = scatoRepositorioClient.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer));
            var chofer = choferRes.Data;
            if (!choferRes.IsValid)
            {
                Log.Info("Error al obtener chofer de Scato " + cuilChofer);
                foreach (var err in choferRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageCode, err.Message));
                }

                return (choferRes.Messages.All(msg => msg.MessageCode != ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido), chofer);
            }
            return (true, chofer);
        }
        public (bool, ScatoRepo.Chofer) ValidarCuitTransporte(string cuitTransporte)
        {
            var transporteRes = scatoRepositorioClient.ObtenerTransportePorCuit(DataFormatter.CuitConGuion(cuitTransporte));
            var transporte = transporteRes.Data;
            if (!transporteRes.IsValid)
            {
                Log.Info("Error al obtener transporte de Scato " + cuitTransporte);
                foreach (var err in transporteRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageCode, err.Message));
                }

                return (transporteRes.Messages.All(msg => msg.MessageCode != ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido), transporte);
            }
            return (true, transporte);
        }
        public bool ValidarCuilChoferDigito(string cuilChofer)
        {
            return ValidarCuilChofer(cuilChofer).Item1;
        }
        public bool ValidarCuitTransporteDigito(string cuitTransporte)
        {
            return ValidarCuitTransporte(cuitTransporte).Item1;
        }
    }
}
