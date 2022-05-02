CREATE PROCEDURE ActualizarIngresosBrutosCoeficienteUnificado
    @IdIngresosBrutosCoeficienteUnificado int,
    @Observaciones VARCHAR(MAX) = ''
AS   
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado INT = 2
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado INT = 3

    DECLARE @EstadoConsultaCerrado INT = 6
    
    DECLARE @TipoMovimientoIngresosBrutosCoeficienteUnificado_ExportacionExitosa INT = 4
    DECLARE @OrigenMovimientoIngresosBrutosCoeficienteUnificado_SAP INT = 2
    
    IF EXISTS (SELECT TOP 1 1 FROM IngresosBrutosCoeficienteUnificado 
               WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
               AND EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado)
    BEGIN
        UPDATE IngresosBrutosCoeficienteUnificado 
        SET EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado 
        WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
        and EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado

        UPDATE c
        SET EstadoConsulta_Id = @EstadoConsultaCerrado
        FROM Consulta c
        JOIN IngresosBrutosCoeficienteUnificado i ON c.Id = i.Consulta_Id
        WHERE i.Id = @IdIngresosBrutosCoeficienteUnificado
    
        INSERT MovimientoIngresosBrutosCoeficienteUnificado
            (IngresosBrutosCoeficienteUnificado_Id, 
             Observaciones, 
             Fecha, 
             TipoMovimientoIngresosBrutosCoeficienteUnificado_Id, 
             OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id, 
             EstadoAnterior_Id, 
             EstadoPosterior_Id)
        VALUES
           (@IdIngresosBrutosCoeficienteUnificado,
            @Observaciones,
            GETDATE(),
            @TipoMovimientoIngresosBrutosCoeficienteUnificado_ExportacionExitosa,
            @OrigenMovimientoIngresosBrutosCoeficienteUnificado_SAP,
            @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado,
            @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado)
    END
GO