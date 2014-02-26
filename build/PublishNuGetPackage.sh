sed -i "s/<version>.*<\/version>/<version>${BUILD_NUM}<\/version>/g" build/SimpleDb.Net.nuspec
NuGet SetApiKey $NUGET_APIKEY
NuGet Pack build/SimpleDb.Net.nuspec
NuGet Push build/SimpleDb.Net.nupkg