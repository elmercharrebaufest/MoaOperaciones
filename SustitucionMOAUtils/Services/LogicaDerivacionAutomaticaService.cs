// Ignore Spelling: reasignación reasignaciones Inicializar redimensionamiento

using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class LogicaDerivacionAutomaticaService : ILogicaDerivacionAutomaticaService
    {
        protected IRepositorio repositorio;
        protected IEntradaServicioService entradaServicioService;

        /// <summary>
        /// Initial Delay:momento de la primer ejecución
        /// _interval: cada cuanto se corre luego
        /// </summary>
        public LogicaDerivacionAutomaticaService(IRepositorio repositorio, IEntradaServicioService entradaServicioService)
        {
            this.repositorio = repositorio;
            this.entradaServicioService = entradaServicioService;
        }

        /// <summary>
        /// Método encargado del proceso de reasignación automática - MMSN - 1151
        /// Separado del Timer para ser corrido a demanda por los usuarios con el rol Administración
        /// </summary>
        public string CorrerProcesoReasignacion()
        {
            Logger.Log.Info("Corriendo proceso de reasignación automática");

            List<IGrouping<int, UsuarioReasignacion>> registrosAgrupados;
            IEnumerable<UsuarioReasignacion> registrosReasignacion;

            try
            {
                registrosReasignacion =
                    repositorio
                    .ListarTodos<UsuarioReasignacion>();

                if (!registrosReasignacion.Any())
                {
                    Logger.Log.Info("No hay registros en la tabla UsuarioReasignacion para procesar.");
                    return "SinRegistros";
                }

                //En caso de registros con ID duplicados
                registrosAgrupados = registrosReasignacion
                .GroupBy(r => r.Usuario_Id)
                .ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error en la obtención de registros(Tabla UsuarioReasignacion) del proceso de reasignación: " + ex.Message, ex);
                throw;
            }

            HashSet<int> userIdsEnRango = new HashSet<int>();
            HashSet<int> userIdsVencidos = new HashSet<int>();
            HashSet<int> userIdsFuturos = new HashSet<int>();
            HashSet<UsuarioReasignacion> registrosVencidos = new HashSet<UsuarioReasignacion>();
            foreach (IGrouping<int, UsuarioReasignacion> registro in registrosAgrupados)
            {
                var registroAProcesar = registro.OrderByDescending(r => r.Id).First();
                if (registroAProcesar.FechaDesde.Date > DateTime.Today.Date)
                {
                    _ = userIdsFuturos.Add(registroAProcesar.Usuario_Id);
                }
                else if (registroAProcesar.FechaHasta.Date >= DateTime.Today.Date)
                {
                    _ = userIdsEnRango.Add(registroAProcesar.Usuario_Id);
                }
                else if (registroAProcesar.FechaHasta.Date < DateTime.Today.Date)
                {
                    _ = userIdsVencidos.Add(registroAProcesar.Usuario_Id);
                    registrosVencidos.UnionWith(registro);
                }
            }

            IEnumerable<Usuario> usuariosEnRango;
            IEnumerable<Usuario> usuariosVencidos;
            IEnumerable<Usuario> usuariosFuturos;
            try
            {
                usuariosEnRango = repositorio.Listar<Usuario>(x => userIdsEnRango.Contains(x.Id));
                usuariosVencidos = repositorio.Listar<Usuario>(x => userIdsVencidos.Contains(x.Id));
                usuariosFuturos = repositorio.Listar<Usuario>(x => userIdsFuturos.Contains(x.Id));
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error en la obtención de usuarios(Tabla Usuarios) del proceso de reasignación: " + ex.Message, ex);
                throw;
            }

            try
            {
                ReasignarAprobacionesEnRango(usuariosEnRango);

                RevertirAsignacionesFueraDeRango(usuariosVencidos.Union(usuariosFuturos));

                // Adicionalmente, remover los registros vencidos
                foreach (var registro in registrosVencidos)
                {
                    repositorio.Remover(registro);
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error en el procesamiento de registros del proceso de reasignación: " + ex.Message, ex);
                throw;
            }

            return "Éxito";
        }

        /// <summary>
        /// Reasigna las aprobaciones de los usuarios que se encuentren en el rango de fechas de reasignación.
        /// </summary>
        private void ReasignarAprobacionesEnRango(IEnumerable<Usuario> usuariosAReasignar)
        {
            string reasignarASuplente(Usuario usuario, Aprobaciones ap)
            {
                var suplenteMail = ResolverSuplenteReasignacion(usuario, usuariosAReasignar);

                if (suplenteMail == null || ap.Aprobador_CDS == suplenteMail)
                {
                    // No se debe hacer el cambio. Se devuelve null para evitar que se envíe la notificación de reasignación
                    return null;
                }
                ap.Aprobador_CDS = suplenteMail;
                ap.Suplente = usuario.Mail;
                return ap.NRO_ES_LOCAL;
            }

            ReasignarAprobaciones(usuariosAReasignar, reasignarASuplente);
        }

        /// <summary>
        /// Revierte las asignaciones en las aprobaciones de los usuarios que se encuentren fuera del rango de reasignación.
        /// </summary>
        private void RevertirAsignacionesFueraDeRango(IEnumerable<Usuario> usuariosARevertir)
        {
            string reasignarAAprobadorOriginal(Usuario usuario, Aprobaciones ap)
            {
                if (string.IsNullOrWhiteSpace(usuario.Suplente))
                {
                    // No es posible hacer el cambio
                    return null;
                }
                if (ap.Aprobador_CDS == usuario.Mail)
                {
                    // No se debe hacer el cambio. Se devuelve null para evitar que se envíe la notificación de reasignación
                    return null;
                }
                ap.Aprobador_CDS = usuario.Mail;
                ap.Suplente = null;
                return ap.NRO_ES_LOCAL;
            }

            ReasignarAprobaciones(usuariosARevertir, reasignarAAprobadorOriginal);
        }

        /// <summary>
        /// Método encargado de reasignar las aprobaciones a los suplentes correspondientes.
        /// </summary>
        /// <param name="usuarios">Usuarios para los cuales hay que encontrar Aprobaciones a reasignar</param>
        /// <param name="transformacion">Instrucciones de reasignación. Debe retornar el número de ES (NRO_ES_LOCAL)</param>
        private void ReasignarAprobaciones(IEnumerable<Usuario> usuarios, Func<Usuario, Aprobaciones, string> transformacion)
        {
            HashSet<string> esLocalModificada = new HashSet<string>();
            foreach (Usuario user in usuarios)
            {
                if (!string.IsNullOrEmpty(user.Mail) && user.Mail.Contains("@"))
                {
                    //Obtener aprobaciones donde el usuario sea aprobador Y fiscal.
                    IEnumerable<Aprobaciones> aprobacionesAsociadas =
                        repositorio
                        .Listar<Aprobaciones>(x => x.Fiscal_SOLPED == user.Mail && x.Estado_certificacion == "Pendiente Aprobación");

                    if (aprobacionesAsociadas.Any())
                    {
                        Logger.Log.Info("ES pendientes de aprobación han sido derivadas a sus suplentes por fecha de reasignación vigente");
                    }

                    foreach (var ap in aprobacionesAsociadas)
                    {
                        string esLocal = transformacion(user, ap);
                        if (!string.IsNullOrEmpty(esLocal))
                        {
                            _ = esLocalModificada.Add(esLocal);
                        }
                    }
                }
            }
            if (esLocalModificada.Count > 0)
            {
                repositorio.GuardarCambios();
                Notificar(esLocalModificada);
            }
        }

        private string ResolverSuplenteReasignacion(Usuario usuario, IEnumerable<Usuario> usuariosEnRangoReasignacion, HashSet<string> suplentesYaEvaluados = null)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Suplente))
                {
                    Logger.Log.Info($"Error: reasignación de usuario sin suplente ({usuario.Mail})");
                    return null;
                }

                suplentesYaEvaluados = suplentesYaEvaluados ?? new HashSet<string>();
                if (suplentesYaEvaluados.Contains(usuario.Suplente))
                {
                    Logger.Log.Info($"Error: Referencia circular al intentar resolver suplente: {string.Join(", ", suplentesYaEvaluados)}");
                    return null;
                }
                else
                {
                    suplentesYaEvaluados.Add(usuario.Suplente);
                }

                var siguienteSuplente = usuariosEnRangoReasignacion.FirstOrDefault(x => x.Mail.Equals(usuario.Suplente, StringComparison.OrdinalIgnoreCase));
                if (siguienteSuplente != null)
                {
                    return ResolverSuplenteReasignacion(siguienteSuplente, usuariosEnRangoReasignacion, suplentesYaEvaluados);
                }
                else
                {
                    return usuario.Suplente;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error al resolver suplente de reasignación para el usuario {(usuario != null ? usuario.Mail : "NULO")}", ex);
                return null;
            }
        }

        private void Notificar(IEnumerable<string> eSLocalesInicio)
        {
            try
            {
                Task.Run(() => entradaServicioService.NotificarReasignaciones(eSLocalesInicio)).Wait();
                Logger.Log.Info("Notificaciones de reasignación a suplente enviadas");
            }
            catch (Exception ex)
            {
                Logger.Log.Info("Error en la notificación de reasignaciones del proceso de reasignación: " + ex.Message);
            }
        }

        /// <summary>
        /// Chequea si el usuario esta autorizado para correr el proceso.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool isUserAllowed(string mail)
        {
            Usuario user = repositorio.Listar<Usuario>(x => x.Mail == mail).FirstOrDefault();

            if (user != null)
            {
                return user.TieneRol(RolEnum.Administracion);
            }

            return false;
        }

        public bool isUserAllowedDerivacion(string mail)
        {
            Usuario user = repositorio.Listar<Usuario>(x => x.Mail == mail).FirstOrDefault();

            if (user != null)
            {
                return user.TienePermiso(PermisoEnum.VerTodosLosEstadosDeES) && user.TienePermiso(PermisoEnum.CertificacionDeServicios); 
            }

            return false;
        }
    }
}
