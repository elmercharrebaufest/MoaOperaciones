namespace SustitucionMOAModel.Enums
{
	public enum EstadoOrdenDeCargaFason
	{
		Generada, 
		Pendiente, 
		Vencida, 
		Entregada,
        SinEstado,
		PendienteCompensacion
	}

	public static class EstadoOrdenDeCargaFasonExtensions
	{
		public static string ToFriendlyString(this EstadoOrdenDeCargaFason me)
		{
			switch (me)
			{
				case EstadoOrdenDeCargaFason.Generada:
					return "Orden generada";
				case EstadoOrdenDeCargaFason.Pendiente:
					return "Pendiente";
				case EstadoOrdenDeCargaFason.Vencida:
					return "Orden vencida";
				case EstadoOrdenDeCargaFason.Entregada:
					return "Orden entregada";
				default:
					return "Sin estado";
			}
		}

        public static string ObtenerSemaforo(this EstadoOrdenDeCargaFason me)
        {
            switch (me)
            {
				case EstadoOrdenDeCargaFason.Vencida:
					return "red";                           
              	case EstadoOrdenDeCargaFason.Pendiente:
                    return "yellow";
                case EstadoOrdenDeCargaFason.Generada:
                    return "green";                             
                default:
				case EstadoOrdenDeCargaFason.Entregada:
					return "white";
            }
        }

		public static string ToUserFriendlyString(this EstadoOrdenDeCargaFason me)
		{
			switch (me)
			{
				case EstadoOrdenDeCargaFason.Generada:
				case EstadoOrdenDeCargaFason.Pendiente:
					return "OK";
				case EstadoOrdenDeCargaFason.Vencida:
					return "Orden vencida";
				case EstadoOrdenDeCargaFason.Entregada:
					return "Orden entregada";
				default:
					return "Sin estado";
			}
		}

        public static EstadoOrdenDeCargaFason ObtenerDescripcionEstado(string estado)
        {

            switch (estado)
            {
                case "Orden generada":
                    return EstadoOrdenDeCargaFason.Generada;
				case "Pendiente":
					return EstadoOrdenDeCargaFason.Pendiente;
				case "Orden vencida":
					return EstadoOrdenDeCargaFason.Vencida;
				case "Orden entregada":
					return EstadoOrdenDeCargaFason.Entregada;
				default:
					return EstadoOrdenDeCargaFason.SinEstado;
			}
        }
    }
}
