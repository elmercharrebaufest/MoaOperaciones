using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.Web.Security;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailComprasService : IEmailComprasService
    {
        private readonly IEmailService emailService;
        private readonly IHttpContextService httpContextService;

        public EmailComprasService(IEmailService emailService, IHttpContextService httpContextService)
        {
            this.emailService = emailService;
            this.httpContextService = httpContextService;
        }

        public void EnviarMailCotizacionCreada(Cotizacion cotizacion)
        {
            try
            {
                var cotizadorRazonSocial = cotizacion.UsuarioCreador.ObtenerProveedor().RazonSocial;
                var peticion = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta;
                var enviarA = new List<string> { peticion.Usuario.Mail };

                var solps = peticion.Posiciones.Select(x => x.SolpPosicion.Solp);
                var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList();
                mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
                var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList();

                enviarA.AddRange(mailPliego);
                enviarA.AddRange(mailCreador);

                Log.Info($"Copia mail solicitante paso 1: {mailPliego.ToJson()}");
                Log.Info($"Copia mail solicitante: {mailCreador.ToJson()}");

                var asunto = $"NUEVA cotización creada por {cotizadorRazonSocial} - SOLPs " +
                    string.Join(", ", peticion.Posiciones.Select(x => x.SolpPosicion.Solp).Select(x => x.NroSolp).Distinct());

                emailService.EnviarMail(enviarA.Distinct().ToList(), asunto, "", null, ObtenerCuerpoCotizacionCreada(cotizacion), null, null, null, null);
            }
            catch (Exception e)
            {
                Log.Error($"Error al enviar mail GrabarCotizacion en cotizacion: " + cotizacion.Id, e);
            }

        }

        public void EnviarMailSolpLiberada(Solp solp)
        {
            try
            {
                Log.Info($"EnviarMailSolpLiberada. Nro de SOLP {solp.NroSolp}. Copia mail comprador: {solp.UsuarioCompras?.Mail}. Copia mail creador: {solp.UsuarioCreacion?.Mail}");

                var copia = new List<string> { };
                if (!string.IsNullOrEmpty(solp.UsuarioCreacion?.Mail))
                {
                    copia.Add(solp.UsuarioCreacion.Mail);
                    Log.Info($"Copia mail solicitante {solp.UsuarioCreacion.Mail}");
                }

                if (!string.IsNullOrEmpty(solp.Pliego.Email))
                {
                    //Mail del solicitante
                    copia.Add(solp.Pliego.Email);
                    Log.Info($"Copia mail solicitante paso 1 {solp.Pliego.Email}");
                }

                if (!string.IsNullOrEmpty(solp.Pliego.SupervisorTrabajo))
                {
                    //Supervisor
                    copia.Add(solp.Pliego.SupervisorTrabajo);
                    Log.Info($"Copia mail responsable de trabajo paso 2 {solp.Pliego.SupervisorTrabajo}");
                }

                var descripcionSolp = !string.IsNullOrEmpty(solp.Pliego?.NombreObra) ? solp.Pliego.NombreObra : solp.Posiciones.First().Tarea;
                var asunto = $"{(solp.TrabajoYaHecho == true ? "Nueva SOLP de trabajo ya hecho liberada" : "Nueva SOLP liberada")}: {solp.NroSolp} - {descripcionSolp}" +
                    (solp.Adicional == true ? $" - con Adicional OC: {solp.NroOrdenDeCompraAdicional}" : "");
                var enviarA = new List<string> { solp.UsuarioCompras.Mail };

                emailService.EnviarMail(enviarA, asunto, "", copia, ObtenerCuerpoSolpLiberada(solp), null, "");
            }
            catch (Exception ex)
            {
                Log.Error($"Error al enviar mail SOLP liberada Nro de SOLP {solp.NroSolp}", ex);
            }
        }

        public void EnviarMailFinalizacionPliegoMultiple(Pliego pliego,List<Solp> solps)
        {
            try
            {
                Log.Info($"EnviarMailFinalizacionPliegoMultiple.Pliego Multiple con nombre {pliego.NombreObra} fue finalizado.");

                var copia = new List<string> { };
                var enviarA = new List<string> { };

                foreach (Solp solp in solps) {
                    Log.Info($"Obteniendo los correos de solp numero {solp.NroSolp}");
                    if (!string.IsNullOrEmpty(solp.UsuarioCreacion?.Mail))
                    {
                        
                        copia.Add(solp.UsuarioCreacion.Mail);
                        Log.Info($"Copia mail solicitante {solp.UsuarioCreacion.Mail}");
                    }

                    if (!string.IsNullOrEmpty(solp.Pliego.Email))
                    {
                        //Mail del solicitante
                        copia.Add(solp.Pliego.Email);
                        Log.Info($"Copia mail solicitante paso 1 {solp.Pliego.Email}");
                    }

                    if (!string.IsNullOrEmpty(solp.Pliego.SupervisorTrabajo))
                    {
                        //Supervisor
                        copia.Add(solp.Pliego.SupervisorTrabajo);
                        Log.Info($"Copia mail responsable de trabajo paso 2 {solp.Pliego.SupervisorTrabajo}");
                    }

                    if (solp.UsuarioCompras != null && !string.IsNullOrEmpty(solp.UsuarioCompras.Mail))
                    {
                        enviarA.Add(solp.UsuarioCompras.Mail);
                    }
                }

                var asunto = $"En el presente mail se informa la finalizacion de un Nuevo Pliego Multiple {pliego.NombreObra}";

                emailService.EnviarMail(enviarA.Distinct().ToList(), asunto, "", copia.Distinct().ToList(), ObtenerCuerpoSolpsAsociadas(pliego, solps), null, "");
            }
            catch (Exception ex)
            {
                Log.Error($"Error al enviar mail de finalizacion de Pliego Multiple: {pliego.NombreObra}", ex);
            }
        }

        private AlternateView ObtenerCuerpoCotizacionCreada(Cotizacion cotizacion)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var proveedor = cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor();

            var htmlBody = "En el presente mail se informa la cotización realizada para la SOLP " +
                $"{string.Join(", ", cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp).Select(x => x.NroSolp).Distinct())} " +
                $"y la PO {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id}, generada por el proveedor {proveedor.RazonSocial} ({proveedor.CUIT}). <br /> <br/>";

            var todasLasPosicionesNoDisponibles = cotizacion.CotizacionPosiciones.All(x => x.NoDisponible != null && x.NoDisponible.Value);
            var algunaPosicionNoDisponible = cotizacion.CotizacionPosiciones.Any(x => x.NoDisponible != null && x.NoDisponible.Value);
            var noRespetaMateriales = cotizacion.RespetaMateriales == false;

            if (todasLasPosicionesNoDisponibles || algunaPosicionNoDisponible || noRespetaMateriales)
            {
                htmlBody += $"<strong>Nota:</strong><br/>";
            }
            if (todasLasPosicionesNoDisponibles)
            {
                htmlBody += $"El proveedor no cuenta con el material disponible.<br/>";
            }
            else if (algunaPosicionNoDisponible)
            {
                htmlBody += $"El proveedor no cuenta con algún material disponible.<br/>";
            }
            if (noRespetaMateriales)
            {
                htmlBody += $"La propuesta no cumple con las especificaciones técnicas solicitadas. Revisar con prioridad. <br/>";
            }

            htmlBody += " <br/>Puede visualizar la cotización en www.moaoperaciones.com.ar " +
                 "<br/><br/>Saludos Cordiales<br/>" +
                 "Molinos Agro S.A. <br/><br/> " +
                  @"<img width:'5%' src='cid:" + logoMailResource.ContentId + @"'/>";

            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }

        private AlternateView ObtenerCuerpoSolpLiberada(Solp solp)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var htmlBody = $"En el presente mail se informa la liberación de la SOLP {solp.NroSolp} generada con Molinos Agro S.A. <br /><br/>";
            var esMaterial = solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";

            // Agregar la tabla de posiciones y subposiciones
            if (solp.Posiciones != null && solp.Posiciones.Any())
            {
                htmlBody += "<b>Detalle:</b><br/>";
                htmlBody += "<br/>";

                foreach (var posicion in solp.Posiciones)
                {
                    htmlBody += "<table style=\"border-collapse: collapse; border: 2px solid #ddd; text-align: center; font-size: 13px; width: 100%;\">";
                    htmlBody += "<tr>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Posición</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Centro</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Descripción</th>";
                    if (esMaterial)
                    {
                        htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Precio Bruto</th>";
                    }

                    htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Moneda</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Grupo de compras</th>" +
                                "</tr>";

                    // Agregar la fila para la posición
                    htmlBody += "<tr>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Indice}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Centro.Codigo}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{(!string.IsNullOrEmpty(posicion.MaterialSolp?.Descripcion) ? posicion.MaterialSolp.Descripcion : posicion.Tarea)}</td>";

                    if (esMaterial)
                    {
                        htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Unidad?.CodigoSap}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Cantidad.Value:n2}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.PrecioBruto.Value:n2}</td>";
                    }

                    htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Moneda?.CodigoSap}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.GrupoCompras?.CodigoSap}</td>" +
                                "</tr>";

                    if (!esMaterial)
                    {
                        htmlBody += "<tr>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Subposición</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Tarea a subcontratar</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Precio bruto</th>" +
                                    "</tr>";

                        foreach (var subpos in posicion.Subposiciones)
                        {
                            htmlBody += "<tr>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Numero}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Tarea}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Cantidad.Value:n2}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Unidad.CodigoSap}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.PrecioBruto.Value:n2}</td>" +
                                        "</tr>";
                        }
                    }

                    htmlBody += "</table>";
                    htmlBody += "<br/>";
                }
            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width='15%' src='cid:" + logoMailResource.ContentId + @"'/>";
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }

        private AlternateView ObtenerCuerpoSolpsAsociadas(Pliego pliego, List<Solp> solps)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var htmlBody = $"En el presente mail se informa la finalizacion de un Nuevo Pliego Multiple {pliego.NombreObra} <br /><br/>";

            foreach (Solp solp in solps) {
                var esMaterial = solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";

                // Agregar la tabla de posiciones y subposiciones
                if (solp.Posiciones != null && solp.Posiciones.Any())
                {
                    htmlBody += $"<b>Solp Nro {solp.NroSolp}:</b><br/>";
                    htmlBody += "<br/>";

                    foreach (var posicion in solp.Posiciones)
                    {
                        htmlBody += "<table style=\"border-collapse: collapse; border: 2px solid #ddd; text-align: center; font-size: 13px; width: 100%;\">";
                        htmlBody += "<tr>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Posición</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Centro</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Descripción</th>";
                        if (esMaterial)
                        {
                            htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Precio Bruto</th>";
                        }

                        htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Moneda</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Grupo de compras</th>" +
                                    "</tr>";

                        // Agregar la fila para la posición
                        htmlBody += "<tr>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Indice}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Centro.Codigo}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{(!string.IsNullOrEmpty(posicion.MaterialSolp?.Descripcion) ? posicion.MaterialSolp.Descripcion : posicion.Tarea)}</td>";

                        if (esMaterial)
                        {
                            htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Unidad?.CodigoSap}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{posicion.Cantidad.Value:n2}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{posicion.PrecioBruto.Value:n2}</td>";
                        }

                        htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Moneda?.CodigoSap}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.GrupoCompras?.CodigoSap}</td>" +
                                    "</tr>";

                        if (!esMaterial)
                        {
                            htmlBody += "<tr>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Subposición</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Tarea a subcontratar</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Precio bruto</th>" +
                                        "</tr>";

                            foreach (var subpos in posicion.Subposiciones)
                            {
                                htmlBody += "<tr>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Numero}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Tarea}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Cantidad.Value:n2}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Unidad.CodigoSap}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.PrecioBruto.Value:n2}</td>" +
                                            "</tr>";
                            }
                        }

                        htmlBody += "</table>";
                        htmlBody += "<br/>";
                    }
                }

            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                        "<br/><br/>Saludos Cordiales<br/>" +
                        "Molinos Agro S.A. <br/><br/> " +
                         @"<img width='15%' src='cid:" + logoMailResource.ContentId + @"'/>";

            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }
    }
}
