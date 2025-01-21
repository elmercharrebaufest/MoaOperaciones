IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteLiquidacionesInformadasJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteLiquidacionesInformadasJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarTransporteOrdenesDeCargaJob') BEGIN    INSERT into HabilitacionJob VALUES ('VerificarTransporteOrdenesDeCargaJob',1)END

IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnviarASAPOrdenDeCargaJob') 
BEGIN 
	INSERT INTO HabilitacionJob VALUES ('EnviarASAPOrdenDeCargaJob', 1) 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteCamposSustentablesTSAJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteCamposSustentablesTSAJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteConflictosCamposSustentablesJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteConflictosCamposSustentablesJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarBaseDeDatosSolpSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarBaseDeDatosSolpSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VencimientoOrdenesDeCargaSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('VencimientoOrdenesDeCargaSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarEstadoSolpSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarEstadoSolpSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ObtenerSolpsDesdeSAPJob') BEGIN    INSERT into HabilitacionJob VALUES ('ObtenerSolpsDesdeSAPJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarLocalidades') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarLocalidades',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarSISAJob') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarSISAJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VencimientoOrdenesDeCargaFasonJob') BEGIN    INSERT into HabilitacionJob VALUES ('VencimientoOrdenesDeCargaFasonJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteLoginsJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteLoginsJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarSituacionCrediticiaJob') BEGIN    INSERT into HabilitacionJob VALUES ('VerificarSituacionCrediticiaJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarTransporteOrdenesDeCargaFasonJob') BEGIN    INSERT into HabilitacionJob VALUES ('VerificarTransporteOrdenesDeCargaFasonJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'AltaClienteSAPJob') BEGIN    INSERT into HabilitacionJob VALUES ('AltaClienteSAPJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarOrdenesFacturaCompensadaJob') BEGIN    INSERT into HabilitacionJob VALUES ('VerificarOrdenesFacturaCompensadaJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'CcSsObtenerArchivosUcropJob') BEGIN    INSERT into HabilitacionJob VALUES ('CcSsObtenerArchivosUcropJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnviarMailReporteSOLPJob') BEGIN    INSERT into HabilitacionJob VALUES ('EnviarMailReporteSOLPJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnviarCamposUcropitJob') BEGIN    INSERT into HabilitacionJob VALUES ('EnviarCamposUcropitJob',1)END

IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VencimientoOrdenesResiduosJob') BEGIN    INSERT into HabilitacionJob VALUES ('VencimientoOrdenesResiduosJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'NotificacionErroresJob') BEGIN    INSERT into HabilitacionJob VALUES ('NotificacionErroresJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EliminarFacturasAntiguasJob') BEGIN    INSERT into HabilitacionJob VALUES ('EliminarFacturasAntiguasJob',1)END
