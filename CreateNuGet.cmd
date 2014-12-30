set version=1.0.30-alpha
if not exist .\nuget_packages mkdir nuget_packages
del /Q .\nuget_packages\*.*
.nuget\NuGet.exe pack RoslynDOM\RoslynDom.csproj -IncludeReferencedProjects -OutputDirectory .\nuget_packages -Version %version% -symbols
.nuget\NuGet.exe pack RoslynDomCSharpFactories\RoslynDomCSharpFactories.csproj -IncludeReferencedProjects -OutputDirectory .\nuget_packages -Version %version% -symbols
.nuget\NuGet.exe pack RoslynDomExampleTests\RoslynDomExampleTests.csproj -IncludeReferencedProjects -OutputDirectory .\nuget_packages -Version %version% -symbols
copy nuget_packages\*.* ..\LocalNuGet
pause 