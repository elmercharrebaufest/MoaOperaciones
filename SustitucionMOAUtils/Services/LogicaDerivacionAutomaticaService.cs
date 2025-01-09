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
                string transformacionReasignar(Usuario user, Aprobaciones ap)
                {
                    if (ap.Aprobador_CDS == user.Suplente)
                    {
                        // no se debe hacer el cambio.
                        // se hace para evitar que se envíe la notificación de reasignación
                        return null;
                    }
                    ap.Aprobador_CDS = user.Suplente;
                    ap.Suplente = user.Mail;
                    return ap.NRO_ES_LOCAL;
                }

                string transformacionRevertirAsignacion(Usuario user, Aprobaciones ap)
                {
                    if (string.IsNullOrWhiteSpace(user.Suplente))
                    {
                        //no es posible hacer el cambio
                        return null;
                    }
                    if (ap.Aprobador_CDS == user.Mail)
                    {
                        // no se debe hacer el cambio.
                        // se hace para evitar que se envíe la notificación de reasignación
                        return null;
                    }
                    ap.Aprobador_CDS = user.Mail;
                    ap.Suplente = null;
                    return ap.NRO_ES_LOCAL;
                }

                // Reasignar aprobaciones a los suplentes correspondientes
                ReasignarAprobaciones(usuariosEnRango, transformacionReasignar);

                // Revertir aprobaciones a los aprobadores originales
                ReasignarAprobaciones(usuariosVencidos.Union(usuariosFuturos), transformacionRevertirAsignacion);

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
    }
}
