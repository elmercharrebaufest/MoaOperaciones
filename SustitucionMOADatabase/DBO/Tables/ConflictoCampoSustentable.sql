CREATE TABLE [dbo].[ConflictoCampoSustentable]
(
    [IdCampo] INT NOT NULL, 
    [IdTSA] INT NOT NULL, 
    [IdCampoSustentable] INT NULL,
    [ToneladasActuales] FLOAT NULL, 
    [ToneladasInformadas] FLOAT NOT NULL, 
    [StockDisponible] FLOAT NULL, 
    [CUIT] NVARCHAR(15) NOT NULL,
    [MotivoRechazo] NVARCHAR(500) NULL,
    [FechaConflicto] DATE NULL, 
    [Notificado] BIT NULL, 
    CONSTRAINT [PK_ConflictoCampoSustentable] PRIMARY KEY ([IdCampo], [IdTSA], [CUIT], [ToneladasInformadas]), 
)
