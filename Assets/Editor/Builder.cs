using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ProjectBuilder
{
    const string buildPath = "Builds/";
    const string testingResourcesBasePath = "Assets/Resources/Testing";
    const string testingResourcesBaseMeta = testingResourcesBasePath + ".meta";
    const string testingResourcesTmpPath = "Temp/TestingResources";
    const string testingResourcesTmpMeta = testingResourcesTmpPath + ".meta";
    static readonly string[] scenes = {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/MainGame.unity",
        "Assets/Minigame/Scenes/Minigame.unity"
    };
    static readonly Dictionary<string, string> nameMapping = new Dictionary<string, string>
    {
        {"macOS", "UniversityDomination.app"},
        {"win32","win32/UniversityDomination32.exe"},
        {"win64","win64/UniversityDomination64.exe"},
        {"android","UniversityDomination.apk"},
        {"linux-universal","linuxUniversal/UniversityDomination"},
        {"linux32","linux32/UniversityDomination"},
        {"linux64","linux64/UniversityDomination"}
    };

    public static void BuildProject()
    {
        Directory.CreateDirectory(buildPath);

        // move unneeded resources
        if (Directory.Exists(testingResourcesBasePath))
        {
            if (Directory.Exists(testingResourcesTmpPath))
            {
                Directory.Delete(testingResourcesTmpPath);
                File.Delete(testingResourcesTmpMeta);
            }
            Directory.Move(testingResourcesBasePath, testingResourcesTmpPath);
            File.Move(testingResourcesBaseMeta, testingResourcesTmpMeta);
        }

        // perform builds for all platforms
        foreach (Tuple<string, BuildTarget> buildItem in BuildHelper.BuildNames(false))
            PerformBuild(buildItem.Item1, buildItem.Item2, nameMapping[buildItem.Item1]);

        if (Directory.Exists(testingResourcesTmpPath))
        {
            Directory.Move(testingResourcesTmpPath, testingResourcesBasePath);
            File.Move(testingResourcesTmpMeta, testingResourcesBaseMeta);
        }
    }

    static void PerformBuild(string kind, BuildTarget target, string name)
    {
        Debug.Log(string.Format("=@= Building {0} =@=", kind));
        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            scenes = scenes,
            locationPathName = buildPath + name,
            target = target,
            options = BuildHelper.CurrentBuildOptions
        });
        Debug.Log("=@= Build complete! =@=");
    }
}
