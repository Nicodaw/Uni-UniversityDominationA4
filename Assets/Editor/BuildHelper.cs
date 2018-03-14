using System;
using System.Collections.Generic;
using UnityEditor;

public static class BuildHelper
{
    public static IEnumerable<Tuple<string, BuildTarget>> BuildNames(bool includeiOS)
    {
#if UNITY_2017_3_OR_NEWER
        yield return Tuple.Create("macOS", BuildTarget.StandaloneOSX);
#else
        yield return Tuple.Create("macOS", BuildTarget.StandaloneOSXIntel64);
#endif
        yield return Tuple.Create("win32", BuildTarget.StandaloneWindows);
        yield return Tuple.Create("win64", BuildTarget.StandaloneWindows64);
        yield return Tuple.Create("android", BuildTarget.Android);
        if (includeiOS)
            yield return Tuple.Create("ios", BuildTarget.iOS);
        yield return Tuple.Create("linux-universal", BuildTarget.StandaloneLinuxUniversal);
        yield return Tuple.Create("linux32", BuildTarget.StandaloneLinux);
        yield return Tuple.Create("linux64", BuildTarget.StandaloneLinux64);
    }
}
