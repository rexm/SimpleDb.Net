NUGET_PACKAGE_NUM=$BUILD_NUM
if [ "$TRAVIS_BRANCH" != "master" ]; then
	NUGET_PACKAGE_NUM+="-${TRAVIS_BRANCH}${TRAVIS_BUILD_NUMBER}"
fi
sed -i "s/<version>.*<\/version>/<version>${NUGET_PACKAGE_NUM}<\/version>/g" SimpleDb.Net.nuspec
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Pack SimpleDb.Net.nuspec -NonInteractive
mono --runtime=v4.0.30319 source/.nuget/NuGet.exe Push SimpleDb.Net.${NUGET_PACKAGE_NUM}.nupkg $NUGET_APIKEY -NonInteractive
