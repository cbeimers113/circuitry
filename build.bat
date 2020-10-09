@echo off
title Build

"C:\Program Files\Unity\Hub\Editor\2020.1.5f1\Editor\Unity.exe" -batchmode -quit -executeMethod Builder.DevelopmentBuild
tar.exe -a -c -f Techne_Win64.zip Build
rmdir /s /q Build

set /p VERSION=<.version
git add *
git commit -m "Alpha 1.%VERSION%"
git push --force
set /a VERSION=%VERSION% + 1
echo %VERSION%> .version
pause