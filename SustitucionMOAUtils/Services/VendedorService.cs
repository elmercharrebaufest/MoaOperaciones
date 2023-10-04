using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
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
        protected readonly IVendedorHabilitadoConsumerMOA vendedorHabilitadoConsumer;
        private readonly IVendedoresConsumerMOA vendedoresConsumerMOA;

        public VendedorService(IRepositorio repositorio, IDataAgroService dataAgroService,
            IVendedorHabilitadoConsumerMOA vendedorHabilitadoConsumer, IVendedoresConsumerMOA vendedoresConsumerMOA)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
            this.vendedorHabilitadoConsumer = vendedorHabilitadoConsumer;
            this.vendedoresConsumerMOA = vendedoresConsumerMOA;
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
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
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

            List<Models.FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
            VendedoresWSMOAResponse response = new VendedoresWSMOAResponse();
            try
            {
                response = vendedoresConsumerMOA.Request(codigoProveedor, fechas);
            }
            catch
            {

            }

            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == usuariomail);

            if (usuario.EsAdmin())
            {
                var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

                if (proveedor != null)
                {
                    usuariomail = proveedor.Mail;
                }
            }

            var vendedoresAprobados = GetVendedores(usuariomail, v => v.EstadoAprobacion == EstadoAprobacion.Aprobado || v.EstadoAprobacion == EstadoAprobacion.Deshabilitado)
                .Select(v => new Vendedor()
                {
                    descVendedor = v.RazonSocial,
                    estado = "",
                    estadoMoa = v.EstadoAprobacion == EstadoAprobacion.Aprobado ? (
                        (v.ContieneDocumentacionFisica.HasValue && v.ContieneDocumentacionFisica == true) ? "Habilitado"
                        : "Pendiente de envío documentación original") : v.EstadoAprobacionDescripcion,
                    idVendedor = v.CodigoProveedor
                });
            response.vendedores.AddRange(vendedoresAprobados);

            response.vendedores = response.vendedores.GroupBy(i => new
            {
                i.idVendedor,
                i.descVendedor
            })
                .Select(vendedor => vendedor.Skip(1)
                .Aggregate(
                    vendedor.First(), (a, o) =>
                    {
                        if (!a.estado.Contains("Pendiente de envío documentación original")
                        && (a.estadoMoa.Contains("Pendiente de envío documentación original") || o.estadoMoa.Contains("Pendiente de envío documentación original"))
                        && a.estado != ""
                        && !a.estado.Contains("Habilitado"))
                        {
                            a.estado = a.estado + " - Pendiente de envío documentación original";
                        }

                        return a;
                    }))
                .ToList();
            return response;
        }
        public List<ProveedorDto> GetAllClientsByType(int tipoProveedorId)
        {
            try
            {
                var clientesBD = repositorio.Listar<Proveedor>(p => p.TipoProveedor.Id == (tipoProveedorId > 0 ? tipoProveedorId : p.TipoProveedor.Id) && p.EstadoAprobacion == EstadoAprobacion.Aprobado)
                   .ToList();
                var clientesDto = clientesBD.Select(proveedor => new ProveedorDto(proveedor)).ToList();
                return clientesDto;
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
        public VendedoresWSMOAResponse AutocompleteProveedores(string usuariomail, string codigoProveedor, string fechaInicio, string fechaFin, int tipoProveedorId)
        {
            VendedoresWSMOAResponse response = new VendedoresWSMOAResponse();

            if (tipoProveedorId != 4 && tipoProveedorId != 5 )
            {
                if (fechaInicio == "")
                {
                    fechaInicio = DateTime.Now.AddDays(-1).ToShortDateString();
                }
                if (fechaFin == "")
                {
                    fechaFin = DateTime.Now.ToShortDateString();
                }

                List<Models.FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                try
                {
                    response = vendedoresConsumerMOA.Request(codigoProveedor, fechas);
                }
                catch
                {

                }
            }

            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == usuariomail);

            if (usuario.EsAdmin())
            {
                var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

                if (proveedor != null)
                {
                    usuariomail = proveedor.Mail;
                }
            }

            var vendedoresAprobados = GetVendedores(usuariomail,
                v => (v.EstadoAprobacion == EstadoAprobacion.Aprobado)
                    && v.TipoProveedor.Id == (tipoProveedorId > 0 ? tipoProveedorId : v.TipoProveedor.Id))
                .Select(v => new Vendedor()
                {
                    descVendedor = v.RazonSocial,
                    estado = "",
                    idVendedor = v.CodigoProveedor,
                    cuit = v.CUIT
                });
            response.vendedores.AddRange(vendedoresAprobados);

            response.vendedores = response.vendedores.GroupBy(i => new
            {
                i.idVendedor,
                i.descVendedor
            })
                .Select(vendedor => vendedor.Skip(1)
                .Aggregate(
                    vendedor.First(), (a, o) =>
                    {
                        return a;
                    }))
                .ToList();
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

                VendedorHabilitadoWSMOAResponse response = vendedorHabilitadoConsumer.Request(cuit, "MOA", user);
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
        public List<EstadoVendedorDto> GetVariosVendedoresStatus(List<string> cuitsVendedores, string user)
        {
            try
            {
                List<EstadoVendedorDto> listaResultados = new List<EstadoVendedorDto>();

                foreach (string cuit in cuitsVendedores.Select(s => s.Trim()).Distinct().ToList())
                {
                    var estadoVendedorDto = new EstadoVendedorDto
                    {
                        CUIT = cuit
                    };

                    if (cuit == null || cuit == "")
                    {
                        estadoVendedorDto.Estado = string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT");
                    }
                    VendedorHabilitadoWSMOAResponse response;

                    try
                    {
                        response = vendedorHabilitadoConsumer.Request(cuit, "MOA", user);
                    }
                    catch (InfoCustomException ex)
                    {
                        estadoVendedorDto.Estado = ex.Message;
                        listaResultados.Add(estadoVendedorDto);
                        continue;
                    }

                    if (response == null)
                    {
                        estadoVendedorDto.Estado = InfoMsg.ProveedorSinAlta;
                    }
                    else
                    {

                        if (response.status == null || response.status == "" || response.status == "Proveedor inexistente")
                        {
                            estadoVendedorDto.Estado = InfoMsg.ProveedorSinAlta;
                        }
                        else
                        {
                            estadoVendedorDto.CodigoProveedor = response.cabeceras.FirstOrDefault().proveedor;
                            estadoVendedorDto.RazonSocial = response.cabeceras.FirstOrDefault().descripcion;
                            estadoVendedorDto.Estado = response.status;
                        }
                    }

                    listaResultados.Add(estadoVendedorDto);
                }

                return listaResultados;
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

            if (usuario.EsAdmin() || usuario.TienePermiso(PermisoEnum.ElegirTodosVendedores))
            {
                listadoProveedores = repositorio
                        .Listar<Proveedor>(p => p.EstadoAprobacion == EstadoAprobacion.Aprobado)
                        .Where(filtro)
                        .Select(proveedor => new ProveedorDto(proveedor,false)).ToList();
            }
            else
            {
                var proveedores = usuario.Proveedores.ToList();


                if (filtro != null)
                {
                    proveedores = proveedores.Where(filtro).ToList();
                }

                listadoProveedores.AddRange(proveedores.Select(proveedor => new ProveedorDto(proveedor,false)).ToList());
            }


            foreach (var item in listadoProveedores.Where(a => a.CUIT == null || a.CUIT == ""))
            {
                item.CUIT = "-";
            }
            foreach (var item in listadoProveedores.Where(a => a.RazonSocial == null || a.RazonSocial == ""))
            {
                item.RazonSocial = "-";
            }
            return listadoProveedores.Distinct().ToList();
        }
        public string AgregarVendedor(string mailUsuario, string cuit, int tipoProveedor)
        {
            var usuario = repositorio.Obtener<Entities.Usuario>(u => u.Mail == mailUsuario);

            if (cuit == null || cuit == "")
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT"));
            }

            if (usuario.Proveedores.Where(x => x.CUIT == cuit).Any())
                throw new ValidationCustomException(ErrorMsg.ErrorVendedorRepetido);

            if (tipoProveedor == 2)
            {
                var nuevoVendedor = new Proveedor
                {
                    CUIT = cuit,
                    Mail = usuario.Mail,
                    EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                    CodigoProveedor = FormatearCodigoProveedor(cuit),
                    TipoProveedor = ObtenerTipoPorNombreCorto("G"),
                    FechaSolicitud = DateTime.Now
                };

                if (usuario.EsCorredor())
                {
                    var infoDA = dataAgroService.ObtenerValidarCUITProveedorGranos(cuit);
                    string comercial = "";
                    if (infoDA.HayError)
                    {
                        if (infoDA.ListaErrores[0].Message == "El cuit no tiene ninguno comercial asociado")
                        {
                            var infoDACorredor = dataAgroService.ObtenerValidarCUITProveedorGranos(usuario.ObtenerCorredor().CUIT, true);

                            if (infoDACorredor.HayError)
                            {
                                throw new ValidationCustomException(infoDACorredor.ListaErrores[0].Message);
                            }

                            infoDA.ComercialId = infoDACorredor.ComercialId;
                            comercial = string.Concat(infoDACorredor.ComercialNombres, " ", infoDACorredor.ComercialApellido);
                        }
                        else
                        {
                            throw new ValidationCustomException(infoDA.ListaErrores[0].Message);
                        }
                    }
                    else
                    {
                        comercial = string.Concat(infoDA.ComercialNombres, " ", infoDA.ComercialApellido);
                    }

                    var hist = new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        Usuario_Id = usuario.Id,
                        EstadoAprobacion = nuevoVendedor.EstadoAprobacion,
                        Observacion = "Proveedor habilitado en DataAgro"
                    };
                    nuevoVendedor.HistorialAprobaciones.Add(hist);

                    nuevoVendedor.IdProveedorCorredor = usuario.ObtenerCorredor().Id;
                    nuevoVendedor.RazonSocial = infoDA.ProveedorRazonSocial;
                    nuevoVendedor.Comercial = comercial;
                    nuevoVendedor.IdComercialDataAgro = infoDA.ComercialId;
                    nuevoVendedor.IdDataAgro = infoDA.ProveedorId;
                    nuevoVendedor.TipoProveedor = ObtenerTipoPorNombreCorto("CORR");

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
                }
                usuario.Proveedores.Add(nuevoVendedor);
            }
            else
            {
                Proveedor proveedor = new Proveedor
                {
                    CUIT = cuit,
                    Mail = usuario.Mail,
                    EstadoAprobacion = EstadoAprobacion.EtapaFinal,
                    Observaciones = "Esperando aprobación.",
                    TipoProveedor = ObtenerTipoPorNombreCorto("CLI")
                };

                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>
                {
                    new ProveedorHistorialAprobacion()
                    {
                        Fecha = DateTime.Now,
                        Proveedor_Id = proveedor.Id,
                        EstadoAprobacion = EstadoAprobacion.EtapaFinal,
                        Observacion = "Registro de usuario cliente",
                        Usuario_Id = usuario.Id
                    }
                };

                usuario.Proveedores.Add(proveedor);
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
            return string.Concat("00", CUIT.Substring(2, 8));
        }
        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto) => repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);
    }
}
