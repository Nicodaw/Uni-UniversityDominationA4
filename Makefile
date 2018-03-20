# the path to the Unity application
UNITY_APP=/Applications/Unity/Unity.app/Contents/MacOS/Unity
# dirs
BUILD_DIR=Builds/
BUILD_LOG=$(BUILD_DIR)build.log
ARCHIVE_DIR=Archives/
# build paths
BUILD_ANDROID=$(BUILD_DIR)UniversityDomination.apk
BUILD_LINUX32=$(BUILD_DIR)linux32
BUILD_LINUX64=$(BUILD_DIR)linux64
BUILD_LINUXUNIVERSAL=$(BUILD_DIR)linuxUniversal
BUILD_MACOS=UniversityDomination.app
BUILD_WIN32=$(BUILD_DIR)win32
BUILD_WIN64=$(BUILD_DIR)win64

# default action
all: clean build archive

# build action
build:
	$(UNITY_APP) -batchmode -runEditorTests -logFile "Builds/build.log" -quit -executeMethod ProjectBuilder.BuildProject

# creates archives for build uploading
archive:
	mkdir -p $(BUILD_DIR)$(ARCHIVE_DIR)
	cp -f $(BUILD_ANDROID) $(BUILD_DIR)$(ARCHIVE_DIR)android.apk
	cd $(BUILD_LINUX32) && zip -qr ../$(ARCHIVE_DIR)linux32 *
	cd $(BUILD_LINUX64) && zip -qr ../$(ARCHIVE_DIR)linux64 *
	cd $(BUILD_LINUXUNIVERSAL) && zip -qr ../$(ARCHIVE_DIR)linuxUniversal *
	cd $(BUILD_DIR) && zip -qr $(ARCHIVE_DIR)macOS $(BUILD_MACOS)
	cd $(BUILD_WIN32) && zip -qr ../$(ARCHIVE_DIR)win32 *
	cd $(BUILD_WIN64) && zip -qr ../$(ARCHIVE_DIR)win64 *

# clean action
clean:
	rm -fR Builds/*

.PHONY: build clean
