@echo off

set Path=ProtoGen\protogen.exe
set toPath=C:\develop\rxzp\Assets\Scripts\Protocols

for /f "delims=" %%i in ('dir /b proto "proto/*.proto"') do %Path% -i:proto/%%i -o:%toPath%/%%~ni.cs
pause
