using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaNoGranosService : IAltaEmpresaNoGranosService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        private readonly string DataAgroURL;
        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EstadoAlta.html");


        public AltaEmpresaNoGranosService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }
        public Resultado GrabarNuevoProveedorNoGranos(string razonSocial, string cuit, string email, string telefono, bool realizarAnalisisNOSIS, int IdRubro, string CondicionDePago
            , string ServicioPrestado, string OrganizacionDeCompra, string RazonDeEleccion, int FacturacionAnual, string SolicitanteInterno, string usuarioMail,
            int? idProveedor, string observacionesParaElProveedor, bool requiereVerificacionCompras, bool ingresoAPlanta, bool altaInterna, bool siperObligatorio, string observacionInterna)
        {
            if (repositorio.Existe<Proveedor>(x => x.CUIT == cuit && x.Id != idProveedor))
            {
                throw new ValidationCustomException("El CUIT ya esta registrado.");
            }
            if (repositorio.Existe<Proveedor>(x => x.Mail == email && x.CUIT == cuit && x.Id != idProveedor))
            {
                throw new ValidationCustomException("El Email ya tiene registrado esta CUIT.");
            }
            if (dataAgroService.ProveedorApocrifo(cuit))
            {
                throw new ValidationCustomException("El CUIT esta en la lista de Proveedores Apocrifos.");
            }
            Proveedor proveedor = new Proveedor();
            if (idProveedor.HasValue)
            {
                proveedor = repositorio.Obtener<Proveedor>(idProveedor);
            }
            proveedor.CUIT = cuit;
            proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
            proveedor.RazonSocial = razonSocial;
            proveedor.Mail = email;
            proveedor.Telefono = telefono;
            proveedor.RealizarAnalisisNOSIS = realizarAnalisisNOSIS;
            proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;
            proveedor.IdRubro = IdRubro;
            proveedor.CondicionDePago = CondicionDePago;
            proveedor.ServicioPrestado = ServicioPrestado;
            proveedor.OrganizacionDeCompra = OrganizacionDeCompra;
            proveedor.RazonDeEleccion = RazonDeEleccion;
            proveedor.FacturacionAnual = FacturacionAnual;
            proveedor.SolicitanteInterno = usuarioMail;
            proveedor.ContieneDocumentacionFisica = true;
            proveedor.RequiereVerificacionCompras = requiereVerificacionCompras;
            proveedor.IngresoAPlanta = ingresoAPlanta;
            proveedor.AltaInterna = altaInterna;
            proveedor.TipoProveedor = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "NG");
            proveedor.SiperObligatorio = siperObligatorio;
            proveedor.FechaSolicitud = DateTime.Now;


            int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

            if (proveedor.HistorialAprobaciones == null)
                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();

            proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = proveedor.EstadoAprobacion,
                        Observacion = observacionInterna,
                        Usuario_Id = usuarioId
                    }
                );
            if (!idProveedor.HasValue)
            {
                repositorio.Agregar(proveedor);
            }

            var usuarioYaregistrado = repositorio.Obtener<Usuario>(u => u.Mail == email);
            if (usuarioYaregistrado != null)
            {
                if (!usuarioYaregistrado.TieneRol(RolEnum.Multifirma))
                {
                    usuarioYaregistrado.AgregarRol(repositorio.Obtener<Rol>(r => r.Codigo == "MF"));
                }
                usuarioYaregistrado.Proveedores.Add(proveedor);
            }

            repositorio.GuardarCambios();
            EnviarMailAltaNoGranos(proveedor, observacionesParaElProveedor, null, "Molinos Agro - Alta Iniciada", "iniciada");

            var resultado = new Resultado
            {
                Mensaje = SuccessMsg.AltaVendedorOK,
                IdEntidad = proveedor.Id,
            };

            return resultado;

        }


        private static void EnviarMailAltaNoGranos(Proveedor proveedor, string observacionParaElProveedor, List<string> copia, string asunto, string estado)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, estado, !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");

                EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }

        }

        public List<RubroDto> GetRubros()
        {
            var rubros = repositorio.Listar<Rubro>().Select(a => new RubroDto { Id = a.Id, Nombre = a.Nombre }).ToList();
            return rubros;
        }

        public string HabilitarProveedorOperando(int proveedorId, string razonSocial)
        {
            if (proveedorId <= 0) throw new InfoCustomException("Id invalido.");

            Proveedor proveedor = repositorio.Obtener<Proveedor>(p => p.Id == proveedorId);
            if (proveedor == null) throw new InfoCustomException("No se encontro proveedor con ese Id.");
            if (proveedor.TipoProveedor.NombreCorto != "NG") throw new InfoCustomException("El proveedor no es No Granos.");

            Usuario usuario = proveedor.UsuariosAsociados.First();
            if (usuario == null) throw new InfoCustomException("El usuario no existe.");

            //actualizo los datos del proveedor
            proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;
            proveedor.RazonSocial = razonSocial;
            proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
            proveedor.Observaciones = "";

            //Le saco el Rol de Nuevo usuario NG al usuario.
            usuario.RemoverRol("NUENOGRAN");
            usuario.RemoverRol("DES");

            //Busco el Rol de NG y se lo agrego al usuario
            Rol rolNg = repositorio.Obtener<Rol>(r => r.Codigo == "NOGRAN");
            usuario.AgregarRol(rolNg);

            repositorio.GuardarCambios();

            return "Proveedor Habilitado.";
        }

        public string RechazarProveedorNoGranos(int idProveedor, string usuarioMail, string observacionesParaElProveedor)
        {

            Proveedor proveedor = repositorio.Obtener<Proveedor>(x => x.Id == idProveedor);
            proveedor.Observaciones = "Rechazada";
            proveedor.EstadoAprobacion = EstadoAprobacion.Rechazado;
            int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);
            proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = EstadoAprobacion.Rechazado,
                        Observacion = "Rechazada",
                        Usuario_Id = usuarioId
                    }
                );
            var usuario = proveedor.UsuariosAsociados.FirstOrDefault();
            if (usuario != null)
            {
                usuario.Habilitado = false;
            }
            repositorio.GuardarCambios();
            EnviarMailAltaNoGranos(proveedor, observacionesParaElProveedor, null, "Molinos Agro - Alta Rechazada", "Rechazada");

            return string.Format(SuccessMsg.UsuarioDeshabilitadoOK, proveedor.CUIT);
        }

        public InfoProveedorNoGranosDto ObtenerInfoProveedorNoGranos(string mailUsuario, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Proveedor proveedor;

            if (proveedorId > 0)
                proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            else
                proveedor = usuario.ObtenerProveedor();


            var info = new InfoProveedorNoGranosDto
            {
                ProveedorCUIT = proveedor.CUIT,
                RazonSocial = proveedor.RazonSocial,
                IngresoAPlanta = proveedor.IngresoAPlanta ?? false,
                SiperObligatorio = proveedor.SiperObligatorio ?? false,
                DeclaracionVinculosObligatorio = proveedor.AltaInterna ?? false
            };

            return info;
        }

        public string EditarAltaEmpresaNoGranos(
            int proveedorId,
            string razonSocial,
            string cuit,
            string email,
            string telefono,
            bool realizarAnalisisNOSIS,
            int? IdRubro,
            string CondicionDePago,
            string ServicioPrestado,
            string OrganizacionDeCompra,
            string RazonDeEleccion,
            int? FacturacionAnual,
            bool requiereVerificacionCompras,
            bool ingresoAPlanta,
            bool altaInterna,
            bool siperObligatorio)
        {
            if (proveedorId <= 0)
            {
                throw new InfoCustomException("Id inválido.");
            }

            var proveedor = repositorio.Obtener<Proveedor>(p => p.Id == proveedorId) ??
                throw new InfoCustomException("No se encontró proveedor con este Id.");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);

            proveedor.RazonSocial = razonSocial;
            proveedor.CUIT = cuit;
            proveedor.Telefono = telefono;
            proveedor.RealizarAnalisisNOSIS = realizarAnalisisNOSIS;
            proveedor.IdRubro = IdRubro;
            proveedor.CondicionDePago = CondicionDePago;
            proveedor.ServicioPrestado = ServicioPrestado;
            proveedor.OrganizacionDeCompra = OrganizacionDeCompra;
            proveedor.RazonDeEleccion = RazonDeEleccion;
            proveedor.FacturacionAnual = FacturacionAnual;
            proveedor.RequiereVerificacionCompras = requiereVerificacionCompras;
            proveedor.IngresoAPlanta = ingresoAPlanta;
            proveedor.AltaInterna = altaInterna;
            proveedor.SiperObligatorio = siperObligatorio;

            string mensajeResultado;
            if (usuario == null)
            {
                proveedor.Mail = email;
                mensajeResultado = "Editado correctamente";
            }
            else
            {
                mensajeResultado = "No se puede modificar el Mail porque ya existe un Usuario vinculado.";
            }
            repositorio.GuardarCambios();
            return mensajeResultado;
        }

        public string GetRazonSocial(string CUIT)
        {
            try
            {
                var urlTangoCuit = string.Concat("https://afip.tangofactura.com/Rest/GetContribuyenteFull?cuit=", CUIT);

                string razonSocial = "";

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(urlTangoCuit);

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                    // List data response.
                    HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response body.
                        var dataObjects = response.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                        dynamic data = JObject.Parse(dataObjects);
                        Console.WriteLine(data.Contribuyente.nombre);
                        razonSocial = data.Contribuyente.nombre;
                    }
                    else
                    {
                        throw new InfoCustomException("No se pudo obtener la razón social.");
                    }

                    return razonSocial;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
        private static string FormatearCodigoProveedor(string CUIT)
        {
            return string.Concat("00", CUIT.Substring(2, 8));
        }

        public byte[] DescargarFormularioNG(ProveedorAltaDto proveedorDto)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/FormularioAltaNoGranos/Generar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                string downloadKey = "";

                var content = JsonConvert.SerializeObject(proveedorDto); //myDetails is my class object.
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);

                    task.Wait();

                    var response = task.Result;

                    var stringContent = response.Content.ReadAsStringAsync();

                    dynamic jsonResult = JObject.Parse(stringContent.Result);

                    if (bool.Parse(jsonResult.HayErrores.ToString()))
                    {
                        throw new InfoCustomException(jsonResult.Errores[0].Message);
                    }

                    downloadKey = jsonResult.DownloadKey;


                    urlReporte = string.Concat(urlReporte, "?key=", downloadKey);

                    using (WebClient clienteDescarga = new WebClient())
                    {
                        clienteDescarga.Credentials = new NetworkCredential(userName, password, dominio);
                        byte[] formularioAltaNoGranos = clienteDescarga.DownloadData(urlReporte);
                        return formularioAltaNoGranos;
                    }

                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public bool RegistrarDocumentacionFisica(int proveedorId, bool contieneDocumentacionFisica, string mailUsuarioAlta)
        {
            if (proveedorId <= 0)
            {
                throw new InfoCustomException("Id invalido.");
            }

            var proveedor = repositorio.Obtener<Proveedor>(p => p.Id == proveedorId);

            if (proveedor == null)
            {
                throw new InfoCustomException("No se encontro proveedor con este Id.");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioAlta);

            if (usuario == null)
            {
                throw new InfoCustomException("No se encontro el usuario registrado");
            }

            try
            {
                proveedor.ContieneDocumentacionFisica = contieneDocumentacionFisica;

                //guardo en el historial el cambio realizado
                proveedor.HistorialAprobaciones.Add(
                new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    Observacion = string.Format("Documentación física: {0}", contieneDocumentacionFisica ? "Presentada" : "Faltante"),
                    Proveedor_Id = proveedorId,
                    Usuario_Id = usuario.Id
                });

                repositorio.GuardarCambios();
                return true;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
