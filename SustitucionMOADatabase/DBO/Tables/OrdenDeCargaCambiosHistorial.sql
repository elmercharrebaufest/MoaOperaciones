CREATE TABLE [dbo].[OrdenDeCargaCambiosHistorial]
(
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [OrdenDeCarga_Id] INT NOT NULL, 
    [NombreColumnaCambio] NVARCHAR(50) NOT NULL, 
    [FechaCambio] DATETIME NULL DEFAULT (getdate()), 
    [Usuario_Id] INT NOT NULL, 
    [Antes] NVARCHAR(MAX) NOT NULL, 
    [Despues] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_OrdenDeCargaCambiosHistorial] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrdenDeCargaCambiosHistorial]  WITH CHECK ADD  CONSTRAINT [FK_OrdenDeCargaCambiosHistorial_Usuario] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrdenDeCargaCambiosHistorial]  WITH CHECK ADD  CONSTRAINT [FK_OrdenDeCargaCambiosHistorial_OrdenDeCarga] FOREIGN KEY([OrdenDeCarga_Id])
REFERENCES [dbo].[OrdenDeCarga] ([Id])
ON DELETE CASCADE
GO