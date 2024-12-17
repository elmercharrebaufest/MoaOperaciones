@echo off

REM Variables
set SERVER=\\10.12.42.90\C$\inetpub\wwwroot\QaHueneiMoaOperaciones
set DRIVE=Z:
set USER=MOLINOSAGRO\UsrSvcMOAOperaciones
set PASSWORD="FA!rLf-#aC5Spl"

REM Conectar la unidad de red
echo Conectando la unidad de red...
net use %DRIVE% %SERVER% /user:%USER% %PASSWORD%
if errorlevel 1 (
    echo Error al conectar la unidad de red.
    exit /b 1
)

REM Eliminar la carpeta
echo Eliminando la carpeta en %DRIVE%...
if exist %DRIVE%\ (
    rmdir /s /q %DRIVE%\
    echo Carpeta eliminada.
) else (
    echo La carpeta no existe.
)

REM Desconectar la unidad de red
echo Desconectando la unidad de red...
net use %DRIVE% /delete /yes

echo Proceso completado.
exit /b 0
