#!/usr/bin/env pwsh
# Copyright (c) Microsoft Corporation.
# SPDX-License-Identifier: MIT
#Requires -Version 5.1

<#
.SYNOPSIS
    Creates a baseline copy for one pattern solution.
.DESCRIPTION
    Copies baseline source and tests, creates a solution and README stub, and adds both projects to the umbrella solution.
.PARAMETER Category
    Pattern category: creational, structural or behavioral.
.PARAMETER Name
    Pattern name in kebab-case.
.EXAMPLE
    ./tools/new-pattern.ps1 -Category structural -Name facade
.NOTES
    Compatible with Windows PowerShell 5.1 and PowerShell 7.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('creational', 'structural', 'behavioral')]
    [string]$Category,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[a-z0-9]+(?:-[a-z0-9]+)*$')]
    [string]$Name
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

#region Functions

function Find-RepositoryRoot {
    <#
    .SYNOPSIS
        Finds the repository root from a starting directory.
    .PARAMETER StartPath
        Directory from which to search upward.
    .OUTPUTS
        [string] Repository root path.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$StartPath
    )

    $directory = [System.IO.DirectoryInfo]::new($StartPath)
    while ($null -ne $directory) {
        if (Test-Path -LiteralPath (Join-Path $directory.FullName 'GangOfFour.slnx')) {
            return $directory.FullName
        }

        $directory = $directory.Parent
    }

    throw 'Could not find GangOfFour.slnx above the script directory.'
}

function ConvertTo-PascalName {
    <#
    .SYNOPSIS
        Converts a kebab-case pattern name to PascalCase.
    .PARAMETER Value
        Pattern name in kebab-case.
    .OUTPUTS
        [string] PascalCase name.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    $parts = $Value.Split([char]'-')
    $result = foreach ($part in $parts) {
        $part.Substring(0, 1).ToUpperInvariant() + $part.Substring(1)
    }

    return $result -join ''
}

function Copy-ProjectTree {
    <#
    .SYNOPSIS
        Copies a project tree while excluding build output.
    .PARAMETER Source
        Source directory.
    .PARAMETER Destination
        Destination directory.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Source,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    foreach ($item in Get-ChildItem -LiteralPath $Source -Recurse -Force) {
        $relativePath = $item.FullName.Substring($Source.Length).TrimStart([char[]]@('\', '/'))
        $segments = $relativePath -split '[\\/]'
        if ($segments -contains 'bin' -or $segments -contains 'obj') {
            continue
        }

        $targetPath = Join-Path $Destination $relativePath
        if ($item.PSIsContainer) {
            New-Item -ItemType Directory -Path $targetPath -Force | Out-Null
        }
        else {
            $targetDirectory = Split-Path -Path $targetPath -Parent
            New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
            Copy-Item -LiteralPath $item.FullName -Destination $targetPath -Force
        }
    }
}

function Write-XmlDocument {
    <#
    .SYNOPSIS
        Writes an XML document as UTF-8 without a BOM and with LF endings.
    .PARAMETER Document
        XML document to write.
    .PARAMETER Path
        Destination path.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlDocument]$Document,

        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $settings = [System.Xml.XmlWriterSettings]::new()
    $settings.Encoding = [System.Text.UTF8Encoding]::new($false)
    $settings.Indent = $true
    $settings.IndentChars = '  '
    $settings.NewLineChars = "`n"
    $settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace
    $settings.OmitXmlDeclaration = $true
    $writer = [System.Xml.XmlWriter]::Create($Path, $settings)
    try {
        $Document.Save($writer)
    }
    finally {
        $writer.Dispose()
    }
}

function New-PatternSolution {
    <#
    .SYNOPSIS
        Writes a pattern solution containing the copied source and test projects.
    .PARAMETER Path
        Solution file path.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $document = [System.Xml.XmlDocument]::new()
    $solution = $document.CreateElement('Solution')
    [void]$document.AppendChild($solution)
    foreach ($projectPath in @('src/CornerStore/CornerStore.csproj', 'tests/CornerStore.Tests/CornerStore.Tests.csproj')) {
        $project = $document.CreateElement('Project')
        $pathAttribute = $document.CreateAttribute('Path')
        $pathAttribute.Value = $projectPath
        [void]$project.Attributes.Append($pathAttribute)
        [void]$solution.AppendChild($project)
    }

    Write-XmlDocument -Document $document -Path $Path
}

function Add-ProjectsToSolution {
    <#
    .SYNOPSIS
        Adds a pattern's projects to the matching folder in the umbrella solution.
    .PARAMETER SolutionPath
        Umbrella solution path.
    .PARAMETER FolderName
        Pattern category folder path.
    .PARAMETER ProjectPaths
        Project paths relative to the repository root.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$SolutionPath,

        [Parameter(Mandatory = $true)]
        [string]$FolderName,

        [Parameter(Mandatory = $true)]
        [string[]]$ProjectPaths
    )

    $document = [System.Xml.XmlDocument]::new()
    $document.Load($SolutionPath)
    $folder = $document.SelectSingleNode("/Solution/Folder[@Name='$FolderName']")
    if ($null -eq $folder) {
        $folder = $document.CreateElement('Folder')
        $nameAttribute = $document.CreateAttribute('Name')
        $nameAttribute.Value = $FolderName
        [void]$folder.Attributes.Append($nameAttribute)
        [void]$document.DocumentElement.AppendChild($folder)
    }

    foreach ($projectPath in $ProjectPaths) {
        $project = $document.CreateElement('Project')
        $pathAttribute = $document.CreateAttribute('Path')
        $pathAttribute.Value = $projectPath
        [void]$project.Attributes.Append($pathAttribute)
        [void]$folder.AppendChild($project)
    }

    Write-XmlDocument -Document $document -Path $SolutionPath
}

