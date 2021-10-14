namespace SustitucionMOAModel.Entities
{
    public class ServicioSolp
    {
        public int Id { get; set; }
        public int Codigo { get; set; }//SERVICE
        public string Descripcion { get; set; }//SHORT TEXT
        public int? GrupoArticulos { get; set; }//MATL GROUP
        public string TipoServicio { get; set; }//SERV CAT
        public string AmbitoServicio { get; set; }//SERV TYPE
        public int Edicion { get; set; }//EDITION
        public string UnidadMedidaBase { get; set; }//BASE UOM
        public string SSCItem { get; set; }//SSCItem
    }
}