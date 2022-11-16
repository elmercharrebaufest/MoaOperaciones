using System;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOASecurity;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class OrdenDeCargaFasonController : BaseController
	{
		private readonly IOrdenDeCargaFasonService _ordenDeCargaFasonService;

		public OrdenDeCargaFasonController(IOrdenDeCargaFasonService ordenDeCargaFasonService)
		{
			_ordenDeCargaFasonService = ordenDeCargaFasonService;
		}

		[HttpGet]
		public ActionResult Listar(string fechaInicio, string fechaFin)
		{
			try
			{
				var mailUsuario = SessionPersister.getUsername();
				var request = new ListarOrdenDeCargaFasonRequest()
				{
					MailUsuario = mailUsuario,
					FechaDesde = fechaInicio,
					FechaHasta = fechaFin
				};
				var result = _ordenDeCargaFasonService.Listar(request);
				return JsonCustom(result);
			}
			catch (InfoCustomException ex)
			{
				return Json(new { info = ex.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (ValidationCustomException ex)
			{
				return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}
		}

		[HttpGet]
		public ActionResult GetDetalle(int IdOrdenCargaFason)
		{
			try
			{
				var mailUsuario = SessionPersister.getUsername();
				var request = new DetalleOrdenDeCargaFasonRequest()
				{
					MailUsuario = mailUsuario
				};

				return JsonCustom(new { data = _ordenDeCargaFasonService.ObtenerDetalle(IdOrdenCargaFason, request) });
			}
			catch (InfoCustomException ex)
			{
				return Json(new { info = ex.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (ValidationCustomException ex)
			{
				return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(ex.Message);
			}
		}


		[HttpGet]
		public ActionResult VerificarTransporte(int IdOrdenCargaFason)
		{
			try
			{
				var mailUsuario = SessionPersister.getUsername();
				return JsonCustom(new { data = _ordenDeCargaFasonService.VerificarTransporte(IdOrdenCargaFason) });
			}
			catch (InfoCustomException e)
			{
				return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (ValidationCustomException e)
			{
				return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
				return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
			}
		}
	}
}
