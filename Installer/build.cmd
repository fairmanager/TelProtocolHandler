@ECHO OFF

ECHO Clearing output folders...
CD ..
FOR /D /R . %%d in (bin obj) do @IF EXIST "%%d" RD /S /Q "%%d"

ECHO Building project...
CD Installer
IF EXIST Output RD /S /Q Output
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe" /Build "Release|x86" "%~dp0..\ClickToDial.sln"

ECHO Building installer...
"C:\Program Files (x86)\Inno Setup 5\ISCC.exe" "%~dp0ClickToDial.iss"

IF EXIST "../.certificates/PFX_PASSWORD" (
	ECHO Signing installer...
	SET /p FAIRMANAGER_CODE_SIGNING_PASSWORD=<"../.certificates/PFX_PASSWORD"
	"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool.exe" sign /f "../.certificates/fairmanager-production-codesign.pfx" /p "%FAIRMANAGER_CODE_SIGNING_PASSWORD%" "Output/*"
	"C:\Program Files (x86)\Windows Kits\10\bin\x86\signtool.exe" timestamp /t http://timestamp.comodoca.com/authenticode "Output/*"
)
