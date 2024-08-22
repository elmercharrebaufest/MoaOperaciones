using SustitucionMOAModel.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto.Consulta
{
    public class ReqListadoConsultaDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public DirOrden DirOrden { get; set; } = DirOrden.Asc;
        public string FiltrosURIEncoded { get; set; }
        public bool Todos { get; set; } = false;
    }

    public class FiltrosConsultaDto
    {
        public int? Id { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public List<int?> CategoriaId { get; set; }
        public List<int?> SubCategoriaId { get; set; }
        public string Asunto { get; set; }
        public List<int?> EstadoConsultaId { get; set; }
        public List<int?> Material_Id { get; set; }
        public List<DateTime?> FechaCreacion { get; set; }
        public List<DateTime?> FechaUltimaModificacion { get; set; }
        public string DiasReclamo { get; set; }
        public FiltrosConsultaDto() { }
    }
}
