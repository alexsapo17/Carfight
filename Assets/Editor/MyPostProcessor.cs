using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif

public class MyPostProcessor : IPostprocessBuildWithReport
{
    private static Tuple<string, string>[] IosBuildValuesToSetInProject = {
        new Tuple<string, string>("ENABLE_BITCODE", "NO"),
    };

    int IOrderedCallback.callbackOrder => int.MaxValue;

    public void OnPostprocessBuild(BuildReport report) {
        BuildTarget target = report.summary.platform;
        string appPath = report.summary.outputPath;

        if (target != BuildTarget.iOS) {
            Debug.LogError("OnIosPostProcess - Called on non iOS build target!");
            return;
        }
        string pbxprojPath = PBXProject.GetPBXProjectPath(appPath);
        if (File.Exists(pbxprojPath)) {
            PBXProject project = new PBXProject();
            project.ReadFromFile(pbxprojPath);
            foreach (var bv in IosBuildValuesToSetInProject)
            {
                project.SetBuildProperty(project.ProjectGuid(), bv.Item1, bv.Item2);
            }
            project.WriteToFile(pbxprojPath);
        }
    }
}