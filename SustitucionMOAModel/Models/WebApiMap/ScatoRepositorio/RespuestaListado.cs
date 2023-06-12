namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    public class RespuestaListado<T> : RespuestaScatoBase
    {
        public T[] Data { get; set; }
    }
}
