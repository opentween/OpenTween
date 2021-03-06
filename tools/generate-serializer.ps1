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
# Reproducible Build に対応した OpenTween.XmlSerializers.dll の生成を行うスクリプト
#
# 使い方:
#   .\tools\generate-serializer.ps1 -ExePath ".\OpenTween\bin\Debug\OpenTween.exe" -SgenOpts "/type:OpenTween.SettingCommon"
#

Param(
  [Parameter(Mandatory = $true)][String] $ExePath,
  [String] $SgenOpts = ""
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'

$binDir = Split-Path -Parent $ExePath
$exeName = Split-Path -Leaf $ExePath

$dllName = $exeName -replace "\.exe$", ".XmlSerializers.dll"
$dllPath = Join-Path $binDir $dllName

. .\tools\functions.ps1

Function Generate-Serializer() {
  $tempDirName = "temp"
  $tempDir = Join-Path $binDir $tempDirName
  $tempExePath = Join-Path $tempDir $exeName

  # sgen は *.exe と同じディレクトリにソースコードを書き出すため、作業用のディレクトリを用意してビルドを実行する
  Remove-Item -Recurse $tempDir -ErrorAction Ignore
  New-Item -ItemType "directory" -Path $binDir -Name $tempDirName | Out-Null
  Copy-Item $ExePath -Destination $tempDir

  # sgen が実行する C# コンパイラは Roslyn ではないため /deterministic オプションに対応していない。
  # sgen の /keep オプションによってソースコードと csc に渡すコマンドラインオプションが書き出されるため、これを使用して改めて Roslyn でコンパイルし直す
  Invoke-NativeCommand "sgen /nologo /silent /keep $SgenOpts $tempExePath"
  $cmdlinePath = Join-Path $tempDir "*.cmdline"
  Invoke-NativeCommand "csc -nologo -warn:1 -deterministic $(Get-Content $cmdlinePath)"

  Copy-Item $(Join-Path $tempDir $dllName) -Destination $binDir
  Remove-Item -Recurse $tempDir
}

Generate-Serializer
