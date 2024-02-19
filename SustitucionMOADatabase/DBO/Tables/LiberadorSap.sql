CREATE TABLE [dbo].[LiberadorSap] (
    [Id]                     INT IDENTITY (1, 1) NOT NULL,
    [Mail]                   NVARCHAR (50)  NOT NULL,
    [NombreCompleto]         NVARCHAR (50)  NOT NULL,
    [Habilitado]             BIT NOT NULL DEFAULT 1, 
    [Obligatorio]            BIT NOT NULL DEFAULT 0, 
    [Cargo]                  NVARCHAR (50)  NOT NULL,
    [LiberadorSapTipo_Id]    INT NOT NULL, 
    CONSTRAINT [PK_dbo.LiberadorSap] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LiberadorSapTipo] FOREIGN KEY ([LiberadorSapTipo_Id]) REFERENCES [dbo].[LiberadorSapTipo] ([Id])
);