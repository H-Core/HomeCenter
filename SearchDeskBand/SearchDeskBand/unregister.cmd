@echo off   
color 0B   
echo JumpStart! v0.1 by Scotch   
echo.   
echo Your desktop is being restored, Please wait. . .   
timeout 1
echo Killing process Explorer.exe. . .   
taskkill /f /im explorer.exe   
cls   
echo Success!   
echo.   
echo Your desktop is now loading. . .   
timeout 1
echo.   
start explorer.exe   

%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %~dp0\bin\Debug\H.NET.SearchDeskBand.dll /tlb /codebase /unregister /nologo
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %~dp0\bin\Release\H.NET.SearchDeskBand.dll /tlb /codebase /unregister /nologo
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %~dp0\Libraries\SharpShell.dll /tlb /codebase /unregister /nologo
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %~dp0\bin\Debug\SharpShell.dll /tlb /codebase /unregister /nologo
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe %~dp0\bin\Release\SharpShell.dll /tlb /codebase /unregister /nologo

pause