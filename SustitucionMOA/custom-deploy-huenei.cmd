@echo off

REM Configura la ruta destino
set DESTINATION_FOLDER=C:\inetpub\wwwroot\QaHueneiMoaOperaciones

REM Elimina la carpeta destino si existe
IF EXIST "%DESTINATION_FOLDER%" (
    echo Eliminando la carpeta destino...
    rmdir /S /Q "%DESTINATION_FOLDER%"
)

REM Crea la carpeta destino
echo Creando la carpeta destino...
mkdir "%DESTINATION_FOLDER%"

echo Eliminando completado.
