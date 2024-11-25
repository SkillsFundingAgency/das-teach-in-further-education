<#
.SYNOPSIS
    Exports content from a source Contentful space and imports it into a target Contentful space based on the specified environment.

.DESCRIPTION
    This script uses the Contentful CLI to export all content, including assets, from a source Contentful space and imports it into a target space.
    The script accepts environment identifiers and management tokens for both the source and target spaces as parameters.

.PARAMETER SourceEnvironment
    The environment identifier for the source Contentful space. Must be either "at", "pp", or "prod".

.PARAMETER TargetEnvironment
    The environment identifier for the target Contentful space. Must be either "at", "pp", or "prod".

.PARAMETER TargetManagementToken
    The management token for the target Contentful space.

.EXAMPLE
    .\Import.ps1 -SourceEnvironment at -TargetEnvironment pp -TargetManagementToken "CFPAT-TargetToken"
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet("at", "pp", "prod", IgnoreCase=$true)]
    [string]$SourceEnvironment,

    [Parameter(Mandatory=$true, Position=1)]
    [ValidateSet("at", "pp", "prod", IgnoreCase=$true)]
    [string]$TargetEnvironment,

    [Parameter(Mandatory=$true, Position=2)]
    [string]$TargetManagementToken
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

$targetConfigs = @{
    "at" = @{
        "TargetEnvironmentName" = "at"
        "TargetSpaceId" = "97af9qkbawls"
    }
    "pp" = @{
        "TargetEnvironmentName" = "pp"
        "TargetSpaceId" = "97af9qkbawls"
    }
    "prod" = @{
        "TargetEnvironmentName" = "prod"
        "TargetSpaceId" = "97af9qkbawls"  
    }
}

# Validate Environment parameter and retrieve configuration
$SourceEnvKey = $SourceEnvironment.ToLower()
if (-not $sourceConfigs.ContainsKey($SourceEnvKey)) {
    Write-Error "Invalid source environment specified. Use 'at', 'pp', or 'prod'."
    exit 1
}

$TargetEnvKey = $TargetEnvironment.ToLower()
if (-not $targetConfigs.ContainsKey($TargetEnvKey)) {
    Write-Error "Invalid target environment specified. Use 'at', 'pp', or 'prod'."
    exit 1
}

$sourceconfig = $sourceConfigs[$SourceEnvKey]
$targetconfig = $targetConfigs[$TargetEnvKey]

$targetEnvName = $targetconfig.TargetEnvironmentName
$targetSpaceId = $targetconfig.TargetSpaceId

# Define export directory (e.g., ./exports/at or ./exports/pp)
$exportDir = Join-Path -Path (Get-Location) -ChildPath "contentful-export-$SourceEnvironment"
$contentFile = Join-Path -Path $exportDir -ChildPath "contentful-export.json"

# ================================
# Import Content into Target Space
# ================================
Write-Host "Starting import into target space ID '$targetSpaceId'..."

try {
    & contentful space import `
        --environment-id "$targetEnvName" `
        --space-id "$targetSpaceId" `
        --management-token "$TargetManagementToken" `
        --content-file "$contentFile"

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Contentful import failed with exit code $LASTEXITCODE."
        exit $LASTEXITCODE
    }

    Write-Host "Import completed successfully into space ID '$targetSpaceId'."
}
catch {
    Write-Error "An error occurred during import: $_"
    exit 1
}

# ================================
# Completion Message
# ================================
Write-Host "Contentful import operation completed successfully for source environment '$SourceEnvironment' to target environment '$TargetEnvironment'."
