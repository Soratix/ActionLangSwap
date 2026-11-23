<#
.SYNOPSIS
    Compile le plugin, met à jour repo.json et publie une release GitHub.

.EXAMPLE
    .\publish.ps1 -Version 0.2.0 -Notes "Ajout des statuts dans l'infobulle"
    .\publish.ps1 -Version 0.2.0 -NoRelease   # met juste à jour les fichiers, sans pousser
#>
param(
    [Parameter(Mandatory = $true)][string]$Version,
    [string]$Notes = "",
    [switch]$NoRelease
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$owner = "Soratix"
$repo = "ActionLangSwap"
$project = Join-Path $root "ActionLangSwap"
$utf8 = New-Object System.Text.UTF8Encoding($false)

if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw "Version attendue au format X.Y.Z (ex : 0.2.0)" }

# 1. Version dans le .csproj
$csproj = Join-Path $project "ActionLangSwap.csproj"
$xml = [IO.File]::ReadAllText($csproj) -replace '<Version>[^<]*</Version>', "<Version>$Version.0</Version>"
[IO.File]::WriteAllText($csproj, $xml, $utf8)

# 2. Compilation (produit bin\Release\ActionLangSwap\latest.zip)
dotnet build $project -c Release
if ($LASTEXITCODE -ne 0) { throw "La compilation a échoué." }

$zip = Join-Path $project "bin\Release\ActionLangSwap\latest.zip"
if (-not (Test-Path $zip)) { throw "latest.zip introuvable : $zip" }

# 3. repo.json : le manifeste généré + les liens de téléchargement
$manifest = Get-Content (Join-Path $project "bin\Release\ActionLangSwap.json") -Raw -Encoding UTF8 | ConvertFrom-Json
$download = "https://github.com/$owner/$repo/releases/latest/download/latest.zip"
$entry = [ordered]@{
    Author              = $manifest.Author
    Name                = $manifest.Name
    InternalName        = $manifest.InternalName
    AssemblyVersion     = $manifest.AssemblyVersion
    TestingAssemblyVersion = $manifest.AssemblyVersion
    Description         = $manifest.Description
    Punchline           = $manifest.Punchline
    ApplicableVersion   = $manifest.ApplicableVersion
    Tags                = @($manifest.Tags)
    DalamudApiLevel     = $manifest.DalamudApiLevel
    RepoUrl             = "https://github.com/$owner/$repo"
    DownloadLinkInstall = $download
    DownloadLinkUpdate  = $download
    DownloadLinkTesting = $download
    LastUpdate          = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
    AcceptsFeedback     = $false
}
$json = ConvertTo-Json -InputObject @($entry) -Depth 5
[IO.File]::WriteAllText((Join-Path $root "repo.json"), $json, $utf8)
Write-Host "repo.json mis à jour (v$Version, API $($manifest.DalamudApiLevel))." -ForegroundColor Green

if ($NoRelease) { return }

# 4. Commit, tag, push et release GitHub
git -C $root add -A
git -C $root commit -m "Version $Version"
git -C $root tag "v$Version"
git -C $root push origin main --tags
if ($LASTEXITCODE -ne 0) { throw "git push a échoué." }

if ($Notes -eq "") { $Notes = "Version $Version" }
gh release create "v$Version" $zip --repo "$owner/$repo" --title "v$Version" --notes $Notes
if ($LASTEXITCODE -ne 0) { throw "La création de la release a échoué." }

Write-Host "Publié ! Dalamud proposera la mise à jour d'ici quelques minutes." -ForegroundColor Green
