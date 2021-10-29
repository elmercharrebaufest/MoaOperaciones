CREATE TABLE [dbo].[UsuarioComprasRelacionConUsuarios] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [Usuario_Id]        INT NOT NULL,
    [UsuarioCompras_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.UsuarioComprasRelacionConUsuarios] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.UsuarioComprasRelacionConUsuarios_dbo.Usuario_Usuario_Id] FOREIGN KEY ([Usuario_Id]) REFERENCES [dbo].[Usuario] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UsuarioComprasRelacionConUsuarios_dbo.UsuarioCompras_UsuarioCompras_Id] FOREIGN KEY ([UsuarioCompras_Id]) REFERENCES [dbo].[UsuarioCompras] ([Id]) ON DELETE CASCADE
);

