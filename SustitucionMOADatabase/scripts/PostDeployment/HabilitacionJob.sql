IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteLiquidacionesInformadasJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteLiquidacionesInformadasJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarTransporteOrdenesDeCargaJob') BEGIN    INSERT into HabilitacionJob VALUES ('VerificarTransporteOrdenesDeCargaJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteCamposSustentablesTSAJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteCamposSustentablesTSAJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReporteConflictosCamposSustentablesJob') BEGIN    INSERT into HabilitacionJob VALUES ('ReporteConflictosCamposSustentablesJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarBaseDeDatosSolpSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarBaseDeDatosSolpSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VencimientoOrdenesDeCargaSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('VencimientoOrdenesDeCargaSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarEstadoSolpSapJob') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarEstadoSolpSapJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ObtenerSolpsDesdeSAPJob') BEGIN    INSERT into HabilitacionJob VALUES ('ObtenerSolpsDesdeSAPJob',1)END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarLocalidades') BEGIN    INSERT into HabilitacionJob VALUES ('ActualizarLocalidades',1)END

