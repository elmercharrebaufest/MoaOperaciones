using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaActualizacionStrategy : ConsultaCommon, IConsultaStrategy
    {
        public List<string> Names => new List<string>() { "Actualización" };

        private readonly IUsuarioService usuarioService;
        private readonly IAzureService azureService;
        private readonly ITimeProvider timeProvider;
        private readonly string rutaArchivosCM05 = ConfigurationManager.AppSettings["RutaArchivosCM05"];

        public ConsultaActualizacionStrategy(IRepositorio repositorio, IEmailService emailService, IUsuarioService usuarioService, IAzureService azureService,
            ITimeProvider timeProvider) : base(repositorio, emailService)
        {
            this.usuarioService = usuarioService;
            this.azureService = azureService;
            this.timeProvider = timeProvider;
        }

        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            try
            {
                this.RellenarCampos(consulta, comentario, destinatarios);
                Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);
                var subcategoria = this.repositorio.Obtener<SubCategoria>(s => s.Id == consulta.SubCategoria_Id);

                this.repositorio.Agregar(consulta);
                this.repositorio.GuardarCambios();

                if (files.Count > 0)
                {
                    if (subcategoria.Code == SubCategorias.CM05)
                    {
                        Proveedor proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == consulta.CodigoProveedor);
                        try
                        {
                            var msj = this.ProcesarCM05(files, proveedor.CUIT, comentario.Id, false);
                        }
                        catch (ValidationCustomException vex)
                        {
                            consulta.Comentarios.Add(new Comentario
                            {
                                Consulta_Id = consulta.Id,
                                Detalle = vex.Message,
                                Fecha = DateTime.Now,
                                Usuario_Id = usuario.Id
                            });

                            consulta.FechaUltimaModificacion = DateTime.Now;

                            repositorio.GuardarCambios();
                        }
                    }
                    GuardarAdjuntoComentario(consulta.Id, files);
                }

                EnviarMail(consulta, comentario, destinatarios, files);

                return consulta;
            }
            catch (Exception e)
            {
                Log.Error("Ha ocurrido un error al intentar generar la consulta.", e);
                throw e;
            }
        }

        #region Actualizacion_CM05

        public string ProcesarCM05(HttpFileCollectionBase archivos, string cuitProveedor, int? comentario_Id = null, bool esCargaInterna = false)
        {
            try
            {
                var errores = new List<string>();
                string resultado = string.Empty;

                bool existeArchivoConCoeficientes = false;
                for (int i = 0; i < archivos.Count; i++)
                {
                    HttpPostedFileBase archivo = archivos[i];
                    if (archivo.ContentType == "application/pdf")
                    {
                        var operacionOCRId = Task.Run(async () => await azureService.AnalizarImagenAsync(archivo)).Result;

                        Thread.Sleep(2000);

                        var elementosLeidos = Task.Run(async () => await azureService.ObtenerResultadoOCRAsync(operacionOCRId)).Result;

                        if (elementosLeidos.Any(str => str == "Determinación del Coeficiente Unificado"))
                        {
                            Comentario comentario = null;
                            int? consulta_Id = null;
                            int archivo_Id = 1;
                            string nombreArchivo = string.Format("{0}_{1}", comentario_Id, Path.GetFileName(archivo.FileName));

                            if (comentario_Id != null)
                            {
                                comentario = repositorio.Obtener<Comentario>(comentario_Id);
                                consulta_Id = comentario.Consulta_Id;
                                archivo_Id = comentario.Archivos.Single(file => file.ObtenerNombre() == nombreArchivo).Id;
                            }

                            if (esCargaInterna)
                            {
                                var ruta = ArmarRutaCarpetaCM05(cuitProveedor);
                                var rutaArchivo = string.Concat(ruta, "/", nombreArchivo);

                                Directory.CreateDirectory(ruta);

                                var ArchivoAGuardar = new Archivo()
                                {
                                    FileKey = FileKeys.FormularioCM05,
                                    Ruta = rutaArchivo,
                                };

                                archivo.SaveAs(rutaArchivo);
                                repositorio.Agregar(ArchivoAGuardar);
                                repositorio.GuardarCambios();

                                archivo_Id = ArchivoAGuardar.Id;
                            }

                            resultado = ProcesarArchivoCoeficientesImpuestosIngresosBrutos(elementosLeidos.ToList(), archivo_Id, cuitProveedor, consulta_Id, esCargaInterna);
                            existeArchivoConCoeficientes = true;
                            break;
                        }
                    }
                }

                if (!existeArchivoConCoeficientes)
                {
                    throw new ValidationCustomException("No se pudieron obtener los coeficientes. Por favor, asegúrese de adjuntar el documento correspondiente.");
                }

                return resultado;
            }
            catch (ValidationCustomException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidationCustomException(ErrorMsg.ErrorCargaCM05, ex, true);
            }
        }
        public virtual string ArmarRutaCarpetaCM05(string username)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == username);
            return string.Format("{0}/{1}", rutaArchivosCM05, usuario.Id);
        }

        private string ProcesarArchivoCoeficientesImpuestosIngresosBrutos(List<string> elementosLeidos, int archivo_Id, string cuitProveedor, int? consulta_Id, bool esCargaInterna)
        {
            //Descarto palabras que ya se que son "basura"
            elementosLeidos
                .RemoveAll(elemento => elemento.StartsWith("..") && elemento.EndsWith("..") ||
                                       elemento.All(caracter => caracter == '.'));

            int indiceDeterminacionDelCoeficienteUnificado = elementosLeidos.IndexOf("Determinación del Coeficiente Unificado");
            string encabezadoFormulario = "OSIRIS";
            int indiceComienzoPaginaCoeficientesBrutos = elementosLeidos.Take(indiceDeterminacionDelCoeficienteUnificado).ToList().LastIndexOf(encabezadoFormulario);
            List<string> info_DeterminacionCoeficienteUnificado = elementosLeidos.Skip(indiceComienzoPaginaCoeficientesBrutos).ToList();

            string cuit = SacarHasta(info_DeterminacionCoeficienteUnificado, "CUIT:")[0].Replace("-", "");

            int anticipoAux;
            int anticipo = Int32.TryParse(SacarHasta(info_DeterminacionCoeficienteUnificado, "Anticipo:")[0], out anticipoAux) ? anticipoAux : 0;

            int sedeAux;
            int sede = Int32.TryParse(SacarHasta(info_DeterminacionCoeficienteUnificado, "Sede:")[0], out sedeAux) ? sedeAux : 0;

            string secuencia = SacarHasta(info_DeterminacionCoeficienteUnificado, "Secuencia:")[0];
            int? idSecuencia =
                secuencia == "Original" ? (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original :
                secuencia.Contains("Rectificativa") ? (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa :
                (int?)null;

            string razonSocial = SacarHasta(info_DeterminacionCoeficienteUnificado, "Contribuyente:")[0];

            var ingresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificado
            {
                EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                CUIT = cuit,
                Anticipo = anticipo,
                Sede = sede,
                FechaCarga = timeProvider.Now(),
                FechaUltimaModificacion = timeProvider.Now(),
                Consulta_Id = consulta_Id,
                Archivo_Id = archivo_Id,
                SecuenciaIngresosBrutosCoeficienteUnificado_Id = idSecuencia,
                RazonSocial = razonSocial,
            };

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetalles = new List<IngresosBrutosCoeficienteUnificadoDetalle>();

            List<string> listadoCoeficientes = SacarHasta(elementosLeidos, "Coeficiente Unificado");

            for (int i = 0; i < listadoCoeficientes.Count; i++)
            {
                try
                {
                    int numeroJurisdiccionAux;
                    int? numeroJurisdiccion = int.TryParse(listadoCoeficientes[i], out numeroJurisdiccionAux) ? numeroJurisdiccionAux : (int?)null;

                    string jurisdiccion = listadoCoeficientes[i + 1];

                    DateTime fechaInicioAux;
                    DateTime? fechaInicio = DateTime.TryParseExact(listadoCoeficientes[i + 2], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioAux) ? fechaInicioAux : (DateTime?)null;

                    DateTime fechaCeseAux;
                    DateTime? fechaCese = DateTime.TryParseExact(listadoCoeficientes[i + 3], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaCeseAux) ? fechaCeseAux : (DateTime?)null;

                    i += (fechaInicio.HasValue ? fechaCese.HasValue ? 4 : 3 : 2);

                    decimal coeficienteIngresosAux;
                    decimal? coeficienteIngresos = decimal.TryParse(listadoCoeficientes[i++], out coeficienteIngresosAux) ? coeficienteIngresosAux : (decimal?)null;

                    decimal coeficienteGastosAux;
                    decimal? coeficienteGastos = decimal.TryParse(listadoCoeficientes[i++], out coeficienteGastosAux) ? coeficienteGastosAux : (decimal?)null;

                    decimal coeficienteUnificadoAux;
                    decimal? coeficienteUnificado = decimal.TryParse(listadoCoeficientes[i], out coeficienteUnificadoAux) ? coeficienteUnificadoAux : (decimal?)null;

                    IngresosBrutosCoeficienteUnificadoDetalle detalleGenerado = new IngresosBrutosCoeficienteUnificadoDetalle
                    {
                        NumeroJurisdiccion = numeroJurisdiccion,
                        Jurisdiccion = jurisdiccion,
                        FechaInicio = fechaInicio,
                        FechaCese = fechaCese,
                        CoeficienteIngresos = coeficienteIngresos,
                        CoeficienteGastos = coeficienteGastos,
                        CoeficienteUnificado = coeficienteUnificado,
                        FechaUltimaModificacion = timeProvider.Now()
                    };

                    //repositorio.Agregar(detalleGenerado);

                    ingresosBrutosCoeficienteUnificadoDetalles.Add(detalleGenerado);

                    if (ingresosBrutosCoeficienteUnificadoDetalles.Count >= 24)
                        break;
                }
                catch (Exception)
                {

                }
            }

            ingresosBrutosCoeficienteUnificado.Detalle = ingresosBrutosCoeficienteUnificadoDetalles;
            ingresosBrutosCoeficienteUnificado.MalCargada = string.IsNullOrWhiteSpace(cuit) || anticipo == 0 || sede == 0 || !idSecuencia.HasValue || string.IsNullOrWhiteSpace(razonSocial) ||
                ingresosBrutosCoeficienteUnificadoDetalles.Any(i => !i.CoeficienteUnificado.HasValue || !i.NumeroJurisdiccion.HasValue || string.IsNullOrWhiteSpace(i.Jurisdiccion));

            repositorio.Agregar(ingresosBrutosCoeficienteUnificado);



            MovimientoIngresosBrutosCoeficienteUnificado movimientoIngresosBrutosCoeficienteUnificado = new MovimientoIngresosBrutosCoeficienteUnificado
            {
                IngresosBrutosCoeficienteUnificado_Id = ingresosBrutosCoeficienteUnificado.Id,
                Observaciones = $"Creado por: {cuitProveedor}",
                Fecha = timeProvider.Now(),
                TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = 1,
                OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = 1,
                EstadoAnterior_Id = 1,
                EstadoPosterior_Id = 1,
            };
            repositorio.Agregar(movimientoIngresosBrutosCoeficienteUnificado);

            repositorio.GuardarCambios();

            return
                esCargaInterna ?
                    ingresosBrutosCoeficienteUnificado.MalCargada ?
                        SuccessMsg.AltaFormularioCM05CargaInternaMalCargadoOK
                      : SuccessMsg.AltaFormularioCM05CargaInternaOK
                : cuit != cuitProveedor ?
                    SuccessMsg.AltaFormularioCM05DistintoCUITOK
                : string.Empty;
        }

        private List<string> SacarHasta(IList<string> listaStrings, string elemento)
        {
            try
            {
                return listaStrings.Skip(1 + listaStrings.IndexOf(elemento)).ToList();
            }
            catch (Exception) { }

            return new List<string>();
        }

        #endregion

    }
}
