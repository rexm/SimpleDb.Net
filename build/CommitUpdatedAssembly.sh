git config --global user.email "travis@travis-ci.org"
git config --global user.name "travis-ci"
git remote set-url origin git@github.com:rexm/SimpleDb.Net.git
git checkout master
cp -f source/Cucumber.SimpleDb/bin/Release/Cucumber.SimpleDb.dll .
echo -e $BUILD_NUM > version.txt
git commit Cucumber.SimpleDb.dll -m "Update binary [ci skip]"
git commit version.txt -m "Update version number [ci skip]"
git push origin master
