param(
    [switch]$SkipBuild
)

if (-not $SkipBuild) {
    Write-Host "Building lpastlai..." -ForegroundColor Cyan
    dotnet build -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed." -ForegroundColor Red
        exit 1
    }
}

$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$outputDir = Join-Path $projectDir "releases"
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

$targets = @(
    @{ Rid = "win-x64";  Suffix = "win-x64" },
    @{ Rid = "win-x86";  Suffix = "win-x86" }
)

foreach ($t in $targets) {
    $rid = $t.Rid
    $suffix = $t.Suffix
    Write-Host "Publishing $rid..." -ForegroundColor Cyan

    & dotnet publish -c Release -r $rid --self-contained false -p:PublishSingleFile=true -p:DebugType=none

    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Failed to publish $rid" -ForegroundColor Red
        continue
    }

    $src = Join-Path $projectDir "bin\Release\net8.0-windows\$rid\publish\lpastlai.exe"
    $dst = Join-Path $outputDir "lpastlai-$suffix.exe"

    if (Test-Path $src) {
        Copy-Item -Path $src -Destination $dst -Force
        Write-Host "  -> $((Get-Item $dst).FullName)" -ForegroundColor Green
    }
}

Write-Host "Done. Releases in: $outputDir" -ForegroundColor Cyan
