using NLog.Internal;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class TimerNotificarAprobacionesService : ITimerNotificarAprobacionesService
    {
        private Timer _timer;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1);
        private readonly object _lock = new object();
        private bool _isRunning = false;
        private int _retryCount = 0;
        private readonly int _maxRetries = 1000;
        private readonly int _baseDelay = 2000;
        private readonly int _maxDelay = 30000;

        protected IRepositorio repositorio;
        private readonly IAprobacionesService _aprobacionesService;


        public TimerNotificarAprobacionesService(IAprobacionesService aprobacionesService, IRepositorio repositorio)
        {
            _aprobacionesService = aprobacionesService;
            this.repositorio = repositorio;

            string startHour = System.Configuration.ConfigurationManager.AppSettings["TimerNotification"];
            TimeSpan startTime = TimeSpan.Parse(startHour);

            TimeSpan initialDelay = CalculateInitialDelay(startTime);
            
            /*Para prueba interna*/
            //TimeSpan initialDelay = CalculateInitialDelay(new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes + 1, DateTime.Now.TimeOfDay.Seconds));

            TimerCallback callback = new TimerCallback(Notificar);

            _timer = new Timer(callback, null, initialDelay, _interval);
        }

        private void Notificar(object state)
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    try
                    {
                        _aprobacionesService.Notificar();
                        _retryCount = 0;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error al notificar: {ex.Message}");

                        _retryCount++;
                        if (_retryCount <= _maxRetries)
                        {
                            int delay = Math.Min(_baseDelay * (int)Math.Pow(2, _retryCount), _maxDelay);

                            if (delay < 0 || delay > Int32.MaxValue)
                            {
                                delay = _maxDelay; 
                            }

                            Log.Info($"Reintentando en {delay / 1000} segundos. Intento {_retryCount} de {_maxRetries}");

                            // Reintentar
                            TimerCallback callback = new TimerCallback(Notificar);
                            Timer retryTimer = new Timer(callback, null, delay, Timeout.Infinite);
                        }
                        else
                        {
                            Log.Error("Número máximo de reintentos alcanzado.");
                        }
                    }
                    finally
                    {
                        _isRunning = false;
                        Log.Info("Notificaciones ES pendientes enviadas.");
                    }
                }
            }
        }


        private TimeSpan CalculateInitialDelay(TimeSpan targetTime)
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            if (currentTime > targetTime)
            {
                // Target time -> next day
                var time = targetTime.Add(new TimeSpan(24, 0, 0)) - currentTime;
                return targetTime.Add(new TimeSpan(24, 0, 0)) - currentTime;
            }
            else
            {
                // Target time -> today
                var time = targetTime - currentTime;
                return targetTime - currentTime;
            }
        }

    }
}
