using CsvHelper;
using CsvHelper.Configuration;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Extensions;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Helpers.CSV;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using AppCCPPRequests = SustitucionMOAWS.WSRequests.AplicacionCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAplicacionCartaPorteConsumer consumer;
        private readonly IEmailAplicacionCPService emailAplicacionCPService;

        public AplicacionCartaPorteService(IRepositorio repositorio,
                                           IAplicacionCartaPorteConsumer consumer,
                                           IEmailAplicacionCPService emailAplicacionCPService)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
            this.emailAplicacionCPService = emailAplicacionCPService;
        }

        public List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            var fechaInicioDateTime = DataFormatter.StringToDateTime(fechaInicio, "fechaInicio");
            var fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fechaFin");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            string cuit = usuario.CUITRegistro;
            bool isAdmin = usuario.TienePermiso(PermisoEnum.AbmAplicacionesCcpp);

            var rawList = repositorio.Listar<AplicacionCartaPorte>(apl =>
                (isAdmin || apl.Proveedor.CUIT == cuit) &&
                fechaInicioDateTime <= apl.FechaAlta && fechaFinDateTime >= DbFunctions.TruncateTime(apl.FechaAlta) &&
                apl.Estado != EstadoAplicacionCartaPorte.Eliminado
            );

            var lista = rawList.Select(apl => new AplicacionCartaPorteDto(apl)).ToList();
            return lista;
        }

        public AplicacionCartaPorteDto Obtener(int aplicacionCCPPId, string mailUsuario)
        {
            return new AplicacionCartaPorteDto(new AplicacionCartaPorte());
        }

        public AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones)
        {
            return new AplicacionCartaPorteFiltrosDto(aplicaciones);
        }

        public void EliminarAplicacion(int aplicacionId)
        {
            var aplicacion = repositorio.Obtener<AplicacionCartaPorte>(aplicacionId);
            if (aplicacion == null)
                throw new InfoCustomException("No se ha encontrado la aplicación.");
            if (aplicacion.Estado != EstadoAplicacionCartaPorte.Pendiente)
                throw new InfoCustomException("No se puede eliminar la aplicación.");
            aplicacion.Estado = EstadoAplicacionCartaPorte.Eliminado;
            repositorio.GuardarCambios();
        }

        public ComboAplicacionesContratosCcppResponse ObtenerCombosDeContratoCCPP(string mailUsuario, string codigoProveedor, bool esCodigoCorredor)
        {
            Log.Info($"Busqueda combo app ccpp: mail={mailUsuario} el codigo proveedor= {codigoProveedor}");
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            //var aplicacionesPendientes = ObtenerAplicacionesDisponiblesSap(usuario, codigoProveedor, esCodigoCorredor);
            var aplicacionesDisponiblesSap = ObtenerAplicacionesDisponiblesSap(usuario, codigoProveedor, esCodigoCorredor);

            var contratos = ObtenerContratosDisponibles(aplicacionesDisponiblesSap.Contratos);

            var aplicacionesPendientesDeProcesar = ObtenerAplicacionesPendientesDeProcesar(aplicacionesDisponiblesSap);
            var cartasPorte = ObtenerCartasPorteDisponibles(aplicacionesDisponiblesSap.CartasDePorte, aplicacionesPendientesDeProcesar);

            return new ComboAplicacionesContratosCcppResponse { CartasPorte = cartasPorte, Contratos = contratos };
        }

        public void GuardarAplicacion(CrearAplicacionCartaPorte aplicacionACrear, string mailUsuario)
        {
            Log.Info($"Aplicaciones CCPP: GuardarAplicacion datos:{aplicacionACrear.ToJson()}");
            ValidarSchema(aplicacionACrear, "AplicacionCartaPorte", "GuardarAplicacion");
            if (!aplicacionACrear.ValidarKilogramos())
                throw new InfoCustomException("Revisar valor de KG.");

            var aplicacionesDisponiblesSap = ObtenerAplicacionesDisponiblesSap(aplicacionACrear.ContratoSeleccionado.CodigoProveedor);
            var contratosValidos = ObtenerContratosDisponibles(aplicacionesDisponiblesSap.Contratos);

            if (!aplicacionACrear.ValidarContrato(contratosValidos))
                throw new InfoCustomException("Revisar contrato seleccionado.");


            var aplicacionesPendientesAplicar = ObtenerAplicacionesPendientesDeProcesar(aplicacionesDisponiblesSap);
            var cartasPorteValidas = ObtenerCartasPorteDisponibles(aplicacionesDisponiblesSap.CartasDePorte, aplicacionesPendientesAplicar);

            if (!aplicacionACrear.ValidarCartaPorteSeleccionada(cartasPorteValidas))
                throw new InfoCustomException("Revisar carta porte seleccionada.");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == aplicacionACrear.ContratoSeleccionado.CodigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

            var estadoAplicacion = ObtenerEstadoNuevaAplicacion(aplicacionACrear.ContratoSeleccionado, contratosValidos);

            var aplicacion = new AplicacionCartaPorte(aplicacionACrear, usuario, proveedor, estadoAplicacion);
            repositorio.Agregar(aplicacion);

            repositorio.GuardarCambios();
        }

        public CargaMasivaResponse ProcesarCargaMasiva(HttpPostedFileBase archivo, string usuarioMail, string proveedorCodigo, bool esCodigoCorredor)
        {
            if (archivo == null || archivo.ContentLength == 0 || Path.GetExtension(archivo.FileName).ToLower() != ".csv")
            {
                throw new ValidationCustomException("Debe seleccionar un archivo .csv válido");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == usuarioMail);

            var registrosArchivo = ObtenerRegistrosCargaMasiva(archivo);

            var aplicacionesDisponiblesSap = ObtenerAplicacionesDisponiblesSap(usuario, proveedorCodigo, esCodigoCorredor);

            var contratosDisponibles = ObtenerContratosDisponibles(aplicacionesDisponiblesSap.Contratos);
            if (!contratosDisponibles.Any())
            {
                throw new ValidationCustomException("No se encontraron contratos disponibles en SAP");
            }

            var aplicacionesPendientesDeProcesar = ObtenerAplicacionesPendientesDeProcesar(aplicacionesDisponiblesSap);

            var cartasPorteDisponibles = ObtenerCartasPorteDisponibles(aplicacionesDisponiblesSap.CartasDePorte, aplicacionesPendientesDeProcesar);
            if (!cartasPorteDisponibles.Any())
            {
                throw new ValidationCustomException("No se encontraron cartas de porte disponibles en SAP");
            }

            var logCargaMasiva = new AplicacionCartaPorteCargaMasiva
            {
                Fecha = DateTime.Now,
                NombreArchivo = archivo.FileName,
                UsuarioId = usuario.Id
            };
            repositorio.Agregar(logCargaMasiva);

            short fila = 2; // Fila inicial en el csv
            var registrosConError = new List<ErrorValidacionCargaMasivaCCPP>();
            var registrosOK = new List<AplicacionGuardadaCargaMasivaCCPP>();

            foreach (var regItem in registrosArchivo)
            {
                var logCargaMasivaFila = new AplicacionCartaPorteCargaMasivaFila
                {
                    AplicacionCartaPorteCargaMasiva = logCargaMasiva,
                    CartaPorte = regItem.CartaDePorte,
                    Contrato = regItem.ContratoNumero,
                    FilaNumero = fila,
                    Kilos = regItem.Kilos
                };
                repositorio.Agregar(logCargaMasivaFila);

                if (RegistroCargaMasivaEsValido(regItem, out string msjError, contratosDisponibles,
                        cartasPorteDisponibles, registrosOK))
                {
                    var contrato = contratosDisponibles.First(c => c.NumeroContrato == regItem.ContratoNumero);
                    var estadoAplicacion = ObtenerEstadoNuevaAplicacion(contrato, contratosDisponibles);

                    registrosOK.Add(new AplicacionGuardadaCargaMasivaCCPP
                    {
                        ContratoNumero = regItem.ContratoNumero,
                        CartaDePorte = regItem.CartaDePorte,
                        Estado = estadoAplicacion,
                        Kilos = regItem.Kilos
                    });
                }
                else
                {
                    registrosConError.Add(new ErrorValidacionCargaMasivaCCPP
                    {
                        Fila = fila,
                        ContratoNumero = regItem.ContratoNumero,
                        CartaDePorte = regItem.CartaDePorte,
                        Kilos = regItem.Kilos,
                        Error = msjError
                    });
                    logCargaMasivaFila.Error = msjError;
                }
                fila++;
            }

            var codigoProveedor = contratosDisponibles.First().CodigoProveedor;
            var response = GuardarCargaMasivaAplicaciones(registrosConError, registrosOK, codigoProveedor, usuario);
            return response;
        }

        public void AprobarAplicacionPendiente(int idAplicacion)
        {
            var aplicacion = repositorio.Obtener<AplicacionCartaPorte>(idAplicacion) ?? throw new Exception("Aplicación no encontrada");

            if (aplicacion.Estado == EstadoAplicacionCartaPorte.PendienteAprobacion)
            {
                aplicacion.Estado = EstadoAplicacionCartaPorte.Pendiente;
                repositorio.GuardarCambios();
            }
        }

        public void RechazarAplicacionPendiente(int idAplicacion, string motivo)
        {
            var aplicacion = repositorio.Obtener<AplicacionCartaPorte>(idAplicacion) ?? throw new Exception("Aplicación no encontrada");

            if (aplicacion.Estado == EstadoAplicacionCartaPorte.PendienteAprobacion)
            {
                aplicacion.Estado = EstadoAplicacionCartaPorte.Rechazada;
                aplicacion.Error = motivo;
                repositorio.GuardarCambios();

                emailAplicacionCPService.EnviarMailAplicacionRechazada(aplicacion.CartaPorte, aplicacion.Contrato, motivo, aplicacion.Usuario.Mail);
            }
        }

        private void ValidarSchema<T>(T schema, string controller, string metodo)
        {
            var validationContext = new ValidationContext(schema);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(schema, validationContext, validationResults))
            {
                Log.Debug(controller, metodo, string.Join("; ", validationResults.Select(valRes => valRes.ErrorMessage)));
                throw new InfoCustomException("Error validando formulario.");
            }
        }

        private List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorteDisponibles(
              List<AplicacionPendienteCartaPorte> cartasDePorte,
              IEnumerable<AplicacionCartaPorte> aplicacionesPendientesDeProcesar)
        {
            return cartasDePorte
                .Select(cp =>
                {
                    var kgPendientesCargados = aplicacionesPendientesDeProcesar
                       .Where(appPendiente => appPendiente.CartaPorte == cp.NumeroCartaPorte)
                       .Sum(appPendiente => appPendiente.Kilogramos);
                    var kgPendientes = kgPendientesCargados > cp.Cantidad ? 0 : cp.Cantidad - kgPendientesCargados;

                    return new CartaPorteParaAplicacionCartaPorte(
                        numeroCartaPorte: cp.NumeroCartaPorte,
                        kgPendientes: kgPendientes,
                        material: cp.Material);
                }).ToList();
        }

        private List<ContratoParaAplicacionCartaPorte> ObtenerContratosDisponibles(List<AplicacionPendienteContrato> contratos)
        {
            return contratos.Select(x => new ContratoParaAplicacionCartaPorte
            {
                CodigoProveedor = x.CodigoProveedor,
                Material = x.Material,
                NumeroContrato = x.NumeroContrato,
                TieneAnticipo = x.TieneAnticipo
            }).ToList();
        }

        private string ObtenerCodigoProveedorSeleccionado(Usuario usuario, Proveedor proveedorAsignado, string codigoSeleccionado, bool esCodigoCorredor)
        {
            var puedeSeleccionarProveedor = usuario.TienePermiso(PermisoEnum.SeleccionarVendedor);
            if (
                usuario.EsCorredor() && (!puedeSeleccionarProveedor || proveedorAsignado.CodigoProveedor == codigoSeleccionado) ||
                (proveedorAsignado.CodigoProveedor != codigoSeleccionado && esCodigoCorredor))
            {
                return null;
            }
            if (puedeSeleccionarProveedor && !string.IsNullOrEmpty(codigoSeleccionado))
            {
                return codigoSeleccionado;
            }
            return proveedorAsignado.CodigoProveedor;
        }

        private string ObtenerCodigoCorredorSeleccionado(Usuario usuario, Proveedor proveedorAsignado, string codigoSeleccionado, bool esCodigoCorredor)
        {
            if (usuario.EsCorredor() && proveedorAsignado.CodigoProveedor != codigoSeleccionado && !esCodigoCorredor)
            {
                return null;
            }
            if (esCodigoCorredor && !string.IsNullOrEmpty(codigoSeleccionado))
            {
                return codigoSeleccionado;
            }
            return usuario.EsCorredor() ? proveedorAsignado.CodigoProveedor : null;
        }
        private List<AplicacionCartaPorte> ObtenerAplicacionesPendientesDeProcesar(
            AppCartasPortePendienteResponse appCartaPorteResponse
            )
        {
            return ObtenerAplicacionesPendientesDeProcesar(appCartaPorteResponse.Contratos
                .GroupBy(c=>c.CodigoProveedor)
                .Select(g=>g.Key));
        }
        private List<AplicacionCartaPorte> ObtenerAplicacionesPendientesDeProcesar(IEnumerable<string> codigosProveedor
            )
        {
            return repositorio.Listar<AplicacionCartaPorte>(app =>
                app.Estado == EstadoAplicacionCartaPorte.Pendiente &&
                codigosProveedor.Contains(app.Proveedor.CodigoProveedor));
        }

        private AppCCPPRequests.AppCartasPortePendienteResponse ObtenerAplicacionesDisponiblesSap(string codigoProveedor)
        {
            var req = new AppCCPPRequests.AppCartasPortePendienteRequest { Proveedor = codigoProveedor };
            return ObtenerAplicacionesDisponiblesSap(req);
        }

        private AppCCPPRequests.AppCartasPortePendienteResponse ObtenerAplicacionesDisponiblesSap(Usuario usuario, string proveedorCodigo, bool esCodigoCorredor)
        {
            var proveedorAsignado = usuario.ObtenerProveedor();
            var codigoProveedorSeleccionado = ObtenerCodigoProveedorSeleccionado(usuario, proveedorAsignado, proveedorCodigo, esCodigoCorredor);
            var codigoCorredor = ObtenerCodigoCorredorSeleccionado(usuario, proveedorAsignado, proveedorCodigo, esCodigoCorredor);

            var ccppPendienteReq = new AppCCPPRequests.AppCartasPortePendienteRequest
            {
                Proveedor = codigoProveedorSeleccionado,
                Corredor = codigoCorredor
            };

            return ObtenerAplicacionesDisponiblesSap(ccppPendienteReq);
        }

        private AppCCPPRequests.AppCartasPortePendienteResponse ObtenerAplicacionesDisponiblesSap(AppCCPPRequests.AppCartasPortePendienteRequest req)
        {
            return consumer.ObtenerPendientesDeAplicar(req);
        }

        private List<AplicacionCCPPRecord> ObtenerRegistrosCargaMasiva(HttpPostedFileBase archivo)
        {
            var registrosArchivo = new List<AplicacionCCPPRecord>();

            using (var streamReader = new StreamReader(archivo.InputStream))
            using (var csvReader = new CsvReader(streamReader,
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
            {
                try
                {
                    csvReader.Context.RegisterClassMap<AplicacionCCPPRecordMap>();
                    registrosArchivo = csvReader.GetRecords<AplicacionCCPPRecord>().ToList();
                }
                catch (HeaderValidationException hvex)
                {
                    Log.Error(hvex);
                    throw new ValidationCustomException("Error en las cabeceras del archivo. Verificar que la cabecera tenga el formato 'CONTRATO;CARTA DE PORTE;KILOS'", hvex);
                }
            }

            if (!registrosArchivo.Any())
            {
                throw new ValidationCustomException("No se encontraron registros en el archivo. Si el mismo tiene datos, verifique el formato: cabecera 'CONTRATO;CARTA DE PORTE;KILOS' y ';' como separador en cada campo de cada registro.");
            }
            return registrosArchivo;
        }

        private bool RegistroCargaMasivaEsValido(AplicacionCCPPRecord registroMasiva, out string error,
            List<ContratoParaAplicacionCartaPorte> contratosDisponibles,
            List<CartaPorteParaAplicacionCartaPorte> cartasPorteDisponibles,
            List<AplicacionGuardadaCargaMasivaCCPP> aplicacionesAnterioresDelArchivo)
        {
            registroMasiva.ContratoNumero = registroMasiva.ContratoNumero.ToContratoSAP();
            registroMasiva.CartaDePorte = registroMasiva.CartaDePorte.ToCartaPorteSAP();

            var contrato = contratosDisponibles.FirstOrDefault(c => c.NumeroContrato == registroMasiva.ContratoNumero);
            var cartaPorte = cartasPorteDisponibles.FirstOrDefault(cp => cp.NumeroCartaPorte == registroMasiva.CartaDePorte);

            if (contrato == null || cartaPorte == null)
            {
                error = (contrato == null && cartaPorte == null) ?
                            "Contrato y Carta de porte inexistentes" :
                            (contrato == null ? "Contrato inexistente" : "Carta de porte inexistente");
                return false;
            }

            if (contrato.Material != cartaPorte.Material)
            {
                error = "La Carta de porte no se corresponde con el Contrato";
                return false;
            }

            if (!int.TryParse(registroMasiva.Kilos, out int kilosSolicitados) || kilosSolicitados == 0)
            {
                error = $"El valor ingresado en Kilos es inválido ({registroMasiva.Kilos})";
                return false;
            }

            //var kilosPendientesAplicar = aplicacionesPendientesBD
            //    .Where(x => x.CartaPorte == registroMasiva.CartaDePorte)
            //    .Sum(x => x.Kilogramos);

            var kilosArchivoEnProceso = aplicacionesAnterioresDelArchivo
                .Where(x => x.CartaDePorte == registroMasiva.CartaDePorte)
                .Sum(x => int.Parse(x.Kilos));

            //var kilosDisponibles = cartaPorte.KgPendientes - kilosPendientesAplicar - kilosArchivoEnProceso;
            var kilosDisponibles = cartaPorte.KgPendientes - kilosArchivoEnProceso;

            if (kilosDisponibles < kilosSolicitados)
            {
                error = "No hay kilos disponibles para aplicar suficientes para la cantidad solicitada";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private CargaMasivaResponse GuardarCargaMasivaAplicaciones(List<ErrorValidacionCargaMasivaCCPP> registrosConError,
            List<AplicacionGuardadaCargaMasivaCCPP> registrosOK, string codigoProveedor, Usuario usuario)
        {
            if (registrosConError.Any())
            {
                repositorio.GuardarCambios();
                return new CargaMasivaResponse
                {
                    HayErroresValidacion = true,
                    ErroresValidacion = registrosConError
                };
            }
            else
            {
                var proveedor = repositorio.Obtener<Proveedor>(p =>
                    p.CodigoProveedor == codigoProveedor &&
                    p.EstadoAprobacion == EstadoAprobacion.Aprobado);

                foreach (var aplNueva in registrosOK)
                {
                    repositorio.Agregar(new AplicacionCartaPorte
                    {
                        Usuario_Id = usuario.Id,
                        Proveedor_Id = proveedor.Id,
                        Contrato = aplNueva.ContratoNumero,
                        CartaPorte = aplNueva.CartaDePorte,
                        Kilogramos = int.Parse(aplNueva.Kilos),
                        Estado = aplNueva.Estado,
                        FechaAlta = DateTime.Now
                    });
                }
                repositorio.GuardarCambios();

                return new CargaMasivaResponse
                {
                    HayErroresValidacion = false,
                    AplicacionesGuardadas = registrosOK
                };
            }
        }

        private EstadoAplicacionCartaPorte ObtenerEstadoNuevaAplicacion(ContratoParaAplicacionCartaPorte contratoSeleccionado,
            List<ContratoParaAplicacionCartaPorte> contratosDisponiblesSap)
        {
            if (contratoSeleccionado.TieneAnticipo)
            {
                return EstadoAplicacionCartaPorte.Pendiente;
            }

            var contratoConAnticipo =
                contratosDisponiblesSap.FirstOrDefault(c =>
                    c.NumeroContrato != contratoSeleccionado.NumeroContrato &&
                    c.Material == contratoSeleccionado.Material &&
                    c.TieneAnticipo);

            if (contratoConAnticipo != null)
            {
                Log.Debug($"Aplicación para contrato {contratoSeleccionado.NumeroContrato} queda pendiente de aprobación. Hay otro contrato con anticipo ({contratoConAnticipo.NumeroContrato}, {contratoConAnticipo.Material})");
                return EstadoAplicacionCartaPorte.PendienteAprobacion;
            }
            else
            {
                return EstadoAplicacionCartaPorte.Pendiente;
            }
        }
    }
}
