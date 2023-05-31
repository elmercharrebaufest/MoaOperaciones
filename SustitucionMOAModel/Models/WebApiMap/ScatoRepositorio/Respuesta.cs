namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    public class Respuesta<T> : RespuestaScatoBase
    {
        public T Data { get; set; }
    }
}
