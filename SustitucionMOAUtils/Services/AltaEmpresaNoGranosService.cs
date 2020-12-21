using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using SustitucionMOAUtils.Email;

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
        public string GrabarNuevoProveedorNoGranos(string razonSocial, string cuit, string email, string telefono, bool realizarAnalisisNOSIS, int IdRubro, string CondicionDePago
            , string ServicioPrestado, string OrganizacionDeCompra, string RazonDeEleccion, int FacturacionAnual, string SolicitanteInterno, string usuarioMail,
            int? idProveedor, string observacionesParaElProveedor,bool requiereVerificacionCompras, bool ingresoAPlanta, bool altaInterna)
        {
            if (repositorio.Existe<Proveedor>(x => x.CUIT == cuit && x.Id != idProveedor))
            {
                throw new ValidationCustomException("El CUIT ya esta registrado.");
            }
            if (repositorio.Existe<Proveedor>(x => x.Mail == email && x.Id != idProveedor))
            {
                throw new ValidationCustomException("El Email ya esta registrado.");
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
            proveedor.SolicitanteInterno = SolicitanteInterno;
            proveedor.RequiereVerificacionCompras = requiereVerificacionCompras;
            proveedor.IngresoAPlanta = ingresoAPlanta;
            proveedor.AltaInterna = altaInterna;
            proveedor.TipoProveedor = repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == "NG");


            int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);

            if (proveedor.HistorialAprobaciones == null)
                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();

            proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                        Observacion = "",
                        Usuario_Id = usuarioId
                    }
                );
            if (!idProveedor.HasValue)
            {
                repositorio.Agregar(proveedor);
            }
            
            repositorio.GuardarCambios();
            EnviarMailAltaNoGranos(proveedor, observacionesParaElProveedor, null, "Molinos Agro - Alta Iniciada", "iniciada");
            return SuccessMsg.AltaVendedorOK;
        }


        private void EnviarMailAltaNoGranos(Proveedor proveedor, string observacionParaElProveedor, List<string> copia, string asunto, string estado)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, estado, !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");

                EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception e)
            {

            }

        }

        public List<RubroDto> GetRubros()
        {
            var rubros = repositorio.Listar<Rubro>().Select(a => new RubroDto { Id = a.Id, Nombre = a.Nombre }).ToList();
            return rubros;
        }

        public string RechazarProveedorNoGranos(int idProveedor, string usuarioMail,string observacionesParaElProveedor)
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

    }
}
