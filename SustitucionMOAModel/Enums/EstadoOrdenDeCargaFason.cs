namespace SustitucionMOAModel.Enums
{
	public enum EstadoOrdenDeCargaFason
	{
		Generada, 
		Pendiente, 
		Vencida, 
		Entregada,
        SinEstado,
		PendienteCompensacion,
		PendienteContabilizacion,
		EdicionSolicitada,
		EdicionRechazada,
		AnulacionSolicitada,
		Anulada
	}

	public static class EstadoOrdenDeCargaFasonExtensions
	{
		public static string ToFriendlyStringInterno(this EstadoOrdenDeCargaFason me)
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
				case EstadoOrdenDeCargaFason.PendienteCompensacion:
					return "Pendiente compensación";
				case EstadoOrdenDeCargaFason.PendienteContabilizacion:
					return "Pendiente contabilizar";
				case EstadoOrdenDeCargaFason.EdicionSolicitada:
					return "Edición solicitada";
				case EstadoOrdenDeCargaFason.EdicionRechazada:
					return "Edición rechazada";
				case EstadoOrdenDeCargaFason.AnulacionSolicitada:
					return "Anulación solicitada";
				case EstadoOrdenDeCargaFason.Anulada:
					return "Anulada";
				default:
					return "Sin estado";
			}
		}

		public static string ToFriendlyStringExterno(this EstadoOrdenDeCargaFason me)
		{
			switch (me)
			{
				case EstadoOrdenDeCargaFason.Generada:
				case EstadoOrdenDeCargaFason.PendienteContabilizacion:
					return "OK";
				case EstadoOrdenDeCargaFason.Pendiente:
				case EstadoOrdenDeCargaFason.PendienteCompensacion:
					return "En proceso";
				case EstadoOrdenDeCargaFason.Vencida:
					return "Orden vencida";
				case EstadoOrdenDeCargaFason.Entregada:
					return "Orden entregada";
				case EstadoOrdenDeCargaFason.EdicionSolicitada:
					return "Edición solicitada";
				case EstadoOrdenDeCargaFason.EdicionRechazada:
					return "Edición rechazada";
				case EstadoOrdenDeCargaFason.AnulacionSolicitada:
					return "Anulación solicitada";
				case EstadoOrdenDeCargaFason.Anulada:
					return "Anulada";

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
				case EstadoOrdenDeCargaFason.Entregada:
					return "white";

				case EstadoOrdenDeCargaFason.PendienteCompensacion:
				case EstadoOrdenDeCargaFason.PendienteContabilizacion:
				case EstadoOrdenDeCargaFason.EdicionSolicitada:
				case EstadoOrdenDeCargaFason.EdicionRechazada:
				case EstadoOrdenDeCargaFason.AnulacionSolicitada:
				case EstadoOrdenDeCargaFason.Anulada:
					return "";

                default:
					throw new System.Exception("Semáforo no mapeado");
			}
		}
		

   //     public static EstadoOrdenDeCargaFason ObtenerDescripcionEstado(string estado)
   //     {

   //         switch (estado)
   //         {
   //             case "Orden generada":
   //                 return EstadoOrdenDeCargaFason.Generada;
			//	case "Pendiente":
			//		return EstadoOrdenDeCargaFason.Pendiente;
			//	case "Orden vencida":
			//		return EstadoOrdenDeCargaFason.Vencida;
			//	case "Orden entregada":
			//		return EstadoOrdenDeCargaFason.Entregada;
			//	default:
			//		return EstadoOrdenDeCargaFason.SinEstado;
			//}
   //     }
    }
}
