CREATE TABLE [dbo].[CircularArchivo](
   
	[Circular_Id] [int] NOT NULL,
	[Archivo_Id] [int] NOT NULL, 
   
   
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CircularArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CircularArchivo_dbo.Circular_Circular_Id] FOREIGN KEY([Circular_Id])
REFERENCES [dbo].[Circular] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CircularArchivo] CHECK CONSTRAINT [FK_dbo.CircularArchivo_dbo.Circular_Circular_Id]
GO

ALTER TABLE [dbo].[CircularArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CircularArchivo_dbo.Archivo_Archivo_Id] FOREIGN KEY([Archivo_Id])
REFERENCES [dbo].[Archivo] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CircularArchivo] CHECK CONSTRAINT [FK_dbo.CircularArchivo_dbo.Archivo_Archivo_Id]
GO

