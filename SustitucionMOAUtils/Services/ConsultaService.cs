using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        public ComentarioDto AgregarComentario(int consultaId, Comentario comentario)
        {
            var consulta = GetConsulta(consultaId);

            consulta.Comentarios.Add(comentario);
            repositorio.GuardarCambios();

            return new ComentarioDto(comentario);
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

        public string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files)
        {
            var comentario = repositorio.Obtener<Comentario>(comentarioId);

            if (comentario == null) throw new InfoCustomException("No existe el comentario");
            if (comentario.Consulta_Id != consultaId) throw new InfoCustomException("El comentario no corresponde a la consulta especificada");

            var errores = new List<string>();
            var proveedor = repositorio.Obtener<Consulta>(c => c.Id == consultaId).Proveedor;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file.FileName);
                var ruta = $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{proveedor.CUIT}/{proveedor.Id}/{FileKeys.Consultas}/{consultaId}";
                var rutaArchivo = string.Concat(ruta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    errores.Add($"{fileName}: {ErrorMsg.ErrorArchivoRepetido}");
                    continue;
                }

                Directory.CreateDirectory(ruta);

                proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.Consultas, Ruta = rutaArchivo });

                file.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();
            }

            return errores.Any() ? string.Join(".", errores) : SuccessMsg.ArchivoSubidoOK;
        }
    }
}
