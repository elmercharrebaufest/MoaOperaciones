using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IRepositorio repositorio;

        public ConsultaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId)
        {
            var estado = repositorio.Obtener<Consulta>(c => c.Id == estadoConsultaId);

            if (estado == null) throw new InfoCustomException("No existe el estado");

            var consulta = GetConsulta(consultaId);

            consulta.EstadoConsulta_Id = estadoConsultaId;
            repositorio.GuardarCambios();
        }

        public void AgregarComentario(int consultaId, Comentario comentario)
        {
            var consulta = GetConsulta(consultaId);

            consulta.Comentarios.Add(comentario);
            repositorio.GuardarCambios();
        }

        private Consulta GetConsulta(int consultaId)
        {
            var consulta = repositorio.Obtener<Consulta>(consultaId);

            if (consulta == null) throw new InfoCustomException("No existe la consulta");

            return consulta;
        }

        public ConsultaDto ObtenerConsulta(int consultaId)
        {
            return repositorio.Obtener<Consulta, ConsultaDto>(c => c.Id == consultaId, c => new ConsultaDto
            {
                Id = c.Id,
                Asunto = c.Asunto,
                Categoria = new CategoriaDto(c.Categoria),
                Comentarios = c.Comentarios.Select(comentario => new ComentarioDto(comentario)).ToList(),
                Comprobante = c.Comprobante,
                Contrato = c.Contrato,
                CUIT = c.CUIT,
                Email = c.Email,
                EstadoConsulta = new EstadoConsultaDto(c.EstadoConsulta),
                FechaCreacion = c.FechaCreacion,
                FechaPago = c.FechaPago,
                FechaUltimaModificacion = c.FechaUltimaModificacion,
                Importe = c.Importe,
                Impuesto = c.Impuesto,
                Inscripcion = c.Inscripcion,
                Motivo = c.Motivo,
                Nombre = c.Nombre,
                NombreVendedor = c.NombreVendedor,
                RazonSocial = c.RazonSocial,
                Telefono = c.Telefono
            });
        }

        public void RecategorizarConsulta(int consultaId, int categoriaId)
        {
            var categoria = repositorio.Obtener<Categoria>(c => c.Id == categoriaId);

            if(categoria == null) throw new InfoCustomException("No existe la categoria");
            
            var consulta = GetConsulta(consultaId);

            consulta.Categoria_Id = categoriaId;
            repositorio.GuardarCambios();
        }
    }
}
