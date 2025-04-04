namespace SustitucionMOAModel.Enums
{
    public enum EstadoDocumentoSolp
    {
        Incompleto = 1,
        Creado = 2,
        Finalizado = 3
    }

    public enum TipoSolpSap
    {
        Web = 1,
        Mantenimiento = 2,
        Sap = 3,
        ReposicionAutomatica = 4,
        //ContratoMarco => solp.Posiciones.Any(p => !string.IsNullOrEmpty(p.NumeroContratoSuperior))
    }

    public enum SolpDescargaZipPorLink
    {
        SolpIdNoExiste = 1,
        EmailTokenInvalido = 2,
        PuedeDescargar = 3,
        SinArchivos = 4
    }

    public static class ComprasEnumsExtensions
    {
        public static string Code(this EstadoDocumentoSolp me)
        {
            switch (me)
            {
                case EstadoDocumentoSolp.Incompleto: return "INCOMPLETO";
                case EstadoDocumentoSolp.Creado: return "CREADO";
                case EstadoDocumentoSolp.Finalizado: return "FINALIZADO";
                default:
                    return string.Empty;
            }
        }
        //public static string CodeTipoSolpSap(this TipoSolpSap me)
        //{
        //    switch (me)
        //    {
        //        case TipoSolpSap.Web: return "R";
        //        case TipoSolpSap.Mantenimiento: return "F";
        //        case TipoSolpSap.Sap: return "R";
        //        case TipoSolpSap.ReposicionAutomatica: return "B"; // puede ser B o U
        //        default:
        //            return string.Empty;
        //    }
        //}
    }
}
