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
# Reproducible Build に対応したサテライトアセンブリのビルド
#
# 使い方:
#   .\tools\build-satelite-assembly.ps1 -ObjDir .\OpenTween\obj\Debug\ -Culture en -AssemblyInfo .\OpenTween\Properties\AssemblyInfo.cs -DestPath .\OpenTween\bin\Debug\en\OpenTween.resources.dll
#

Param(
  [Parameter(Mandatory = $true)][String] $ObjDir,
  [Parameter(Mandatory = $true)][String] $Culture,
  [Parameter(Mandatory = $true)][String] $DestPath
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'

. .\tools\functions.ps1

Function Generate-AssemblyInfo() {
  $tmpFile = New-TemporaryFile
  $content = "[assembly: System.Reflection.AssemblyCulture(`"${Culture}`")]"
  $content > $tmpFile.FullName
  return $tmpFile.FullName
}

Function Build-SateliteAssembly() {
  $tmpAssemblyInfoPath = Generate-AssemblyInfo
  try {
    $resourcePaths = Get-ChildItem -Path $ObjDir -File -Filter "*.${Culture}.resources"
    $cscOpts = "-utf8output -nologo -deterministic -target:library -debug- -optimize+ -out:${DestPath}"
    $cscOpts += " " + ($resourcePaths | % { "-resource:$($_.FullName)" }) -join ' '
    Invoke-NativeCommand "csc $cscOpts $tmpAssemblyInfoPath"
  } finally {
    Remove-Item -Path $tmpAssemblyInfoPath
  }
}

Build-SateliteAssembly
