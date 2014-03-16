sed -i "s/<version>.*<\/version>/<version>${BUILD_NUM}<\/version>/g" build/SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Pack build/SimpleDb.Net.nuspec -NonInteractive
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Push SimpleDb.Net.${BUILD_NUM}.nupkg $NUGET_APIKEY -NonInteractive
