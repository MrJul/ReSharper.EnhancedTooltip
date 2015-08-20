rmdir /s /q Deploy
rmdir /s /q bin
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" Build.targets
mkdir Deploy\9.2
packages\NuGet.CommandLine.2.8.6\tools\NuGet.exe pack GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.nuspec -NoPackageAnalysis -OutputDirectory Deploy\9.2
