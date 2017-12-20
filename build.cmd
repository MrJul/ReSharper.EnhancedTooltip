rmdir /s /q build
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" src\GammaJul.ReSharper.EnhancedTooltip.sln /p:Platform=x86 /p:Configuration=Release
"%USERPROFILE%\.nuget\packages\NuGet.CommandLine\4.3.0\tools\nuget.exe" pack src\GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.nuspec -NoPackageAnalysis -OutputDirectory build\Release
