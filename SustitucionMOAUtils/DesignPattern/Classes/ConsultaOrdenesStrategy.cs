using Quartz.Util;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAUtils.Logger;
using System.IO;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Helpers;
using DocumentFormat.OpenXml.Math;
using SustitucionMOAUtils.Services;
using System.Web;
using System.Configuration;
using SustitucionMOAModel.CustomExceptions;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaOrdenesStrategy : ConsultaCommon, IConsultaStrategy
    {
        public List<string> Names => new List<string>() {"Orden de Carga"};
        private readonly IUsuarioService usuarioService;

        public ConsultaOrdenesStrategy(IRepositorio repositorio, IEmailService emailService, IUsuarioService usuarioService) : base(repositorio, emailService)
        {
            this.usuarioService = usuarioService;
        }
        public Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files)
        {
            try
            {
                List<Consulta> consultas = new List<Consulta>();

                var destinatarioCliente = destinatarios.FirstOrDefault(c => c.Campo == "Cliente");
                var destinatarioUsuario = destinatarios.FirstOrDefault(u => u.Campo == "Usuario creador");

                if (destinatarioUsuario != null)
                {
                    var consultaAUsuario = AgregarConsultaCreadorOrdenFAS(consulta, comentario);
                    this.CompletarCamposYClonar(consultaAUsuario, comentario);
                    this.repositorio.Agregar(consultaAUsuario);
                    consultas.Add(consultaAUsuario);
                }

                if (destinatarioCliente != null)
                {
                    var consultaACliente = AgregarConsultaClienteFAS(consulta, comentario);
                    this.CompletarCamposYClonar(consultaACliente, comentario);
                    this.repositorio.Agregar(consultaACliente);
                    consultas.Add(consultaACliente);
                }

                this.repositorio.GuardarCambios();

                foreach (Consulta c in consultas)
                {
                    if (files.Count > 0)
                    {
                        GuardarAdjuntoComentario(c.Id, files);
                    }
                    this.EnviarMail(c, comentario, files, destinatarios);
                }

                return consultas.First();
            }
            catch (Exception e)
            {
                Log.Error("Ha ocurrido un error al intentar generar la consulta.", e);
                throw e;
            }
        }

        public void EnviarMail(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<DestinatarioDto> destinatarios)
        {
            var destinatariosCC = this.emailService.ObtenerListaDestinatarios(new string[] { consultaInternaCC });
            var destinatario = destinatarios.FirstOrDefault(u => u.UsuarioId == consulta.Usuario_Id);
            this.EnviarMailInterno(consulta, comentario, files, destinatariosCC, destinatario.Mail);       
        }

        private Consulta AgregarConsultaClienteFAS(Consulta consulta, Comentario comentario)
        {
            Consulta consultaCliente = (Consulta)consulta.Clone();
            OrdenDeCarga orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == consulta.Detalle.Orden_Id);
            Proveedor proveedor = this.repositorio.Obtener<Proveedor>(p => p.Id == orden.Cliente_Id);
            Usuario usuarioCliente = proveedor.UsuariosAsociados.FirstOrDefault(u => u.CUITRegistro == proveedor.CUIT &&
            u.Mail == proveedor.Mail && u.TipoUsuario.Id == proveedor.TipoProveedor.Id);

            if (proveedor == null)
            {
                throw new ValidationCustomException("Hubo un problema al intentar obtener datos del proveedor.");
            }

            if (usuarioCliente == null)
            {
                throw new ValidationCustomException("El cliente asociado a la orden no existe en el sistema");
            }

            consultaCliente.Usuario_Id = usuarioCliente.Id;
            consultaCliente.EstadoConsulta_Id = 4;
            consultaCliente.CodigoProveedor = proveedor.CodigoProveedor ?? "-";
            consultaCliente.RazonSocialProveedor = proveedor.RazonSocial;
            comentario.Usuario_Id = (int)consultaCliente.UsuarioInterno_Id;
            return consultaCliente;
        }

        private Consulta AgregarConsultaCreadorOrdenFAS(Consulta consulta, Comentario comentario)
        {
            Consulta consultaCreador = (Consulta)consulta.Clone();

            OrdenDeCarga orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == consulta.Detalle.Orden_Id);
            Usuario usuario = this.repositorio.Obtener<Usuario>(u => u.Id == orden.UsuarioCreacion_Id);
            Proveedor proveedor = usuario.Proveedores.FirstOrDefault(p => p.Mail == usuario.Mail && p.CUIT == usuario.CUITRegistro);

            if(proveedor == null)
            {
                throw new ValidationCustomException("Hubo un problema al intentar obtener datos del vendedor.");
            }

            consultaCreador.Usuario_Id = orden.UsuarioCreacion_Id;
            consultaCreador.EstadoConsulta_Id = 4;
            consultaCreador.CodigoProveedor = proveedor.CodigoProveedor ?? "-";
            consultaCreador.RazonSocialProveedor = proveedor.RazonSocial;
            comentario.Usuario_Id = (int)consultaCreador.UsuarioInterno_Id;
            return consultaCreador;
        }

        private void CompletarCamposYClonar(Consulta consulta, Comentario comentario)
        {
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);

            comentario.ComentarioRecordado = new List<ComentarioRecordado>();

            if (consulta.Comentarios == null)
            {
                consulta.Comentarios = new List<Comentario>();
            }

            consulta.Comentarios.Add(comentario.Clone() as Comentario);

            consulta.Detalle = consulta.Detalle.Clone() as ConsultaDetalle;

        }

    }
}
