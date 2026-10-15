#!/usr/bin/env pwsh
# Copyright (c) Microsoft Corporation.
# SPDX-License-Identifier: MIT
#Requires -Version 5.1

<#
.SYNOPSIS
    Writes a stable source and test diff against the baseline.
.DESCRIPTION
    Produces a file summary and unified diff for one pattern folder, excluding build output.
.PARAMETER Category
    Pattern category: creational, structural or behavioral.
.PARAMETER Name
    Pattern name in kebab-case.
.EXAMPLE
    ./tools/diff-baseline.ps1 -Category structural -Name facade
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

function Get-FileMap {
    <#
    .SYNOPSIS
        Maps relative file paths to their full paths.
    .PARAMETER Root
        Directory to enumerate.
    .OUTPUTS
        [hashtable] Relative file map.
    #>
    [CmdletBinding()]
    [OutputType([hashtable])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Root
    )

    $files = @{}
    foreach ($file in Get-ChildItem -LiteralPath $Root -Recurse -File) {
        $relativePath = $file.FullName.Substring($Root.Length).TrimStart([char[]]@('\', '/'))
        $files[$relativePath.Replace('\', '/')] = $file.FullName
    }

    return $files
}

function Get-LineCount {
    <#
    .SYNOPSIS
        Counts lines in a text file.
    .PARAMETER Path
        File path.
    .OUTPUTS
        [int] Line count.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return [System.IO.File]::ReadAllLines($Path).Length
}

function Get-LineChanges {
    <#
    .SYNOPSIS
        Gets added and removed line counts from Git.
    .PARAMETER BaselinePath
        Baseline file path.
    .PARAMETER PatternPath
        Pattern file path.
    .OUTPUTS
        [int[]] Added and removed line counts.
    #>
    [CmdletBinding()]
    [OutputType([int[]])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaselinePath,

        [Parameter(Mandatory = $true)]
        [string]$PatternPath
    )

    $output = @(& git -c core.autocrlf=false -c core.safecrlf=false diff --no-index --numstat -- $BaselinePath $PatternPath)
    $exitCode = $LASTEXITCODE
    if ($exitCode -gt 1) {
        throw "git diff --numstat failed with exit code $exitCode."
    }

    if ($output.Count -eq 0) {
        return @(0, 0)
    }

    $columns = ([string]$output[0]) -split "`t"
    if ($columns[0] -eq '-' -or $columns[1] -eq '-') {
        return @(0, 0)
    }

    return @([int]$columns[0], [int]$columns[1])
}

function Get-DirectoryDiff {
    <#
    .SYNOPSIS
        Gets a unified diff between two directories.
    .PARAMETER WorkingDirectory
        Directory from which relative paths should be used.
    .PARAMETER BaselinePath
        Baseline directory path relative to the working directory.
    .PARAMETER PatternPath
        Pattern directory path relative to the working directory.
    .OUTPUTS
        [string] Unified diff text.
    #>
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$WorkingDirectory,

        [Parameter(Mandatory = $true)]
        [string]$BaselinePath,

        [Parameter(Mandatory = $true)]
        [string]$PatternPath
    )

    Push-Location -LiteralPath $WorkingDirectory
    try {
        $output = @(& git -c core.autocrlf=false -c core.safecrlf=false diff --no-index --no-color -- $BaselinePath $PatternPath)
        $exitCode = $LASTEXITCODE
        if ($exitCode -gt 1) {
            throw "git diff failed with exit code $exitCode."
        }

        if ($output.Count -eq 0) {
            return ''
        }

        $diffLines = @(
            $output |
                ForEach-Object { [string]$_ } |
                Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
        )
        if ($diffLines.Count -eq 0) {
            return ''
        }

        return [string]::Join("`n", [string[]]$diffLines)
    }
    finally {
        Pop-Location
    }
}

function Get-TreeSummary {
    <#
    .SYNOPSIS
        Builds file-level change rows for two directory trees.
    .PARAMETER BaselineRoot
        Baseline tree root.
    .PARAMETER PatternRoot
        Pattern tree root.
    .OUTPUTS
        [string[]] Markdown table rows.
    #>
    [CmdletBinding()]
    [OutputType([string[]])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaselineRoot,

        [Parameter(Mandatory = $true)]
        [string]$PatternRoot
    )

    $baselineFiles = Get-FileMap -Root $BaselineRoot
    $patternFiles = Get-FileMap -Root $PatternRoot
    $paths = @($baselineFiles.Keys + $patternFiles.Keys | Sort-Object -Unique)
    $rows = @()
    foreach ($relativePath in $paths) {
        if (-not $baselineFiles.ContainsKey($relativePath)) {
            $rows += '| Added | ' + $relativePath + ' | ' + (Get-LineCount -Path $patternFiles[$relativePath]) + ' | 0 |'
            continue
        }

        if (-not $patternFiles.ContainsKey($relativePath)) {
            $rows += '| Removed | ' + $relativePath + ' | 0 | ' + (Get-LineCount -Path $baselineFiles[$relativePath]) + ' |'
            continue
        }

        $baselineHash = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($baselineFiles[$relativePath]))
        $patternHash = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($patternFiles[$relativePath]))
        if ($baselineHash -ne $patternHash) {
            $counts = Get-LineChanges -BaselinePath $baselineFiles[$relativePath] -PatternPath $patternFiles[$relativePath]
            $rows += '| Modified | ' + $relativePath + ' | ' + $counts[0] + ' | ' + $counts[1] + ' |'
        }
    }

    if ($rows.Count -eq 0) {
        return @()
    }

    return $rows
}

#endregion Functions

#region Main Execution

if ($MyInvocation.InvocationName -ne '.') {
    try {
        $repositoryRoot = Find-RepositoryRoot -StartPath $PSScriptRoot
        $patternRoot = Join-Path $repositoryRoot (Join-Path "patterns/$Category" $Name)
        if (-not (Test-Path -LiteralPath $patternRoot)) {
            throw "Pattern folder does not exist: $patternRoot"
        }

        $temporaryRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('go4-diff-' + [guid]::NewGuid().ToString('N'))
        try {
            $baselineSource = Join-Path $temporaryRoot 'baseline/src'
            $patternSource = Join-Path $temporaryRoot 'pattern/src'
            $baselineTests = Join-Path $temporaryRoot 'baseline/tests'
            $patternTests = Join-Path $temporaryRoot 'pattern/tests'
            Copy-ProjectTree -Source (Join-Path $repositoryRoot 'baseline/src') -Destination $baselineSource
            Copy-ProjectTree -Source (Join-Path $patternRoot 'src') -Destination $patternSource
            Copy-ProjectTree -Source (Join-Path $repositoryRoot 'baseline/tests') -Destination $baselineTests
            Copy-ProjectTree -Source (Join-Path $patternRoot 'tests') -Destination $patternTests

            $summaryRows = @()
            $summaryRows += Get-TreeSummary -BaselineRoot $baselineSource -PatternRoot $patternSource
            $summaryRows += Get-TreeSummary -BaselineRoot $baselineTests -PatternRoot $patternTests
            if ($summaryRows.Count -eq 0) {
                $summaryRows = @('| None | - | 0 | 0 |')
            }
            $sourceDiff = Get-DirectoryDiff -WorkingDirectory $temporaryRoot -BaselinePath 'baseline/src' -PatternPath 'pattern/src'
            $testDiff = Get-DirectoryDiff -WorkingDirectory $temporaryRoot -BaselinePath 'baseline/tests' -PatternPath 'pattern/tests'
            $diffBlocks = @()
            if (-not [string]::IsNullOrEmpty($sourceDiff)) { $diffBlocks += $sourceDiff }
            if (-not [string]::IsNullOrEmpty($testDiff)) { $diffBlocks += $testDiff }

            $lines = @(
                "# DIFF: $Name",
                '',
                '## File summary',
                '',
                '| Status | File | Added | Removed |',
                '| --- | --- | ---: | ---: |'
            )
            $lines += $summaryRows
            $lines += @('', '## Full diff', '')
            if ($diffBlocks.Count -eq 0) {
                $lines += 'No differences from the baseline.'
            }
            else {
                $lines += '```diff'
                foreach ($block in $diffBlocks) {
                    $lines += $block -split '\r?\n'
                }
                $lines += '```'
            }

            $diffPath = Join-Path $patternRoot 'DIFF.md'
            [System.IO.File]::WriteAllText($diffPath, ($lines -join "`n") + "`n", [System.Text.UTF8Encoding]::new($false))
            Write-Host "Wrote $diffPath"
            exit 0
        }
        finally {
            if (Test-Path -LiteralPath $temporaryRoot) {
                Remove-Item -LiteralPath $temporaryRoot -Recurse -Force
            }
        }
    }
    catch {
        Write-Error -ErrorAction Continue "diff-baseline failed: $($_.Exception.Message)"
        exit 1
    }
}

#endregion Main Execution