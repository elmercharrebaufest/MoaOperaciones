using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class OrdenDeCargaController : BaseController
    {
        readonly IOrdenDeCargaService ordenDeCargaService;
        private readonly IFacturaAnticipadaService _facturaAnticipadaService;
        private readonly IConsultaService consultaService;
        private readonly object _lockCreacionOrdenes = new Object();
        public OrdenDeCargaController(IConsultaService consultaService, IOrdenDeCargaService ordenDeCargaService, IFacturaAnticipadaService facturaAnticipadaService)
        {
            this.consultaService = consultaService;
            this.ordenDeCargaService = ordenDeCargaService;
            _facturaAnticipadaService = facturaAnticipadaService;
        }

        [HttpPost]
        public ActionResult Agregar(string crearOrdenDeCargaRequestJson)
        {
            lock (_lockCreacionOrdenes)
            {
                var crearOrdenDeCargaRequest = JsonConvert.DeserializeObject<CrearOrdenDeCargaRequest>(crearOrdenDeCargaRequestJson);
                var mailUsuario = SessionPersister.Mail;
                return JsonCustom(new { data = ordenDeCargaService.Agregar(crearOrdenDeCargaRequest, mailUsuario) });
            }
        }

        [HttpPost]
        public ActionResult Editar(string ordenDeCargaJson, string gestionAltaFASJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
            var gestionAltas = JsonConvert.DeserializeObject<GestionAltasFAS>(gestionAltaFASJson);

            var mailUsuario = SessionPersister.Mail;

            return JsonCustom(new { data = ordenDeCargaService.Editar(ordenDeCarga, mailUsuario, gestionAltas) });
        }

        [HttpGet]
        public ActionResult GetListado(string fechaInicio, string fechaFin)
        {
            var mailUsuario = SessionPersister.Mail;
            var idProveedorSeleccionado = SessionPersister.ProveedorId;
            var ordenes = ordenDeCargaService.Listar(mailUsuario, fechaInicio, fechaFin, idProveedorSeleccionado);
            if (ordenes == null || ordenes.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de cargas"));
            }
            return JsonCustom(ordenes);
        }

        [HttpPost]
        public ActionResult CrearOrdenEnSAP(CrearOrdenEnSAPRequest request)
        {
            var response = ordenDeCargaService.CrearOrdenEnSAP(request, true);
            return JsonCustom(response);
        }
        [HttpPost]
        public ActionResult EnviarOrdenesASAP(System.Collections.Generic.List<int> ordenesIds)
        {
            var mailUsuario = SessionPersister.Mail;

            var data = ordenDeCargaService.EnviarOrdenesASAP(ordenesIds, mailUsuario);
            return JsonCustom(new { data });
        }

        [HttpGet]
        public ActionResult Get(int ordenDeCargaId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.Obtener(mailUsuario, ordenDeCargaId), historial = ordenDeCargaService.ObtenerEditarHistorial(mailUsuario, ordenDeCargaId) });
        }

        [HttpGet]
        public ActionResult GetEditar(int ordenDeCargaId)
        {
            return JsonCustom(new { data = ordenDeCargaService.ObtenerEditar(ordenDeCargaId) });
        }

        [HttpGet]
        public ActionResult AnularOrden(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            var response = ordenDeCargaService.AnularOrden(ordenId, mailUsuario);
            return JsonCustom(new { data = response });
        }

        [HttpGet]
        public ActionResult NotificarTransporte(int ordenId)
        {
            return JsonCustom(new { data = ordenDeCargaService.NotificarTransporte(ordenId) });
        }

        [HttpGet]
        public ActionResult ObtenerContratos(int ordenId)
        {
            return JsonCustom(new { data = ordenDeCargaService.ObtenerContratos(ordenId) });
        }

        [HttpPost]
        public ActionResult SeleccionarContrato(int ordenId, string contratoSAP)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.SeleccionarContrato(ordenId, contratoSAP, mailUsuario) });
        }

        [HttpGet]
        public ActionResult VerificarSituacionCrediticia(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.VerificarSituacionCrediticia(ordenId) });
        }

        [HttpGet]
        public ActionResult VerificarTransporte(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.VerificarTransporte(ordenId, mailUsuario) });
        }


        [HttpGet]
        public ActionResult Materiales()
        {
            return JsonCustom(new { data = consultaService.ObtenerMaterial(TablaSeccionMaterial.OrdenDeCarga) });
        }

        [HttpPost]
        public ActionResult ForzarCreacionOrden(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.ForzarCreacionOrden(ordenId, mailUsuario) });
        }

        public ActionResult ObtenerPatentes(string ordenDeCargaJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
            return Json(ordenDeCargaService.ObtenerPatentes(ordenDeCarga), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult VisualizarCliente(string corredor, string fechaInicio, string fechaFin)
        {
            //var response = ordenDeCargaService.VisualizarClienteProducto(corredor, fechaInicio, fechaFin, pendiente);
            var request = new VisualizarClienteRequest()
            {
                Corredor = corredor,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Pendiente = true
            };
            var response = ordenDeCargaService.VisualizarCliente(request);
            return JsonCustom(response);

        }

        [HttpGet]
        public ActionResult ObtenerProveedor(int idProveedor)
        {
            var response = new SustitucionMOAApiResponse<ProveedorDto>();
            try
            {
                response.Data = ordenDeCargaService.ObtenerProveedor(idProveedor);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult VisualizarProducto(string contrato, string fechaInicio, string fechaFin)
        {
            var request = new VisualizarProductoRequest()
            {
                //ClienteCuit = clienteCuit,
                Contrato = contrato,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Pendiente = true
            };
            var response = ordenDeCargaService.VisualizarProducto(request);
            return JsonCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCorredorClienteContratoProducto(string clienteCuit, string clienteCodigo, string contrato, string corredor, string usuarioEmail, string fechaInicio, string fechaFin, string productoId)
        {
            var request = new ValidarCorredorClienteContratoProductoRequest()
            {
                ClienteCuit = clienteCuit,
                ClienteCodigo = clienteCodigo,
                Contrato = contrato,
                Corredor = corredor,
                UsuarioEmail = usuarioEmail,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                ProductoId = productoId,
                Pendiente = true
            };
            var response = ordenDeCargaService.ValidarCorredorClienteContratoProducto(request);
            return JsonCustom(response);
        }

        [HttpGet]
        public ActionResult AnularOrdenPorVencimiento(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.NotificarVencimientoOrdenCarga(ordenId, mailUsuario) });
        }

        [HttpGet]
        public ActionResult ActivarOC(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaService.ActivarOC(ordenId, mailUsuario) });
        }

        [HttpGet]
        public ActionResult ObtenerContratosDisponibles(string clienteCodigo, string fechaDesde, string fechaHasta, string corredorCodigo)
        {
            var response = new ObtenerContratosDisponiblesResponse();
            try
            {
                var mailUsuario = SessionPersister.Mail;
                var req = new ObtenerContratosDisponiblesRequest
                {
                    ClienteCodigo = clienteCodigo,
                    CorredorCodigo = corredorCodigo ?? "",
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta
                };
                response = ordenDeCargaService.ObtenerContratosDisponibles(req, mailUsuario);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return JsonCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo)
        {
            var response = new SustitucionMOAApiResponse<ValidarSisaCorredorClienteResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarSisaCorredorCliente(corredorCodigo, clienteCodigo);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitExisteScato(string cuit)
        {
            var response = new SustitucionMOAApiResponse<ValidarCuitExisteScatoResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuitExisteScato(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarSisaCuit(string cuit, string campo)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarSisaCuit(cuit, campo);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ObtenerPlantasDestino(string destinoCuit)
        {
            var response = new SustitucionMOAApiResponse<List<PlantaDto>>();
            try
            {
                response.Data = ordenDeCargaService.ObtenerPlantasDestino(destinoCuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ObtenerDomiciliosDestino(string destinoCuit)
        {
            var response = new SustitucionMOAApiResponse<List<DomicilioDto>>();
            try
            {
                response.Data = ordenDeCargaService.ObtenerDomiciliosDestino(destinoCuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitRuca(string cuit)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuitRuca(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarIntermediarioFlete(string cuit)
        {
            var response = new SustitucionMOAApiResponse<ValidarIntermediarioFleteResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarIntermediarioFlete(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuilChofer(string cuilChofer)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuilChoferDigito(cuilChofer);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarChofer(string cuilChofer, string cuitCliente)
        {
            var response = new SustitucionMOAApiResponse<ValidarChoferResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarChofer(cuilChofer, cuitCliente);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitTransporte(string cuitTransporte)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuitTransporteDigito(cuitTransporte);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult FacturasDisponibles(string numeroContrato)
        {
            var response = new SustitucionMOAApiResponse<List<FacturaOrdenCarga>>();
            try
            {
                response.Data = _facturaAnticipadaService.ObtenerFacturasDeContrato(numeroContrato);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult SeleccionarFactura(int ordenId, string facturaSeleccionada)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                ordenDeCargaService.SeleccionarFactura(ordenId, facturaSeleccionada);
                response.Data = true;
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult VerificarCompensacion(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                ordenDeCargaService.VerificarCompensacion(ordenId);
                response.Data = true;
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        public ActionResult ObtenerCuilsChofer(string ordenDeCargaJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
            return Json(new { cuils = ordenDeCargaService.ObtenerCuilsChofer(ordenDeCarga) }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ObtenerCuitsTransporte(string ordenDeCargaJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
            var mailUsuario = SessionPersister.Mail;
            return Json(new { cuits = ordenDeCargaService.ObtenerCuitsTransporte(ordenDeCarga, mailUsuario) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult ValidarOrdenActivaScato(string ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarOrdenActivaScato(ordenId);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult VerificarCuitsTerceros(int ordenId)
        {
            return JsonCustom(new { data = ordenDeCargaService.VerificarCuitsTerceros(ordenId) });
        }

        [HttpGet]
        public ContentResult ValidarCamion(string patenteChasis, string patenteAcoplado)
        {
            var response = new SustitucionMOAApiResponse<ValidarCamionResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCamion(patenteChasis, patenteAcoplado);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Info = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Info = "No se ha podido validar la escalabilidad del camión.";
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarExistenciaPatentes(string patenteChasis, string cuitCliente)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarExistenciaPatente(patenteChasis, cuitCliente);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarClienteSolicitaAnulacion(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarClienteSolicitaAnulacion(ordenId);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarClienteSolicitaEdicion(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarClienteSolicitaEdicion(ordenId);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
    }
}
