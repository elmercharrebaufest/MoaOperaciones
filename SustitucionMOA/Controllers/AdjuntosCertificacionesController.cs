using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SustitucionMOAUtils.Interfaces;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Logger;
using SustitucionMOASecurity;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.IO;
using SustitucionMOAModel.Entities;
using System.IO.Compression;
using Newtonsoft.Json;

namespace SustitucionMOA.Controllers
{
    public class AdjuntosCertificacionesController : BaseController
    {
        private readonly IAdjuntosCertificacionesService _adjuntosCertificacionesService;

        public AdjuntosCertificacionesController(IAdjuntosCertificacionesService adjuntosCertificacionesService)
        {
            _adjuntosCertificacionesService = adjuntosCertificacionesService;
        }

        public async Task<ActionResult> Adjuntar()
        {
                var result = await _adjuntosCertificacionesService.AdjuntarAsync(Request.Files, "");

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            
        }


        public async Task<ActionResult> GetAdjuntos(string idES)
        {
            
                var result = await _adjuntosCertificacionesService.GetAdjuntos(idES);

                return JsonCustom(new { data = result });
           
        }
    }
}
