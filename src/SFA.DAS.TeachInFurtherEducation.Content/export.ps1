<#
.SYNOPSIS
    Exports content from a source Contentful space and imports it into a target Contentful space based on the specified environment.

.DESCRIPTION
    This script uses the Contentful CLI to export all content, including assets, from a source Contentful space and imports it into a target space.
    The script accepts an environment identifier ("at", "pp", "prod") and the Source Management Token as parameters.

.PARAMETER Environment
    The environment identifier. Must be either "at", "pp", or "prod".

.PARAMETER SourceManagementToken
    The management token for the source Contentful space.

.EXAMPLE
    .\Export.ps1 -Environment at -SourceManagementToken "CFPAT-YourTokenHere"
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet("at", "pp", "prod", IgnoreCase=$true)]
    [string]$SourceEnvironment,

    [Parameter(Mandatory=$true, Position=1)]
    [string]$SourceManagementToken
)

# ================================
# Configuration Section
# ================================
# Update the Space IDs below with your actual values.

$sourceConfigs = @{
    "at" = @{
        "SourceEnvironmentName" = "at"
        "SourceSpaceId" = "97af9qkbawls"
    }
    "pp" = @{
        "SourceEnvironmentName" = "pp"
        "SourceSpaceId" = "97af9qkbawls"  
    }
    "prod" = @{
        "SourceEnvironmentName" = "prod"
        "SourceSpaceId" = "97af9qkbawls"  
    }
}

# Validate Environment parameter and retrieve configuration
$SourceEnvKey = $SourceEnvironment.ToLower()
if (-not $sourceConfigs.ContainsKey($SourceEnvKey)) {
    Write-Error "Invalid source environment specified. Use 'at', 'pp', or 'prod'."
    exit 1
}

$sourceconfig = $sourceConfigs[$SourceEnvKey]

$sourceEnvName = $sourceconfig.SourceEnvironmentName
$sourceSpaceId = $sourceconfig.SourceSpaceId

# Define export directory (e.g., ./exports/at or ./exports/pp)
$exportDir = Join-Path -Path (Get-Location) -ChildPath "contentful-export-$SourceEnvironment"
$contentFile = Join-Path -Path $exportDir -ChildPath "contentful-export.json"

# Ensure the export directory exists
if (-not (Test-Path $exportDir)) {
    try {
        New-Item -ItemType Directory -Path $exportDir -Force | Out-Null
        Write-Host "Created export directory at '$exportDir'."
    }
    catch {
        Write-Error "Failed to create export directory: $_"
        exit 1
    }
}

# ================================
# Export Content from Source Space
# ================================
Write-Host "Starting export from source space ID '$sourceSpaceId'..."

try {
    & contentful space export `
        --environment-id "$sourceEnvName" `
        --space-id "$sourceSpaceId" `
        --management-token "$SourceManagementToken" `
        --export-dir "$exportDir" `
        --content-file "contentful-export.json" `
        --download-assets

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Contentful export failed with exit code $LASTEXITCODE."
        exit $LASTEXITCODE
    }

    Write-Host "Export completed successfully. Content file located at '$contentFile'."
}
catch {
    Write-Error "An error occurred during export: $_"
    exit 1
}
