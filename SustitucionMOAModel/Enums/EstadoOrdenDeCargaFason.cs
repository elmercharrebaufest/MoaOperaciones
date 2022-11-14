namespace SustitucionMOAModel.Enums
{
	public enum EstadoOrdenDeCargaFason
	{
		Generado, 
		Pendiente, 
		Vencida, 
		Entregada
	}

	public static class EstadoOrdenDeCargaFasonExtensions
	{
		public static string ToFriendlyString(this EstadoOrdenDeCargaFason me)
		{
			switch (me)
			{
				case EstadoOrdenDeCargaFason.Generado:
					return "Generada";
				case EstadoOrdenDeCargaFason.Pendiente:
					return "Pendiente";
				case EstadoOrdenDeCargaFason.Vencida:
					return "Vencida";
				case EstadoOrdenDeCargaFason.Entregada:
					return "Entregada";
				default:
					return "Sin estado";
			}
		}
	}
}
