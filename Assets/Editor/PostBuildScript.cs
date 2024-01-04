using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class PostBuildScript
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Path to the project
            string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

            // Load the project
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            string targetGuid = project.TargetGuidByName("Unity-iPhone");

            // Disable Bitcode
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            // Apply settings
            project.WriteToFile(projectPath);
        }
    }
}
