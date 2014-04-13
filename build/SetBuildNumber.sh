SHOUD_INCREMENT_BUILD_NUM=$1
IFS='.'
read -ra VERS_SEG < "build/version.txt"
BUILD_NUM=""
for((i=0; i<${#VERS_SEG[@]}-1; i++ ));
do
  BUILD_NUM+="${VERS_SEG[$i]}."
done
LAST_SEG=${VERS_SEG[${#VERS_SEG[@]}-1]}
if [ -n "$SHOULD_INCREMENT_BUILD_NUM"  ]; then let LAST_SEG+=1 fi
BUILD_NUM+=$LAST_SEG
echo "Build number: $BUILD_NUM"
export BUILD_NUM
unset IFS
