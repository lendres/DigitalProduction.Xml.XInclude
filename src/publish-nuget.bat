@echo off

rem Get the api key.
set /p api-key=<"../../Keys/Nuget/api-key.txt
echo API KEY: %api-key%

rem Change to the package directory.
echo %cd%
chdir /d ./bin/Release

echo %cd%

rem Publish.
for /f %%a in ('call dir *.nupkg /b') do (
	echo
	echo Processing: %%a
	dotnet nuget push %%a --api-key %api-key% --source https://api.nuget.org/v3/index.json --skip-duplicate
)



rem dotnet nuget push DigitalProductionDotNetLibrary.0.1.1.nupkg --api-key %api-key% --source https://api.nuget.org/v3/index.json --skip-duplicate

pause