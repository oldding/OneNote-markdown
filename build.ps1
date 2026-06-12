param(
    [ValidateSet("All", "x86", "x64")]
    [string]$Architecture = "All",
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = if (Test-Path $vswhere) {
    & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" |
        Select-Object -First 1
} else {
    $null
}
$iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"

if (-not $msbuild -or -not (Test-Path $msbuild)) {
    throw "MSBuild not found. Install Visual Studio Build Tools with the MSBuild component."
}
if (-not (Test-Path $iscc)) {
    throw "Inno Setup compiler not found: $iscc"
}

& $msbuild "$root\src\OneNoteMarkdown.AddIn\OneNoteMarkdown.AddIn.csproj" /t:Rebuild "/p:Configuration=$Configuration" /v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $msbuild "$root\tests\OneNoteMarkdown.Tests\OneNoteMarkdown.Tests.csproj" /t:Rebuild "/p:Configuration=$Configuration" /v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& "$root\tests\OneNoteMarkdown.Tests\bin\$Configuration\OneNoteMarkdown.Tests.exe"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$architectures = if ($Architecture -eq "All") { @("x86", "x64") } else { @($Architecture) }
foreach ($arch in $architectures) {
    & $iscc "/DInstallerArch=$arch" "$root\src\OneNoteMarkdown.Installer\setup.iss"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Get-ChildItem "$root\src\OneNoteMarkdown.Installer\Output\OneNoteMarkdownSetup-*.exe" |
    Select-Object Name, Length, LastWriteTime
