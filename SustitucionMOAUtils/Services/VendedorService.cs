using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.DataAgroServices;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities = SustitucionMOAModel.Entities;
using Models = SustitucionMOAModel.Models;

namespace SustitucionMOAUtils.Services
{

    public class VendedorService : IVendedorService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;

        public VendedorService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public VendedorDetalleWSMOAResponse GetDatosFiscales(string vendedor, string proveedor)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Proveedor"));
                }

                VendedorDetalleWSMOAResponse response = new VendedorDetalleConsumerMOA().request(vendedor, proveedor);
                if (response == null)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Proveedor", proveedor));
                }

                if (response.error != null && response.error != "" && response.error != "11" && response.error != "00")
                    throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Situación Fiscal", "Proveedor: " + proveedor));

                return response;
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

        public VendedoresWSMOAResponse GetVendedores(string usuariomail, string codigoProveedor, string fechaInicio, string fechaFin)
        {
            if (fechaInicio == "")
            {
                fechaInicio = DateTime.Now.AddDays(-1).ToShortDateString();
            }
            if (fechaFin == "")
            {
                fechaFin = DateTime.Now.ToShortDateString();
            }

            List<Models.FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);

            VendedoresWSMOAResponse response = new VendedoresConsumerMOA().request(codigoProveedor, fechas);

            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == usuariomail);

            if (usuario.EsAdmin())
            {
                var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

                if (proveedor != null)
                {
                    usuariomail = proveedor.Mail;
                }
            }

            var vendedoresAprobados = GetVendedores(usuariomail, v => v.EstadoAprobacion == EstadoAprobacion.Aprobado && !v.CodigoProveedor.Contains("C")).Select(v => new Vendedor() { descVendedor = v.RazonSocial, estado = v.EstadoAprobacionDescripcion, idVendedor = v.CodigoProveedor });

            response.vendedores.AddRange(vendedoresAprobados);

            response.vendedores = response.vendedores.Distinct().ToList();
            return response;
        }

        public VendedorHabilitadoWSMOAResponse GetVendedorStatus(string cuit, string user)
        {
            try
            {
                if (cuit == null || cuit == "")
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT"));
                }

                VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoConsumerMOA().request(cuit, "MOA", user);
                if (response == null)
                {
                    throw new InfoCustomException(InfoMsg.ProveedorSinAlta);
                }

                if (response.status == null || response.status == "" || response.status == "Proveedor inexistente")
                    throw new InfoCustomException(InfoMsg.ProveedorSinAlta);

                return response;
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

        public List<ProveedorDto> GetVendedores(string mailUsuario)
        {
            return GetVendedores(mailUsuario, x => x.EstadoAprobacion == EstadoAprobacion.Aprobado);
        }

        public List<ProveedorDto> GetVendedoresPendientes(string mailUsuario, string codigoProveedor)
        {

            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == mailUsuario);


            if (usuario.EsAdmin())
            {
                var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

                if (proveedor != null)
                {
                    mailUsuario = proveedor.Mail;
                }
            }

            List<ProveedorDto> proveedorDtos = GetVendedores(mailUsuario, x => x.EstadoAprobacion != EstadoAprobacion.Aprobado);

            if (proveedorDtos.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Empresas"));
            }
            return proveedorDtos;
        }

        private List<ProveedorDto> GetVendedores(string mailUsuario, Func<Proveedor, bool> filtro = null)
        {

            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == mailUsuario);

            var listadoProveedores = new List<ProveedorDto>();

            if (usuario.EsAdmin())
            {

                listadoProveedores.AddRange(
                    repositorio
                        .Listar<Proveedor>(p => p.EstadoAprobacion == EstadoAprobacion.Aprobado)
                        .Select(proveedor => new ProveedorDto
                        {
                            CodigoProveedor = proveedor.CodigoProveedor ?? "",
                            CUIT = proveedor.CUIT,
                            EstadoAprobacion = proveedor.EstadoAprobacion,
                            EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                            Id = proveedor.Id,
                            IdComercialDataAgro = proveedor.IdComercialDataAgro,
                            IdDataAgro = proveedor.IdDataAgro,
                            Mail = proveedor.Mail ?? "",
                            Observaciones = proveedor.Observaciones,
                            RazonSocial = proveedor.RazonSocial ?? "",
                            FechaSolicitud = proveedor.FechaSolicitud,
                            Comercial = proveedor.Comercial,
                            EstadoSIPER = proveedor.EstadoSIPER
                        })
                );

                UsuariosWSMOAResponse response = new UsuariosConsumerMOA().request();

                listadoProveedores.AddRange(response.usuarios.Select(x => new ProveedorDto(x)));
            }
            else
            {
                var proveedores = usuario.Proveedores.ToList();

                if (filtro != null)
                {
                    proveedores = proveedores.Where(filtro).ToList();
                }

                listadoProveedores.AddRange(proveedores.Select(proveedor => new ProveedorDto
                {
                    CodigoProveedor = proveedor.CodigoProveedor ?? "",
                    CUIT = proveedor.CUIT,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                    Id = proveedor.Id,
                    IdComercialDataAgro = proveedor.IdComercialDataAgro,
                    IdDataAgro = proveedor.IdDataAgro,
                    Mail = proveedor.Mail ?? "",
                    Observaciones = proveedor.Observaciones,
                    RazonSocial = proveedor.RazonSocial ?? "",
                    FechaSolicitud = proveedor.FechaSolicitud,
                    Comercial = proveedor.Comercial,
                    EstadoSIPER = proveedor.EstadoSIPER
                }
                ).ToList());
            }


            foreach (var item in listadoProveedores.Where(a => a.CUIT == null))
            {
                item.CUIT = "-";
            }
            return listadoProveedores.Distinct().ToList();
        }

        public string AgregarVendedor(string mailUsuario, string cuit)
        {
            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == mailUsuario);

            if (cuit == null || cuit == "")
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT"));
            }

            if (usuario.Proveedores.Where(x => x.CUIT == cuit).Any())
            {
                throw new ValidationCustomException(ErrorMsg.ErrorVendedorRepetido);
            }

            var nuevoVendedor = new Proveedor
            {
                CUIT = cuit,
                Mail = usuario.Mail,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CodigoProveedor = FormatearCodigoProveedor(cuit)
            };

            if (usuario.EsCorredor())
            {
                var infoDA = dataAgroService.ObtenerValidarCUITProveedorGranos(cuit);

                if (infoDA.HayError)
                {
                    throw new ValidationCustomException(infoDA.ListaErrores[0].Message);
                }
                var hist = new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    EstadoAprobacion = nuevoVendedor.EstadoAprobacion,
                    Observacion = "Proveedor habilitado en DataAgro"
                };
                nuevoVendedor.HistorialAprobaciones.Add(hist);

                nuevoVendedor.RazonSocial = infoDA.ProveedorRazonSocial;
                nuevoVendedor.Comercial = string.Concat(infoDA.ComercialNombres, " ", infoDA.ComercialApellido);
                nuevoVendedor.IdComercialDataAgro = infoDA.ComercialId;
                nuevoVendedor.IdDataAgro = infoDA.ProveedorId;
            }
            else
            {
                dataAgroService.ValidarNuevoProveedorMultifirma(ref nuevoVendedor);

                var hist = new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    EstadoAprobacion = nuevoVendedor.EstadoAprobacion,
                    Observacion = nuevoVendedor.Observaciones
                };
                nuevoVendedor.HistorialAprobaciones.Add(hist);
                usuario.Proveedores.Add(nuevoVendedor);
            }


            repositorio.GuardarCambios();

            return SuccessMsg.AltaVendedorOK;
        }

        public string EliminarVendedor(string mailUsuario, int proveedorId)
        {
            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == mailUsuario);

            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            if (proveedor.EstadoAprobacion != EstadoAprobacion.DocumentacionPendiente)
            {
                throw new ValidationCustomException("No se puede elimianr el vendedor debido a que su estado no es \"Documentación pendiente\".");
            }

            if (proveedor.HistorialAprobaciones != null)
            {
                if (proveedor.HistorialAprobaciones.Any())
                {
                    throw new ValidationCustomException("No se puede eliminar el vendedor debido a que ya fue enviada su solicitud.");
                }
            }

            usuario.Proveedores.Remove(proveedor);

            repositorio.Remover<Proveedor>(proveedor.Id);

            repositorio.GuardarCambios();

            return SuccessMsg.VendedorBorradoOK;
        }

        private string FormatearCodigoProveedor(string CUIT)
        {
            return CUIT.Substring(2, 8);
        }
    }
}
