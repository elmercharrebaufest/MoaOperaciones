using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaService : IAltaEmpresaService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;

        public AltaEmpresaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];

        }

        public List<ProveedorDto> getEmpresas(EstadoAprobacion estado)
        {
            List<ProveedorDto> proveedores = repositorio.Listar<Proveedor, ProveedorDto>(
                x => new ProveedorDto
                {
                    CodigoProveedor = x.CodigoProveedor??"",
                    CUIT = x.CUIT,
                    EstadoAprobacion = x.EstadoAprobacion,
                    EstadoAprobacionDescripcion = x.EstadoAprobacion.ToString(),
                    Id = x.Id,
                    IdComercialDataAgro = x.IdComercialDataAgro,
                    IdDataAgro = x.IdDataAgro,
                    Mail = x.Mail??"",
                    Observaciones = x.Observaciones,
                    RazonSocial = x.RazonSocial??""
                }
                , x => (int)x.EstadoAprobacion == (int)estado);
            if (proveedores.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
            }
            return proveedores;

        }

        public string setEstadoAprobacion(int empresaId, EstadoAprobacion estado, string observacion)
        {
            try
            {
                Proveedor proveedor = repositorio.Obtener<Proveedor>(empresaId);
                if (proveedor == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
                }
                proveedor.EstadoAprobacion = estado;
                proveedor.Observaciones = observacion;

                if (estado.Equals(EstadoAprobacion.Aprobado))
                {

                    UsuarioGranos usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == proveedor.Mail);

                    usuario.Roles.Clear();

                    Rol rolUsuarioGranos = ObtenerRolPorCodigo("GRAN");

                    usuario.Roles.Add(rolUsuarioGranos);
                }

                repositorio.GuardarCambios();

                return String.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
        }

        public EstadoAprobacionDto GetEstadoAprobacion(string mail)
        {
            try
            {
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);

                Proveedor proveedor = usuario.ObtenerProveedorActual();

                EstadoAprobacionDto estadoAprobacionDto = new EstadoAprobacionDto
                {
                    Estado = proveedor.EstadoAprobacion,
                    EstadoDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                    Observaciones = proveedor.Observaciones.IsNullOrWhiteSpace() ? "" : proveedor.Observaciones
                };

                return estadoAprobacionDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
