cp -f source/Cucumber.SimpleDb/bin/Release/Cucumber.SimpleDb.dll .
git commit Cucumber.SimpleDb.dll -m "Update binary to latest ($BUILD_NUM) [ci skip]"
git push origin master
