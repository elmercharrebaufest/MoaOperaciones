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

            List<UsuarioReasignacion> registrosSinDuplicados;

            try
            {
                IEnumerable<UsuarioReasignacion> registrosReasignacion =
                    repositorio
                    .ListarTodos<UsuarioReasignacion>()
                    .Where(x => x.FechaDesde <= DateTime.Today);

                if (!registrosReasignacion.Any())
                {
                    Logger.Log.Info("No hay registros en la tabla UsuarioReasignacion para procesar.");
                    return "SinRegistros";
                }

                //En caso de registros con ID duplicados
                registrosSinDuplicados = registrosReasignacion
                .GroupBy(r => r.Usuario_Id)
                .Select(g => g.OrderByDescending(r => r.Id).First())
                .ToList();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error en la obtención de registros(Tabla UsuarioReasignacion) del proceso de reasignación: " + ex.Message, ex);
                throw;
            }

            HashSet<int> userIdsEnRango = new HashSet<int>();
            HashSet<int> userIdsVencidos = new HashSet<int>();
            List<UsuarioReasignacion> registrosVencidos = new List<UsuarioReasignacion>(registrosSinDuplicados.Count); // Inicializar con la cantidad de registros para evitar redimensionamiento
            foreach (var registro in registrosSinDuplicados)
            {
                if (registro.FechaHasta.Date >= DateTime.Today.Date)
                {
                    _ = userIdsEnRango.Add(registro.Usuario_Id);
                }

                if (registro.FechaHasta.Date < DateTime.Today.Date)
                {
                    userIdsVencidos.Add(registro.Usuario_Id);
                    registrosVencidos.Add(registro);
                }
            }

            IEnumerable<Usuario> usuariosEnRango;
            IEnumerable<Usuario> usuariosVencidos;
            try
            {
                usuariosEnRango = repositorio.Listar<Usuario>(x => userIdsEnRango.Contains(x.Id));
                usuariosVencidos = repositorio.Listar<Usuario>(x => userIdsVencidos.Contains(x.Id));
            }
            catch (Exception ex)
            {
                Logger.Log.Info("Error en la obtención de usuarios(Tabla Usuarios) del proceso de reasignación: " + ex.Message);
                throw;
            }
            try
            {
                // para los usuarios en rango, ejecutar la reasignación
                HashSet<string> eSLocalesUsuarioEnRango = new HashSet<string>();
                foreach (Usuario user in usuariosEnRango)
                {
                    if (!string.IsNullOrEmpty(user.Mail) && user.Mail.Contains("@")
                        && !string.IsNullOrEmpty(user.Suplente))
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
                            ap.Aprobador_CDS = user.Suplente;
                            ap.Suplente = user.Mail;
                            if (!string.IsNullOrEmpty(ap.NRO_ES_LOCAL))
                            {
                                _ = eSLocalesUsuarioEnRango.Add(ap.NRO_ES_LOCAL);
                            }
                        }
                    }
                }

                //Si hubo cambios, guardar y notificar
                if (eSLocalesUsuarioEnRango.Count > 0)
                {
                    repositorio.GuardarCambios();

                    Notificar(eSLocalesUsuarioEnRango);
                }

                // Para los usuarios vencidos, revertir la reasignación
                HashSet<string> eSLocalesUsuariosVencidos = new HashSet<string>();
                foreach (string mail in usuariosVencidos.Select(user => user.Mail))
                {
                    if (!string.IsNullOrEmpty(mail) && mail.Contains("@"))
                    {
                        IEnumerable<Aprobaciones> aprobacionesAsociadas =
                            repositorio
                            .Listar<Aprobaciones>(x => x.Fiscal_SOLPED == mail && x.Estado_certificacion == "Pendiente Aprobación");

                        if (aprobacionesAsociadas.Any())
                        {
                            Logger.Log.Info("ES pendientes de aprobación han sido derivadas a sus fiscales por fecha de reasignación vencida");
                        }

                        foreach (var ap in aprobacionesAsociadas)
                        {
                            ap.Aprobador_CDS = mail;
                            ap.Suplente = null;
                            if (!string.IsNullOrEmpty(ap.NRO_ES_LOCAL))
                            {
                                _ = eSLocalesUsuariosVencidos.Add(ap.NRO_ES_LOCAL);
                            }
                        }
                    }
                }

                //Si hubo cambios, guardar y notificar
                if (eSLocalesUsuariosVencidos.Count > 0)
                {
                    repositorio.GuardarCambios();

                    Notificar(eSLocalesUsuariosVencidos);
                }

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
