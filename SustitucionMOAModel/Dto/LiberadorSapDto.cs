using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class LiberadorSapDto
    {
        public int Id { get; set; }

        public string Mail { get; set; }

        public string NombreCompleto { get; set; }

        public bool Obligatorio { get; set; }

        public bool Habilitado { get; set; }

        public string Cargo { get; set; }

        public int LiberadorSapTipo_Id { get; set; }

        public LiberadorSapTipoDto LiberadorSapTipo { get; set; }

        public LiberadorSapDto() { }
        public LiberadorSapDto(LiberadorSap entidad)
        {
            Id = entidad.Id;
            Mail = entidad.Mail;
            NombreCompleto = entidad.NombreCompleto; 
            Obligatorio = entidad.Obligatorio; 
            Habilitado = entidad.Habilitado; 
            Cargo = entidad.Cargo; 
            LiberadorSapTipo_Id = entidad.LiberadorSapTipo_Id;
            LiberadorSapTipo = entidad.LiberadorSapTipo != null ? new LiberadorSapTipoDto(entidad.LiberadorSapTipo) : new LiberadorSapTipoDto();
        }
    }

    public class LiberadorSapTipoDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public LiberadorSapTipoDto() { }
        public LiberadorSapTipoDto(LiberadorSapTipo entidad) { Id = entidad.Id; Nombre = entidad.Nombre; }
    }

    public class LiberadorSapSolpDto
    {
        public int Id { get; set; }

        public int Solp_Id { get; set; }

        public int LiberadorSap_Id { get; set; }

        public LiberadorSapSolpDto() { }
        public LiberadorSapSolpDto(LiberadorSapSolp entidad)
        {
            Id = entidad.Id;
            Solp_Id = entidad.Solp_Id;
            LiberadorSap_Id = entidad.LiberadorSap_Id;
        }
    }
}