function New-PatternReadme {
    <#
    .SYNOPSIS
        Writes the standard pattern README headings.
    .PARAMETER Path
        README file path.
    .PARAMETER PatternName
        PascalCase pattern name.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$PatternName
    )

    $lines = @(
        "# $PatternName",
        '',
        '## Intent',
        '',
        'Describe the pattern in your own words and cite the GoF chapter.',
        '',
        '## The smell',
        '',
        'Describe the baseline smell and link to its `SMELL` marker.',
        '',
        '## The refactor',
        '',
        'Explain the participants and include a Mermaid class diagram.',
        '',
        '## Diff highlights',
        '',
        'Summarize the changes and link to [DIFF.md](DIFF.md).',
        '',
        '## Trade-offs',
        '',
        'Explain the costs and when not to use this pattern.',
        '',
        '## Related patterns and common confusions',
        '',
        'Compare this pattern with its nearest neighbors.',
        '',
        '## Modern .NET notes',
        '',
        'Describe relevant platform features.',
        '',
        '## Files changed',
        '',
        '* `src/CornerStore/`',
        '* `tests/CornerStore.Tests/`',
        '',
        '## Blog post link',
        '',
        'To be published.'
    )
    [System.IO.File]::WriteAllText($Path, ($lines -join "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
}

#endregion Functions

#region Main Execution

if ($MyInvocation.InvocationName -ne '.') {
    try {
        $repositoryRoot = Find-RepositoryRoot -StartPath $PSScriptRoot
        $pascalName = ConvertTo-PascalName -Value $Name
        $patternRoot = Join-Path $repositoryRoot (Join-Path "patterns/$Category" $Name)
        if (Test-Path -LiteralPath $patternRoot) {
            throw "Pattern folder already exists: $patternRoot"
        }

        New-Item -ItemType Directory -Path $patternRoot -Force | Out-Null
        Copy-ProjectTree -Source (Join-Path $repositoryRoot 'baseline/src') -Destination (Join-Path $patternRoot 'src')
        Copy-ProjectTree -Source (Join-Path $repositoryRoot 'baseline/tests') -Destination (Join-Path $patternRoot 'tests')
        New-PatternSolution -Path (Join-Path $patternRoot "$pascalName.slnx")
        New-PatternReadme -Path (Join-Path $patternRoot 'README.md') -PatternName $pascalName

        $relativeRoot = "patterns/$Category/$Name"
        $projects = @(
            "$relativeRoot/src/CornerStore/CornerStore.csproj",
            "$relativeRoot/tests/CornerStore.Tests/CornerStore.Tests.csproj"
        )
        Add-ProjectsToSolution `
            -SolutionPath (Join-Path $repositoryRoot 'GangOfFour.slnx') `
            -FolderName "/patterns/$Category/" `
            -ProjectPaths $projects

        Write-Host "Created $Category/$Name from baseline."
        exit 0
    }
    catch {
        Write-Error -ErrorAction Continue "new-pattern failed: $($_.Exception.Message)"
        exit 1
    }
}

#endregion Main Execution