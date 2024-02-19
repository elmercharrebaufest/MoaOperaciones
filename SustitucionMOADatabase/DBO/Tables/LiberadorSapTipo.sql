CREATE TABLE [dbo].[LiberadorSapTipo] (
    [Id]                    INT IDENTITY (1, 1) NOT NULL,
    [Nombre]                NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_dbo.LiberadorSapTipo] PRIMARY KEY CLUSTERED ([Id] ASC)
);