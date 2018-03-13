# the path to the Unity application
UNITY_APP="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

# default action
all: clean build

# build action
build:
	$(UNITY_APP) -batchmode -runEditorTests -logFile "Builds/build.log" -quit -executeMethod ProjectBuilder.BuildProject

# clean action
clean:
	rm -fR Builds/*

.PHONY: build clean
