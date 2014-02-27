sed -i "s/<version>.*<\/version>/<version>${BUILD_NUM}<\/version>/g" build/SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/Cucumber.SimpleDb/.nuget/NuGet.exe SetApiKey $NUGET_APIKEY
mono --runtime=v4.0.30319 source/Cucumber.SimpleDb/.nuget/NuGet.exe Pack build/SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/Cucumber.SimpleDb/.nuget/NuGet.exe Push build/SimpleDb.Net.nupkg
