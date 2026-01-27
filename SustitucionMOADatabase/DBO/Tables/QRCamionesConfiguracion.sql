CREATE TABLE QRCamionesConfiguracion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreEtapa NVARCHAR(50) NOT NULL,
    TiempoEstimado INT NOT NULL,
    TipoWorkflow NVARCHAR(50) NOT NULL,
    FinEtapa NVARCHAR(50) NOT NULL,
    FinEtapaEsControlRecorrido BIT NOT NULL
);