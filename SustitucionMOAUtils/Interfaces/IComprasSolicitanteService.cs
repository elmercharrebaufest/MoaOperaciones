using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasSolicitanteService
    {
        ListaPaginada<SolpDto> ListarSolp(UsuarioDto usuarioActual, Paginacion paginacion, string nroSolp, string nombrePedido, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, bool contratoMarco, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null);

        DatosUltimaSolpDto ObtenerUltimaSolp(int usuarioId);

        List<UsuarioDto> ListarUsuarioSolicitante();

        InfoVisitasDeObraDto ListarVisitasDeObra(List<VisitaObraDto> visitas);

        List<MaterialSolpDto> AutocompleteCodigoMaterialSolp(string valor, int centroId);

        RegistroInfoDto ObtenerUltimoRegistroMaterialConPrecioBase(string material, string centro, string grupoDeCompras);

        List<AsociarContratoDto> DevolverContratosAsociados(List<SolpPosicionDto> posiciones);
    }
}
