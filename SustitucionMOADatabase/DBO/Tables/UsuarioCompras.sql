CREATE TABLE [dbo].[UsuarioCompras] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Mail]       NVARCHAR (50)  NULL,
    [Nombres]    NVARCHAR (200) NULL,
    [Habilitado] BIT            DEFAULT ((1)) NOT NULL,
    [PorDefecto] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.UsuarioCompras] PRIMARY KEY CLUSTERED ([Id] ASC)
);

