using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOA.Jobs
{
	public interface IEnviarASAPOrdenDeCargaJob : IHangfireJob { }

	public class EnviarASAPOrdenDeCargaJob : IEnviarASAPOrdenDeCargaJob
	{
		private readonly IOrdenDeCargaService _ordenDeCargaService;

		public EnviarASAPOrdenDeCargaJob(IOrdenDeCargaService ordenDeCargaService)
		{
			_ordenDeCargaService = ordenDeCargaService;
		}

		public void Execute()
		{
			try
			{
				_ordenDeCargaService.CrearOrdenEnSAPBulk();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}