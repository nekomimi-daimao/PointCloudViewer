#!/bin/sh

cd $(dirname $0)

root_dir=$(pwd | awk '{sub("Assets.*", "");print $0;}')

mpc -i "$root_dir" -o MessagePackGenerated.cs

# too slow...
# replace root_dir with your project dir
# root_dir=../../../

