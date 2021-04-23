CREATE TABLE [dbo].[ConflictoCampoSustentable]
(
    [IdCampo] INT NULL, 
    [IdTSA] INT NULL, 
    [IdCampoSutentable] INT NULL,
    [ToneladasActuales] FLOAT NULL, 
    [ToneladasInformadas] FLOAT NULL, 
    [StockDisponible] FLOAT NULL, 
    [CUIT] NVARCHAR(15) NULL,
    [MotivoRechazo] NVARCHAR(500) NULL,
    [FechaConflicto] DATE NULL, 
    [Notificado] BIT NULL, 
)
