set version=1.0.8-alpha
if not exist .\nuget_packages mkdir nuget_packages
del /Q .\nuget_packages\*.*
.nuget\NuGet.exe pack RoslynDOM\RoslynDom.csproj -IncludeReferencedProjects -OutputDirectory .\nuget_packages -Version %version% -symbols
pause 