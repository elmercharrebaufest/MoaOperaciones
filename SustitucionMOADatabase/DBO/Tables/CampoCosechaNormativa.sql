CREATE TABLE [dbo].[CampoCosechaNormativa]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [CampoCosecha_Id] INT NOT NULL, 
    [TipoNormativa_Id] INT NOT NULL, 
    [ToneladasAprobadas] FLOAT NULL, 
    [MotivoRechazo] NVARCHAR(1000) NULL, 
    [Validado] BIT NOT NULL DEFAULT 0, 
    [ValidadoPor] INT NULL, 
    [ValidadoFecha] DATETIME NULL,
    CONSTRAINT [FK_CampoCosechaNormativa_ToCampoCosecha] FOREIGN KEY ([CampoCosecha_Id]) REFERENCES [CampoCosecha](Id),
    CONSTRAINT [FK_CampoCosechaNormativa_ToTipoNormativa] FOREIGN KEY ([TipoNormativa_Id]) REFERENCES [TipoNormativa](Id)
)
