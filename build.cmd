@echo off
echo Building project...
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -runEditorTests -logFile "Builds/build.log" -quit -executeMethod ProjectBuilder.BuildProject
echo Project Built!
pause