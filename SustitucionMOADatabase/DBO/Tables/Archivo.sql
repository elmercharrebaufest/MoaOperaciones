
CREATE TABLE [dbo].[Archivo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileKey] [nvarchar](400) NULL,
	[Ruta] [nvarchar](max) NULL,
	[ArchivoSap] BIT NULL DEFAULT 0,
	[Proveedor_Id] [int] NULL,
 CONSTRAINT [PK_dbo.Archivo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Archivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Archivo_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
GO

ALTER TABLE [dbo].[Archivo] CHECK CONSTRAINT [FK_dbo.Archivo_dbo.Proveedor_Proveedor_Id]
GO

CREATE NONCLUSTERED INDEX [IX_Archivo_FileKey] ON [dbo].[Archivo]([FileKey])

GO