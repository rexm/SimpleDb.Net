echo -e $BUILD_NUM > build/version.txt
git commit build/version.txt -m "Update version number [ci skip]"
git push origin $TRAVIS_BRANCH
