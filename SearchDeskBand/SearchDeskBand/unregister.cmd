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

%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe  bin\Debug\WebSearchDeskBand.dll /tlb /codebase /unregister
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe  bin\Release\WebSearchDeskBand.dll /tlb /codebase /unregister
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe  Libraries\SharpShell.dll /tlb /codebase /unregister

exit 