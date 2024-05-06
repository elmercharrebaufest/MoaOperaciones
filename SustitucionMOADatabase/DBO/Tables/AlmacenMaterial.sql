CREATE TABLE [dbo].[AlmacenMaterial]
(
	[Almacen_Id] INT NOT NULL , 
    [Material_Id] INT NOT NULL, 
    PRIMARY KEY ([Almacen_Id], [Material_Id]), 
    CONSTRAINT [FK_AlmacenMaterial_Almacen] FOREIGN KEY ([Almacen_Id]) REFERENCES [Almacen]([Id]), 
    CONSTRAINT [FK_AlmacenMaterial_Material] FOREIGN KEY ([Material_Id]) REFERENCES [Material]([Id])
)
