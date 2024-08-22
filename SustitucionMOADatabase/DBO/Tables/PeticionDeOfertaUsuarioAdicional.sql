CREATE TABLE [dbo].[PeticionDeOfertaUsuarioAdicional]
(
    [Id] INT NOT NULL IDENTITY, 
    [PeticionDeOferta_Id] INT NOT NULL, 
    [Usuario_Id] INT NOT NULL,
    CONSTRAINT [PK_PeticionDeOfertaUsuarioAdicional] PRIMARY KEY ([Id]),
    CONSTRAINT [FK.PeticionDeOfertaUsuarioAdicional_Usuario_Usuario_Id] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]), 
    CONSTRAINT [FK.PeticionDeOfertaUsuarioAdicional_PeticionDeOferta_PeticionDeOferta_Id] FOREIGN KEY ([PeticionDeOferta_Id]) REFERENCES [PeticionDeOferta]([Id]) ON DELETE CASCADE, 

)
