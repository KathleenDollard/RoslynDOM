set version=1.0.0-alpha
if not exist .\nuget_packages mkdir nuget_packages
del /Q .\nuget_packages\*.*
.nuget\NuGet.exe pack RoslynK\RoslynK.csproj -OutputDirectory .\nuget_packages -Version %version% -symbols
pause 