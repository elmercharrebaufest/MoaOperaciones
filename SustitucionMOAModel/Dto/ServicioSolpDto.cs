using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class ServicioSolpDto
    {
        public ServicioSolpDto(ServicioSolp servicioSolp)
        {
            Id = servicioSolp.Id;
            Codigo = servicioSolp.Codigo;
            Descripcion= servicioSolp.Descripcion;
            GrupoArticulos = servicioSolp.GrupoArticulos;
            TipoServicio = servicioSolp.TipoServicio;
            AmbitoServicio = servicioSolp.AmbitoServicio;
            Edicion = servicioSolp.Edicion;
            UnidadMedidaBase = servicioSolp.UnidadMedidaBase;
            SSCItem = servicioSolp.SSCItem;
        }

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