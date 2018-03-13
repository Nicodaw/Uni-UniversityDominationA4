using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProjectBuilder
{
    const string buildPath = "Builds/";
    static readonly string[] scenes = {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/MainGame.unity",
        "Assets/Doom/Scenes/DoomMinigame.unity"
    };

    [MenuItem("Tools/Build")]
    public static void BuildProject()
    {
        Directory.CreateDirectory(buildPath);

        // do builds
#if UNITY_2017_3_OR_NEWER
        PerformBuild("macOS", BuildTarget.StandaloneOSX, "UniversityDomination.app");
#else
        PerformBuild("macOS", BuildTarget.StandaloneOSXIntel64, "UniversityDomination.app");
#endif
        PerformBuild("win32", BuildTarget.StandaloneWindows, "win32/UniversityDomination32.exe");
        PerformBuild("win64", BuildTarget.StandaloneWindows64, "win64/UniversityDomination64.exe");
        PerformBuild("android", BuildTarget.Android, "UniversityDomination.apk");
        PerformBuild("linux-universal", BuildTarget.StandaloneLinuxUniversal, "linuxUniversal/UniversityDomination");
        PerformBuild("linux32", BuildTarget.StandaloneLinux, "linux32/UniversityDomination");
        PerformBuild("linux64", BuildTarget.StandaloneLinux64, "linux64/UniversityDomination");
    }

    static void PerformBuild(string kind, BuildTarget target, string name)
    {
        Debug.Log(string.Format("=@= Building {0} =@=", kind));
        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            scenes = scenes,
            locationPathName = buildPath + name,
            target = target,
            options = BuildOptions.None
        });
        Debug.Log("=@= Build complete! =@=");
    }
}
