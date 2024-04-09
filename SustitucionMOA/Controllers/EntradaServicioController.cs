using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class EntradaServicioController : BaseController
    {
        private readonly IEntradaServicioService EntradaServicioService;

        public EntradaServicioController(IEntradaServicioService entradaServicio)
        {
            this.EntradaServicioService = entradaServicio;
        }

        //[ValidateInput(false)]
        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
        public async Task<ActionResult> GetByProveedorAsync(EntradaServicioParamsDto parametros)
        {
            try
            {
                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}

                List<EntradaServicioCabeceraDto> result =  await EntradaServicioService.ObtenerEntradasServicioCompleta(parametros);

                return JsonCustom(new { data = result });
            }
            catch (Exception ex)
            {
                return JsonCustom(new { error = ex.Message });
            }
        }

        public ActionResult DeleteById(EntradaServicioParamsDto parametros)
        {
            try
            {
                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}

                string result = EntradaServicioService.BorrarEntradaServicio(parametros);

                return JsonCustom(new { data = result });
            }
            catch (Exception ex)
            {
                return JsonCustom(new { error = ex.Message });
            }
        }


        //public ActionResult Create(EntradaServicioCreateParamsDto parametros)
        //{
        //    try
        //    {
        //        // Este debe combinarse con permisos de usuario.
        //        //if (parametros.vendedor == "" || parametros.vendedor == null)
        //        //{
        //        //    parametros.vendedor = SessionPersister.Proveedor;
        //        //}

        //        string result = EntradaServicioService.CrearEntradaServicio(parametros);

        //        return JsonCustom(new { data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonCustom(new { error = ex.Message });
        //    }
        //}

        
        public async Task<ActionResult> CreateAsync(List<EntradaServicioCreateParamsDto> parametros)
        {
            try
            {
                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}

                List<EntradaServicioCreateRespuestaDto> ret = new List<EntradaServicioCreateRespuestaDto>();
                foreach (EntradaServicioCreateParamsDto parametro in parametros)
                {
                    var result = new EntradaServicioCreateRespuestaDto();
                    result = await EntradaServicioService.CrearEntradaServicio(parametro);
                    ret.Add(result);
                }

                return JsonCustom(new { data = ret });
            }
            catch (Exception ex)
            {
                return JsonCustom(new { error = ex.Message });
            }
        }

    }
}