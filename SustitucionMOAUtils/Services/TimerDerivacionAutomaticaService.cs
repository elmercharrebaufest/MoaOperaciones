using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using DocumentFormat.OpenXml.Bibliography;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class TimerDerivacionAutomaticaService : ITimerDerivacionAutomaticaService
    {
        private Timer _timer;
        private Timer _timer12;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1); // 24 hs
        //Para Testing - Descomentar lineas y comentar intervals, intervalo de CADA CUANTO deberia correr
        //private readonly TimeSpan _interval = TimeSpan.FromMinutes(7);
        //private readonly TimeSpan _interval12 = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _interval12 = TimeSpan.FromDays(1); // 24 hs
        private readonly object _lock = new object();
        private bool _isRunning = false;
        private readonly ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService;

        /// <summary>
        /// Initial Delay:momento de la primer ejecución
        /// _interval: cada cuanto se corre luego
        /// </summary>
        public TimerDerivacionAutomaticaService(ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService)
        {
            this.logicaDerivacionAutomaticaService = logicaDerivacionAutomaticaService;
            string startFirstHour = System.Configuration.ConfigurationManager.AppSettings["FirstTimerDerivaciones"];
            string startSecondHour = System.Configuration.ConfigurationManager.AppSettings["SecondTimerDerivaciones"];

            TimeSpan firstTime = TimeSpan.Parse(startFirstHour);
            TimeSpan secondTime = TimeSpan.Parse(startSecondHour);

            // Calculate the initial delay
            TimeSpan initialDelay = CalculateInitialDelay(firstTime); // 1:00 AM
            //Para Testing -> Intervalo inicial de cuando levanta la app y deberia correr, descomentar, TimeOfDay.Minutes + 7 = 7 minutos a partir de ahora -> Primer ejecución
            //TimeSpan initialDelay = CalculateInitialDelay(new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes + 2, DateTime.Now.TimeOfDay.Seconds));

            TimerCallback callback = new TimerCallback(ReasignarAutomaticamente);

            _timer = new Timer(callback, null, initialDelay, _interval);

            //Agregado - 2do Timer a las 12
            TimeSpan initialDelay12 = CalculateInitialDelay(secondTime); // 12:00 
            //Para Testing -> Intervalo inicial de cuando levanta la app y deberia correr, descomentar, TimeOfDay.Minutes + 10 = 10 minutos a partir de ahora
            //TimeSpan initialDelay12 = CalculateInitialDelay(new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes + 10, DateTime.Now.TimeOfDay.Seconds));

            TimerCallback callback12 = new TimerCallback(ReasignarAutomaticamente);

            _timer12 = new Timer(callback12, null, initialDelay12, _interval12);
        }


        /// <summary>
        /// Calcular primera corrida y setear horario
        /// </summary>
        /// <param name="targetTime"></param>
        /// <returns></returns>
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

        private void ReasignarAutomaticamente(object state)
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    try
                    {
                        logicaDerivacionAutomaticaService.CorrerProcesoReasignacion();
                    }
                    finally
                    {
                        _isRunning = false;
                    }
                }
            }
        }      
    }
}
