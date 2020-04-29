
rem call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat"
rem dotnet tool install Octopus.DotNet.Cli --global
powershell -command "& { get-date -format yyyy.M.d.HHmm | out-file -filepath .\version.tmp -nonewline -encoding ascii }"
set /p VERSION=< .\version.tmp

rem build and test .net core/standard assemblies...
dotnet restore Diamond.sln --no-cache --verbosity quiet
dotnet build Diamond.sln --configuration Release --verbosity quiet
dotnet test Common.LibTests\Common.LibTests.csproj --configuration Release

rem package .net core/standard assemblies...
dotnet publish Identity.Web\Identity.Web.csproj --output Identity.Web\bin\Release\netcoreapp3.1\publish --configuration Release
dotnet publish STS.Web\STS.Web.csproj --output STS.Web\bin\Release\netcoreapp3.1\publish --configuration Release
dotnet publish Games.Web\Games.Web.csproj --output Games.Web\bin\Release\netcoreapp3.1\publish --configuration Release
dotnet octo pack --id=Ahfc.WebSvc.Sandbox2.Identity --version=%VERSION% --basePath=Identity.Web\bin\Release\netcoreapp3.1\publish\ --outFolder=.
dotnet octo pack --id=Ahfc.WebSvc.Sandbox2.Sts --version=%VERSION% --basePath=STS.Web\bin\Release\netcoreapp3.1\publish\ --outFolder=.
dotnet octo pack --id=Ahfc.WebSvc.Sandbox2.Games --version=%VERSION% --basePath=Games.Web\bin\Release\netcoreapp3.1\publish\ --outFolder=.

rem dotnet tool uninstall Octopus.DotNet.Cli --global
set VERSION=

rem powershell -command "& { update-package -reinstall }"
rem powershell -command "& { get-project –all | add-bindingredirect }"
