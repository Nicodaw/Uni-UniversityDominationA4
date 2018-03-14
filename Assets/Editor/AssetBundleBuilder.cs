using System;
using System.IO;
using UnityEditor;

public static class AssetBundleBuilder
{
    const string assetBundleDirectory = "Assets/AssetBundles/";

    [MenuItem("Tools/Build AssetBundles")]
    public static void BuildAllAssetBundles()
    {
        Directory.CreateDirectory(assetBundleDirectory);

        // perform asset bundle build for each platform
        foreach (Tuple<string, BuildTarget> buildItem in BuildHelper.BuildNames(true))
            PerformBuild(buildItem.Item1, buildItem.Item2);
    }

    static void PerformBuild(string kind, BuildTarget target)
    {
        string directory = assetBundleDirectory + kind;
        Directory.CreateDirectory(directory);
        BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, target);
    }
}
