sed -i "s/<version>.*<\/version>/<version>${BUILD_NUM}<\/version>/g" SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe SetApiKey $NUGET_APIKEY -NonInteractive
<<<<<<< HEAD
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Pack build/SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Push build/SimpleDb.Net.${BUILD_NUM}.nupkg
=======
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Pack SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Push SimpleDb.Net.nupkg
>>>>>>> FETCH_HEAD
