using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class ServicioSolpDto
    {
        //public ServicioSolpDto(object servicioSolp) {}
        public ServicioSolpDto(ServicioSolp servicioSolp)
        {
            if (servicioSolp != null)
            {

                Id = servicioSolp.Id;
                Codigo = servicioSolp.CodigoSap;
                Descripcion = servicioSolp.Descripcion;
                GrupoArticulos = servicioSolp.GrupoArticulos;
                TipoServicio = servicioSolp.TipoServicio;
                AmbitoServicio = servicioSolp.AmbitoServicio;
                Edicion = servicioSolp.Edicion;
                UnidadMedidaBase = servicioSolp.UnidadMedidaBase;
                SSCItem = servicioSolp.SSCItem;

            }


        }
        public ServicioSolpDto() { }

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