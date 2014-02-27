IFS='.'
read -ra VERS_SEG < "version.txt"
VERS_INCREMENT=${VERS_SEG[${#VERS_SEG[@]}-1]}
let VERS_INCREMENT+=1
BUILD_NUM=""
for((i=0; i<${#VERS_SEG[@]}-1; i++ ));
do
  BUILD_NUM+="${VERS_SEG[$i]}."
done
BUILD_NUM+=$VERS_INCREMENT
echo "Build number: $BUILD_NUM"
export BUILD_NUM
unset IFS