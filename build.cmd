rmdir /s /q build
"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" src\GammaJul.ReSharper.EnhancedTooltip.sln /p:Configuration=Release
"%USERPROFILE%\.nuget\packages\NuGet.CommandLine\4.6.2\tools\nuget.exe" pack src\GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.nuspec -NoPackageAnalysis -OutputDirectory build\Release
