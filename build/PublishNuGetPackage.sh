sed -i "s/<version>.*<\/version>/<version>${BUILD_NUM}<\/version>/g" SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe SetApiKey $NUGET_APIKEY -NonInteractive
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Pack SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Push SimpleDb.Net.nupkg
