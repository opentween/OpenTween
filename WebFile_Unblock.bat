@echo off
echo Removing Mark of the Web (Zone.Identifier)...
powershell -Command "Get-ChildItem -Path '.' -Recurse -File | ForEach-Object { Remove-Item -Path $_.FullName -Stream Zone.Identifier -ErrorAction SilentlyContinue }"
echo Finish
pause
