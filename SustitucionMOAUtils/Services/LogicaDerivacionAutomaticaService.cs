using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class LogicaDerivacionAutomaticaService : ILogicaDerivacionAutomaticaService
    {
        private readonly Func<DbContext> _dbContextFactory;
        protected IRepositorio repositorio;
        protected IEntradaServicioService entradaServicioService;

        /// <summary>
        /// Initial Delay:momento de la primer ejecución
        /// _interval: cada cuanto se corre luego
        /// </summary>
        public LogicaDerivacionAutomaticaService(IRepositorio repositorio, Func<DbContext> context, IEntradaServicioService entradaServicioService)
        {
            this.repositorio = repositorio;
            this._dbContextFactory = context;
            this.entradaServicioService = entradaServicioService;
        }

        /// <summary>
        /// Método encargado del proceso de reasignación automática - MMSN - 1151
        /// Separado del Timer para ser corrido a demanda por los usuarios con el rol Administración
        /// </summary>
        public string CorrerProcesoReasignacion()
        {
            using (var dbContext = _dbContextFactory())
            {
                Logger.Log.Info("Corriendo proceso de reasignación automática");

                dbContext.Database.CreateIfNotExists();

                var repositorio = new RepositorioEF(dbContext);

                List<UsuarioReasignacion> registrosSinDuplicados = new List<UsuarioReasignacion>();

                try
                {
                    List<UsuarioReasignacion> registrosReasignacion = repositorio.ListarTodos<UsuarioReasignacion>().ToList();

                    if (registrosReasignacion.Count == 0)
                    {
                        Logger.Log.Info("No hay registros en la tabla UsuarioReasignacion para procesar.");
                        return "SinRegistros";
                    }

                    //En caso de registros con ID duplicados
                    registrosSinDuplicados = registrosReasignacion
                    .GroupBy(r => r.Usuario_Id)
                    .Select(g => g.OrderByDescending(r => r.FechaHasta).First())
                    .ToList();
                }
                catch (Exception ex)
                {
                    Logger.Log.Info("Error en la obtención de registros(Tabla UsuarioReasignacion) del proceso de reasignación: " + ex.Message);
                    throw ex;
                }


                var today = DateTime.Today;
                var ayer = DateTime.Today.AddDays(-1);
                List<int> userIdsInicio = new List<int>();
                List<int> userIdsFin = new List<int>();
                foreach (var registro in registrosSinDuplicados)
                {
                    // 1 - Ver si el periodo debe procesarse  - Inicio / primer día luego del inicio del periodo
                    //18/09/2024 -> fechaInicio
                    //18/09/2024 -> fechaFin
                    //18/09/2024 -> fechaHoy
                    //17/09/2024 -> fechaAyer
                    if (registro.FechaDesde.Date == today.Date)
                    {
                        userIdsInicio.Add(registro.Usuario_Id);
                    }

                    //Registros fin del periodo - Dia siguiente al final de reasignación

                    if ((today.Date - registro.FechaHasta.Date).TotalDays == 1)
                    {
                        userIdsFin.Add(registro.Usuario_Id);
                    }
                }

                List<Usuario> usuarios = new List<Usuario>();
                List<Usuario> usuariosFin = new List<Usuario>();
                try
                {
                    foreach (int id in userIdsInicio)
                    {
                        Usuario user = repositorio.Listar<Usuario>(x => x.Id == id).ToList().FirstOrDefault();
                        usuarios.Add(user);
                    }


                    foreach (int id in userIdsFin)
                    {
                        Usuario user = repositorio.Listar<Usuario>(x => x.Id == id).ToList().FirstOrDefault();
                        usuariosFin.Add(user);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Info("Error en la obtención de usuarios(Tabla Usuarios) del proceso de reasignación: " + ex.Message);
                    throw ex;

                }
                try
                {
                    List<string> eSLocalesInicio = new List<string>();
                    foreach (Usuario user in usuarios)
                    {
                        string mail = user.Mail;
                        string suplente = user.Suplente;

                        if ((mail != null && mail != "" && mail.Contains("@")) && (suplente != null && suplente != ""))
                        {
                            //Obtener aprobaciones donde el usuario sea aprobador Y fiscal.
                            List<Aprobaciones> aprobacionesAsociadas = repositorio.Listar<Aprobaciones>(x => x.Fiscal_SOLPED == mail && x.Estado_certificacion == "Pendiente Aprobación").ToList();

                            if (aprobacionesAsociadas.Count > 0)
                                Logger.Log.Info("ES pendientes de aprobación han sido derivadas a sus suplentes por fecha de reasignación vigente");

                            foreach (var ap in aprobacionesAsociadas)
                            {
                                ap.Aprobador_CDS = suplente;
                                ap.Suplente = mail;
                                if (!string.IsNullOrEmpty(ap.NRO_ES_LOCAL) && !eSLocalesInicio.Contains(ap.NRO_ES_LOCAL))
                                {
                                    eSLocalesInicio.Add(ap.NRO_ES_LOCAL);
                                }
                            }
                        }
                    }

                    //Si hubo cambios, guardar y notificar
                    if (eSLocalesInicio.Count > 0)
                    {
                        repositorio.GuardarCambios();

                        Notificar(repositorio, eSLocalesInicio);
                    }



                    //Para el día siguiente al fin del periodo de reasignación, reasignar las ordenes que tengan como Aprobador al suplente, y fiscal al mail original
                    List<string> eSLocalesFin = new List<string>();
                    foreach (Usuario user in usuariosFin)
                    {
                        string mail = user.Mail;
                        string suplente = user.Suplente;

                        if ((!string.IsNullOrEmpty(mail) && mail.Contains("@")) && (!string.IsNullOrEmpty(suplente)))
                        {
                            List<Aprobaciones> aprobacionesAsociadas = repositorio.Listar<Aprobaciones>(x => x.Fiscal_SOLPED == mail && x.Estado_certificacion == "Pendiente Aprobación").ToList();

                            if (aprobacionesAsociadas.Count > 0)
                                Logger.Log.Info("ES pendientes de aprobación han sido derivadas a sus fiscales por fecha de reasignación vencida");

                            foreach (var ap in aprobacionesAsociadas)
                            {
                                ap.Aprobador_CDS = mail;
                                ap.Suplente = suplente;
                                //No repetir en la lista de emails el mismo NroESLocal
                                if (!string.IsNullOrEmpty(ap.NRO_ES_LOCAL) && !eSLocalesFin.Contains(ap.NRO_ES_LOCAL))
                                {
                                    eSLocalesFin.Add(ap.NRO_ES_LOCAL);
                                }
                            }

                            UsuarioReasignacion registroReasignacion = repositorio.Obtener<UsuarioReasignacion>(x => x.Usuario_Id == user.Id);
                            if (registroReasignacion != null)
                                repositorio.Remover(registroReasignacion);
                        }
                    }

                    //Si hubo cambios, guardar y notificar
                    if (eSLocalesFin.Count > 0)
                    {
                        repositorio.GuardarCambios();

                        Notificar(repositorio, eSLocalesFin);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Log.Info("Error en el procesamiento de registros del proceso de reasignación: " + ex.Message);
                    throw ex;
                }

                return "Exito";
            }
        }

        private void Notificar(RepositorioEF repositorio, List<string> eSLocalesInicio)
        {
            try
            {
                Task.Run(() => entradaServicioService.NotificarReasignaciones(eSLocalesInicio, repositorio)).Wait();
                Logger.Log.Info("Notificaciones de reasignacion a suplente enviadas");

            }
            catch (Exception ex)
            {
                Logger.Log.Info("Error en la notificación de reasignaciones del proceso de reasignación: " + ex.Message);
            }
        }

        /// <summary>
        /// Chequea si el usuario esta autorizado para correr el proceso.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool isUserAllowed(string email)
        {
            bool allowed = false;

            Usuario user = repositorio.Listar<Usuario>(x => x.Mail == email).ToList().FirstOrDefault();

            if (user != null)
            {
                allowed = user.TieneRol(RolEnum.Administracion);
            }

            return allowed;
        }
    }
}
