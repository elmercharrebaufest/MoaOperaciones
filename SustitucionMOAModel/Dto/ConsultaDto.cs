using Entities = SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ConsultaDto
    {
        public int Id { get; set; }
        public string CodigoCorredor { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string CodigoProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public int CategoriaId { get; set; }
        public int? SubCategoriaId { get; set; }
        public int? CausaConsultaId { get; set; }
        public string Asunto { get; set; }
        public int EstadoConsultaId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public int UsuarioId { get; set; }
        public int? UsuarioInternoId { get; set; }
        public DateTime? FechaVtoReapertura { get; set; }
        public int UsuarioActualId { get; set; }
        public string Material { get; set; }
        public int? Material_Id { get; set; }
        //detalle
        public DateTime? Fecha { get; set; }
        public string ComprobanteNo { get; set; }
        public string OtroComprobanteNo { get; set; }
        public string ContratoNo { get; set; }
        public Decimal? Importe { get; set; }
        public string Impuesto { get; set; }
        public string BolsaEmisoraOblea { get; set; }
        public int? OrdenId { get; set; }
        public string PatenteChasis { get; set; }
        public string Rubro { get; set; }
        public bool RelacionadaPorCodigo { get; set; }
        public bool GeneradaInternamente { get; set; }
        public bool GeneradaExternamente { get; set; }
        public bool GeneradaPorUsuarioSesion { get; set; }
        public string MailUsuarioIniciaConsulta { get; set; }

        public CategoriaDto Categoria { get; set; }
        public SubCategoriaDto SubCategoria { get; set; }
        public EstadoConsultaDto EstadoConsulta { get; set; }
        public CausaConsultaDto CausaConsulta { get; set; }
        public UsuarioDto Usuario { get; set; }
        public IList<ComentarioDto> Comentarios { get; set; }

        public int? DiasReclamo { 
            get {
                if (this.EstadoConsulta != null && this.EstadoConsulta.Code == EstadosConsulta.Finalizado.Code())
                    return (FechaUltimaModificacion - FechaCreacion).Days;

                return (DateTime.Now - FechaCreacion).Days;
            } 
        }

        public bool PuedeReabrir
        {
            get
            {
                if (this.EstadoConsulta.Code == EstadosConsulta.Finalizado.Code() && this.UsuarioInternoId == null
                    && this.FechaVtoReapertura != null && DateTime.Now < this.FechaVtoReapertura)
                {
                    return true;
                }
                else
                {
                    return false;
                }         
            }
        }

        public ConsultaDto() { }

        public ConsultaDto(Entities.Consulta consulta)
        {
            this.Id = consulta.Id;
            this.Asunto = consulta.Asunto;
            this.CodigoCorredor = consulta.CodigoCorredor;
            this.RazonSocialCorredor = consulta.RazonSocialCorredor;
            this.CodigoProveedor = consulta.CodigoProveedor;
            this.RazonSocialProveedor = consulta.RazonSocialProveedor;
            this.CategoriaId = consulta.Categoria_Id;
            this.Categoria = new CategoriaDto(consulta.Categoria);
            if(consulta.SubCategoria != null)
            {
                this.SubCategoriaId = consulta.SubCategoria_Id;
                this.SubCategoria = new SubCategoriaDto(consulta.SubCategoria);
            }
            else
            {
                this.SubCategoriaId = 0;
                this.SubCategoria = new SubCategoriaDto { Nombre = "" };
            }
            this.EstadoConsultaId = consulta.EstadoConsulta_Id;
            this.EstadoConsulta = new EstadoConsultaDto(consulta.EstadoConsulta);
            this.FechaCreacion = consulta.FechaCreacion;
            this.FechaUltimaModificacion = consulta.FechaUltimaModificacion;
            this.UsuarioId = consulta.Usuario_Id;
            this.UsuarioInternoId = consulta.UsuarioInterno_Id;
            this.FechaVtoReapertura = consulta.FechaVtoReapertura;
            this.Usuario = new UsuarioDto(consulta.Usuario);
            if (consulta.Detalle != null) {
                this.Fecha = consulta.Detalle.Fecha;
                this.ComprobanteNo = consulta.Detalle.ComprobanteNo;
                this.OtroComprobanteNo = consulta.Detalle.OtroComprobanteNo;
                this.ContratoNo = consulta.Detalle.ContratoNo;
                this.Importe = consulta.Detalle.Importe;
                this.Impuesto = consulta.Detalle.Impuesto;
                this.BolsaEmisoraOblea = consulta.Detalle.BolsaEmisoraOblea;
                this.OrdenId = consulta.Detalle.Orden_Id;
               
                if(consulta.Detalle.CausaConsulta != null)
                {
                    this.CausaConsultaId = consulta.Id;
                    this.CausaConsulta = new CausaConsultaDto(consulta.Detalle.CausaConsulta);
                }
       
            }
        }
    }
}
