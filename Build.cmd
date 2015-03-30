"%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" Build.targets
mkdir Deploy\8.2
mkdir Deploy\9.0
mkdir Deploy\9.1
nuget.exe pack GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.8.2.nuspec -NoPackageAnalysis -OutputDirectory Deploy\8.2
nuget.exe pack GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.9.0.nuspec -NoPackageAnalysis -OutputDirectory Deploy\9.0
nuget.exe pack GammaJul.ReSharper.EnhancedTooltip\GammaJul.ReSharper.EnhancedTooltip.9.1.nuspec -NoPackageAnalysis -OutputDirectory Deploy\9.1
