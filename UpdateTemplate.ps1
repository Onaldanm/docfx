$ErrorActionPreference = "Stop"

# Check if node exists globally
if (-not (Get-Command "node" -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: UpdateTemplate.sh requires node installed globally."
    exit 1
}

$logLevelParam = if ($env:Agent.Name -eq "Hosted Windows 2019 with VS2019") { "--loglevel=error" } else { "" }
exec { & npm install $logLevelParam }

Push-Location $PSScriptRoot

$TemplateHome="$PSScriptRoot/src/docfx.website.themes/"
$DefaultTemplate="${TemplateHome}default/"
$GulpCommand="${DefaultTemplate}node_modules/gulp/bin/gulp"

Set-Location "$DefaultTemplate"
npm install $logLevelParam
node ./node_modules/bower/bin/bower install
node "$GulpCommand"

Set-Location "$TemplateHome"
npm install $logLevelParam
node "$GulpCommand"

Pop-Location
