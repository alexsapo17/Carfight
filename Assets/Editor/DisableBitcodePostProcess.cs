#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class DisableBitcodePostProcess
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            // Path to the Xcode project
            string pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(pbxPath));

            string targetGuid = pbxProject.GetUnityMainTargetGuid();

            // Disable Bitcode
            pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // Write the modified project back to the file
            File.WriteAllText(pbxPath, pbxProject.WriteToString());
        }
    }
}
#endif
