using SustitucionMOAModel.Entities;
using System;

namespace SustitucionMOAModel.Dto
{
    public class UsuarioReasignacionDto
    {
        public UsuarioReasignacionDto() { }

        public UsuarioReasignacionDto(UsuarioReasignacion reasignacion)
        {
            Id = reasignacion.Id;
            Usuario_Id = reasignacion.Usuario_Id;
            FechaDesde = reasignacion.FechaDesde;
            FechaHasta = reasignacion.FechaHasta;
        }

        public int Id { get; set; }

        public int Usuario_Id { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
