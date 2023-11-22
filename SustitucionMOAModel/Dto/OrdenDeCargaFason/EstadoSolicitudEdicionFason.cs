namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class EstadoSolicitudEdicionFason : EstadoSolicitudOrdenDeCargaBaseDto
    {
        public EstadoSolicitudEdicionFason(
            int ordenId,
            string mailUsuario,
            bool aprobado) : base(ordenId, mailUsuario, aprobado)
        {
        }
    }
}
