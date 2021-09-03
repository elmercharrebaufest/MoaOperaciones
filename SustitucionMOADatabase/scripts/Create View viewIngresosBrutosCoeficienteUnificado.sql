CREATE VIEW [dbo].[viewIngresosBrutosCoeficienteUnificado] AS

SELECT
i.Id, 
i.Anticipo, 
i.Cuit, 
i.Sede, 
i.FechaCarga, 
i.EstadoIngresosBrutosCoeficienteUnificado_Id AS EstadoId, 
ei.Descripcion AS Estado,
i.SecuenciaIngresosBrutosCoeficienteUnificado_Id AS SecuenciaId, 
si.Descripcion AS Secuencia,
id.Id AS DetalleId, 
id.NumeroJurisdiccion, 
id.Jurisdiccion, 
id.FechaInicio, 
id.FechaCese, 
id.CoeficienteUnificado
FROM IngresosBrutosCoeficienteUnificado i
JOIN EstadoIngresosBrutosCoeficienteUnificado ei ON i.EstadoIngresosBrutosCoeficienteUnificado_Id = ei.Id
JOIN SecuenciaIngresosBrutosCoeficienteUnificado si ON i.SecuenciaIngresosBrutosCoeficienteUnificado_Id = si.Id
JOIN IngresosBrutosCoeficienteUnificadoDetalle id ON i.Id = id.IngresosBrutosCoeficienteUnificado_Id
WHERE i.EstadoIngresosBrutosCoeficienteUnificado_Id = 2
GO