CREATE TABLE [dbo].[EcheqApertura] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [OrdenCheque] INT NOT NULL,
    [UsuarioCreacionId]     INT            NOT NULL,
    [UsuarioModificacionId] INT            NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [FechaModificacion]      DATETIME2 (7)  NULL,
    [EcheqLiquidacionId]              INT            NOT NULL,
    [ImporteCheque] DECIMAL(18, 2) NOT NULL, 
    [Estado] BIT NOT NULL, 
    CONSTRAINT [FK.EcheqApertura_Usuario_UsuarioCreacionId] FOREIGN KEY ([UsuarioCreacionId]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.EcheqApertura_Usuario_UsuarioModificacionId] FOREIGN KEY ([UsuarioModificacionId]) REFERENCES [Usuario]([Id]),


);


