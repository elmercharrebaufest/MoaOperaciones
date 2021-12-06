CREATE PROCEDURE [dbo].[ActualizarIngresosBrutosCoeficienteUnificadoRechazado]
    @IdIngresosBrutosCoeficienteUnificado INT,
    @Observaciones VARCHAR(MAX)
AS   
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado INT = 2
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_RechazadoSistemas INT = 5

    DECLARE @TipoMovimientoIngresosBrutosCoeficienteUnificado_Error INT = 5
    DECLARE @OrigenMovimientoIngresosBrutosCoeficienteUnificado_SAP INT = 2
    
    IF EXISTS (SELECT TOP 1 1 FROM IngresosBrutosCoeficienteUnificado 
               WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
               AND EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado)
    BEGIN
        UPDATE IngresosBrutosCoeficienteUnificado 
        SET EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_RechazadoSistemas
        WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
        and EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado

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
            @TipoMovimientoIngresosBrutosCoeficienteUnificado_Error,
            @OrigenMovimientoIngresosBrutosCoeficienteUnificado_SAP,
            @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado,
            @EstadoIngresosBrutosCoeficienteUnificado_Id_RechazadoSistemas)
    END
GO