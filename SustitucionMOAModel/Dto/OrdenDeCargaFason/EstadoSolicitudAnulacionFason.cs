namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class EstadoSolicitudAnulacionFason : EstadoSolicitudOrdenDeCargaBaseDto
    {
        public EstadoSolicitudAnulacionFason(
            int ordenId,
            string mailUsuario,
            bool aprobado) : base(ordenId, mailUsuario, aprobado)
        {
        }
    }
}
