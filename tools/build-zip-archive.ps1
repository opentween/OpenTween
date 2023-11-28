# OpenTween - Client of Twitter
# Copyright (c) 2021 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
# All rights reserved.
#
# This file is part of OpenTween.
#
# This program is free software; you can redistribute it and/or modify it
# under the terms of the GNU General Public License as published by the Free
# Software Foundation; either version 3 of the License, or (at your option)
# any later version.
#
# This program is distributed in the hope that it will be useful, but
# WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
# or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
# for more details.
#
# You should have received a copy of the GNU General Public License along
# with this program. If not, see <http://www.gnu.org/licenses/>, or write to
# the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
# Boston, MA 02110-1301, USA.

#
# Reproducible Build に対応した ZIP アーカイブのビルドを行うスクリプト
#
# 使い方:
#   .\tools\build-zip-archive.ps1 -BinDir .\OpenTween\bin\Debug\ -DestPath OpenTween.zip
#

Param(
  [Parameter(Mandatory = $true)][String] $BinDir,
  [Parameter(Mandatory = $true)][String] $DestPath,
  [String] $HeadCommit = 'HEAD'
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'

$assemblyName = "OpenTween"

$exePath = Join-Path $BinDir "${assemblyName}.exe"
$pdbPath = Join-Path $BinDir "${assemblyName}.pdb"
$includeFiles = @(
  "en\",
  "Icons\",
  "LICENSE",
  "LICENSE.GPL-3",
  "LICENSE.ja",
  "LICENSE.LGPL-3",
  "${assemblyName}.exe",
  "${assemblyName}.exe.config"
)

. .\tools\functions.ps1

Function Get-SourceDateEpoch() {
  # 本来 $unixEpoch は UTC で表さなければならないが、ZIP アーカイブには
  # ローカルのタイムゾーンの日時でタイムスタンプが記録されるため、わざとタイムゾーンを指定していない。
  # これにより、生成される ZIP アーカイブには UTC での $sourceDateEpoch に相当する日時が記録されるようになる
  $unixEpoch = Get-Date "1970/01/01 00:00:00"
  $sourceDateUnixtime = [int](Invoke-NativeCommand "git log -1 --pretty=%ct ${HeadCommit}")
  $sourceDateEpoch = $unixEpoch.AddSeconds($sourceDateUnixtime)
  return $sourceDateEpoch
}

Function Reset-Timestamps([String[]] $Path, [DateTime] $Timestamp) {
  # ZIP アーカイブに含めるファイルのタイムスタンプを揃える
  Get-ChildItem $Path -Recurse | Set-ItemProperty -Name "LastWriteTime" -Value $Timestamp
}

Function Build-Package([String[]] $Path, [String] $DestPath) {
  Compress-Archive -Force -Path $Path -DestinationPath $DestPath
}

Function Get-CommandVersion([String] $Name) {
  Get-Command -Name $Name | Select -Property Name, @{Name='ProductVersion'; Expression={$_.FileVersionInfo.ProductVersion}}
}

Function Get-RuntimeVersion() {
  return [PSCustomObject]@{
    Name = 'RuntimeVersion'
    Value = [Attribute]::GetCustomAttribute(
      [Object].Assembly,
      [System.Reflection.AssemblyInformationalVersionAttribute]
    ).InformationalVersion
  }
}

$includePaths = $includeFiles | % { Join-Path $BinDir $_ }
$timestamp = Get-SourceDateEpoch

Reset-Timestamps -Path $includePaths -Timestamp $timestamp
Build-Package -Path $includePaths -DestPath $DestPath

Write-Host
Write-Host "Build success!"
@(
  Get-CommandVersion 'msbuild.exe'
  Get-CommandVersion 'csc.exe'
  Get-RuntimeVersion
  [PSCustomObject]@{
    Name = 'SOURCE_DATE_EPOCH'
    Value = $timestamp
  }
  Get-FileHash -Algorithm SHA256 $destPath
  # Get-FileHash -Algorithm SHA256 $pdbPath
) | Format-List
