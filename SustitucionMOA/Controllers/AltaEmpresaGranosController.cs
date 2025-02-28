using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class AltaEmpresaGranosController : BaseController
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        readonly IAltaEmpresaGranosService altaEmpresaService;

        public AltaEmpresaGranosController(IAltaEmpresaGranosService altaEmpresaService, IDataAgroService dataAgroService, IRepositorio repositorio)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.dataAgroService = dataAgroService;
            this.repositorio = repositorio;
        }

        public ActionResult GenerarInformeComercial(string informeComercialJson, string mailUsuario, int proveedorId)
        {
            informeComercialJson = informeComercialJson.Replace("nia", "ña");
            var informeComercial = JsonConvert.DeserializeObject<ParamInformeComercial>(informeComercialJson);

            mailUsuario = SessionPersister.getUsername();

            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            var infoProveedor = altaEmpresaService.ObtenerInfoProveedor(mailUsuario, proveedorId);

            if (infoProveedor.ProveedorClasificacion == "Productor")
            {
                if (!informeComercial.NuevosCampos.Any())
                {
                    throw new ValidationCustomException("Para generar el informe comercial debe informar los campos");
                }
            }

            informeComercial.ContactoComercial.Email1 = proveedor.Mail;

            //Para los proveedores que hicieron el alta con los flujos, tenemos el IDDataAgro y IDComercial. Para los migrados no. Por esto, lo vamos a buscar
            if (proveedor.IdDataAgro == null)
            {
                informeComercial.ProveedorId = (int)proveedor.IdDataAgro;
                informeComercial.ComercialID = (int)proveedor.IdComercialDataAgro;
            }
            else
            {
                var proveedorDAO = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                informeComercial.ProveedorId = (int)proveedorDAO.ProveedorId;
                informeComercial.ComercialID = proveedorDAO.ComercialId;
            }

            informeComercial.InformeComercialId = 0;

            if (informeComercial.NuevosCampos != null)
            {
                foreach (var nuevosCampos in informeComercial.NuevosCampos.GroupBy(x => x.MaterialId))
                {
                    informeComercial.Materiales.Add(new ParamInformeComercialMaterial
                    {
                        MaterialId = nuevosCampos.First().MaterialId,
                        Toneladas = nuevosCampos.Sum(x => x.Toneladas)
                    });
                }
            }

            var FileArray = altaEmpresaService.GenerarInformeComercial(informeComercial, mailUsuario, proveedorId);

            //return File(FileArray, "application/pdf", "Informe Comercial.pdf");
            PDFResponse result = new PDFResponse
            {
                Pdf = new Pdf()
                {
                    data = FileArray
                }
            };

            return JsonCustom(result.Pdf);
        }

        public ActionResult GrabarNuevoProveedorGranos(string cuit, string mailVendedor)
        {
            var userMail = SessionPersister.getUsername();

            return JsonCustom(new
            {
                data = altaEmpresaService.GrabarProveedorAltaInternaGranos(cuit, userMail, mailVendedor)
            });
        }


        public ActionResult GenerarCartaPresentacion(string cartaPresentacionJson, int proveedorId)
        {
            string mailUsuario = SessionPersister.getUsername();
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            if (proveedorId == 0)
            {
                proveedorId = usuario.ObtenerProveedor().Id;
            }

            var corredor = usuario.ObtenerCorredor();

            cartaPresentacionJson = cartaPresentacionJson.Replace("nia", "ña");
            var cartaPresentacion = JsonConvert.DeserializeObject<RptCartaDePresentacionInfo>(cartaPresentacionJson);

            cartaPresentacion.corredorCuit = corredor.CUIT;
            cartaPresentacion.corredorRazonSocial = corredor.RazonSocial;

            if (cartaPresentacion.vendedorActividad == "Productor")
            {
                if (!cartaPresentacion.NuevosCampos.Any())
                {
                    throw new ValidationCustomException("Para generar la carta de presentacion debe informar los campos");
                }
            }

            if (cartaPresentacion.vendedorActividad == "Productor")
            {
                if (!cartaPresentacion.NuevosCampos.Any())
                {
                    throw new ValidationCustomException("Para generar la carta de presentacion debe informar los almacenamientos");
                }
            }

            var FileArray = altaEmpresaService.GenerarCartaDePresentacion(cartaPresentacion, mailUsuario, proveedorId);

            //return File(FileArray, "application/pdf", "Informe Comercial.pdf");
            PDFResponse result = new PDFResponse
            {
                Pdf = new Pdf()
                {
                    data = FileArray
                }
            };

            return JsonCustom(result.Pdf);
        }

        public ActionResult GetLocalidadCombo(string localidad)
        {
            localidad = localidad.IsNullOrWhiteSpace() ? "" : localidad;

            if (localidad.Length > 2)
            {
                var listadoLocalidad = repositorio.Listar<Localidad, LocalidadCombo>(x => new LocalidadCombo()
                {
                    LocalidadId = x.LocalidadId,
                    Nombre = x.Nombre + " (" + x.Provincia.Nombre + ")",
                },
                 x => x.Nombre.Contains(localidad)
                 , 500).OrderBy(x => x.Nombre).ToList();

                return JsonCustom(listadoLocalidad);
            }

            return JsonCustom("");
        }

        public ActionResult GetLocalidad(int localidadId)
        {
            return JsonCustom(altaEmpresaService.GetLocalidad(localidadId));
        }

        public async Task<ActionResult> GetMateriales()
        {
            return JsonCustom(await altaEmpresaService.ObtenerMaterialesDataAgro());
        }
        public async Task<ActionResult> GetCampanias()
        {
            string CampanaMin = ConfigurationManager.AppSettings["CampanaMin"].ToString();
            var datosjson = await altaEmpresaService.ObtenerCampañasDataAgroAsync();
            var listCamp = JsonConvert.DeserializeObject<List<CampaniaDto>>(datosjson);

            var idCamp = listCamp.Where(a => a.Descripcion == CampanaMin).Single().CampaniaId;
            listCamp = listCamp.Where(a => a.CampaniaId >= idCamp).ToList();
            return JsonCustom(listCamp);
        }

        [HttpPost]
        public ActionResult GuardarArchivo()
        {
            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);

            var fileKey = Request.Form.Get("fileKey");

            var proveedorId = int.Parse(Request.Form.Get("proveedorId"));

            if (proveedorId == 0)//para el caso de los directo que no tienen mas de un proveedor
            {
                proveedorId = usuario.ObtenerProveedor().Id;
            }

            List<string> errores = new List<string>();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var fileSubido = Request.Files[i];

                if (fileSubido.ContentLength > 0)
                {
                    var result = altaEmpresaService.GuardarArchivo(fileSubido, fileKey, mail, proveedorId);

                    if (!result.Equals(SuccessMsg.ArchivoSubidoOK))
                    {
                        errores.Add(string.Concat("Ocurrió un error con el archivo ", fileSubido.FileName, ": ", result));
                    }
                }
                else
                {
                    errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
                }
            }

            if (errores.Count > 0)
            {
                return JsonCustom(new { info = errores });
            }

            return JsonCustom(new { data = SuccessMsg.ArchivoSubidoOK });
        }

        public ActionResult ObtenerArchivosSubidos(string mail, int proveedorId)
        {
            var esOperador = false;

            //Si viene con un mail, significa que está siendo consultado por un operador, por lo tanto paso todos los archivos
            if (string.IsNullOrEmpty(mail))
            {
                mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            }
            else
            {
                esOperador = true;
            }
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            return JsonCustom(altaEmpresaService.ObtenerArchivosSubidos(mail, proveedorId, esOperador));

        }

        public ActionResult ObtenerInfoProveedor(string mail, int proveedorId)
        {
            if (string.IsNullOrEmpty(mail))
                mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            return JsonCustom(altaEmpresaService.ObtenerInfoProveedor(mail, proveedorId));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult DescargarArchivo(string mail, int archivoID, int proveedorId)
        {
            if (string.IsNullOrWhiteSpace(mail))
                mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            string rutaArchivoSubido = altaEmpresaService.ObtenerArchivo(mail, archivoID, proveedorId);

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
            string fileName = Path.GetFileName(rutaArchivoSubido);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        public ActionResult DescargarArchivos(string mail, int proveedorId)
        {
            if (string.IsNullOrWhiteSpace(mail))
                mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }

            var path = $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);

            string rutaZip = altaEmpresaService.ObtenerArchivos(mail, proveedorId, path);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        public ActionResult EliminarArchivo(int archivoID, int proveedorId)
        {
            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            string result = altaEmpresaService.EliminarArchivo(mail, archivoID, proveedorId);

            return JsonCustom(result);
        }

        [HttpPost]
        public ActionResult EnviarSolicitudUsuario(int proveedorId, string datosJson)
        {
            var altaEmpresa = JsonConvert.DeserializeObject<AltaEmpresaViewModel>(datosJson);

            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

            return JsonCustom(altaEmpresaService.EnviarSolicitudUsuario(mail, proveedorId, false, altaEmpresa));
        }


        public ActionResult CargarSolicitudUsuario(string mail, int proveedorId)
        {
            if (string.IsNullOrEmpty(mail))
                mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            if (proveedorId == 0)
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                proveedorId = usuario.ObtenerProveedor().Id;
            }
            AltaEmpresaViewModel result = altaEmpresaService.CargarSolicitudUsuario(mail, proveedorId);
            return JsonCustom(result);
        }

        [HttpPost]
        public ActionResult SolicitudAltaInterna(int proveedorId, string datosJson)
        {
            string mail = SessionPersister.getUsername();
            var altaEmpresa = JsonConvert.DeserializeObject<AltaEmpresaViewModel>(datosJson);

            return JsonCustom(altaEmpresaService.SolicitudAltaInterna(mail, proveedorId, altaEmpresa));
        }
    }
}
