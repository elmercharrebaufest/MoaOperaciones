using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.Curso
{
    public class AsignarReqDto
    {
        public List<string> MailsUsuarios { get; set; }
        public int CursoId { get; set; }
    }
}
